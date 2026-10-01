using System.Diagnostics;
using System.Globalization;
using System.Text;
using MainOps.Data;
using MainOps.Models;
using MainOps.Models.ReportClasses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MainOps.Controllers;

public class HomeController(
    IHttpContextAccessor httpContextAccessor,
    DataContext context,
    UserManager<ApplicationUser> userManager,
    IWebHostEnvironment env) : Controller
{
    private readonly IHttpContextAccessor _http = httpContextAccessor;
    private readonly DataContext _context = context;
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IWebHostEnvironment _env = env;

    private const int BackupColumnCount = 24;

    private static readonly string[] BackupHeaders =
    [
        "Id", "short_Description", "Report_Date", "Starthour", "Endhour",
        "Work_Performed", "Extra_Works", "DoneBy", "TitleId", "ProjectId",
        "tobepaid", "Signature", "Amount", "Machinery", "SafetyHours",
        "StandingTime", "EnteredIntoDataBase", "LastEdited", "Checked_By",
        "Report_Checked", "SubProjectId", "OtherPeople", "HasPhotos", "OtherPeopleIDs"
    ];

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public IActionResult LoadBackupData()
    {
        return View();
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> LoadBackupData(IFormFile? postedFile)
    {
        if (postedFile is null || postedFile.Length == 0)
        {
            TempData["Error"] = "No file was uploaded.";
            return RedirectToAction(nameof(Index));
        }

        if (!Path.GetExtension(postedFile.FileName).Equals(".csv", StringComparison.OrdinalIgnoreCase))
        {
            TempData["Error"] = "Only CSV files are allowed.";
            return RedirectToAction(nameof(Index));
        }

        var reports = new List<Daily_Report_2>();

        using var stream = postedFile.OpenReadStream();
        using var reader = new StreamReader(stream);

        _ = await reader.ReadLineAsync(); // skip header

        var lineNumber = 1;

        while (!reader.EndOfStream)
        {
            lineNumber++;
            var line = await reader.ReadLineAsync();

            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            var row = line.Split(';');

            if (row.Length < BackupColumnCount)
            {
                TempData["Error"] = $"CSV import failed at line {lineNumber}: expected {BackupColumnCount} columns, got {row.Length}.";
                return RedirectToAction(nameof(Index));
            }

            if (!TryMapDailyReport(row, out var report))
            {
                TempData["Error"] = $"CSV import failed at line {lineNumber}: invalid data format.";
                return RedirectToAction(nameof(Index));
            }

            reports.Add(report);
        }

        if (reports.Count == 0)
        {
            TempData["Error"] = "The CSV file did not contain any valid rows.";
            return RedirectToAction(nameof(Index));
        }

        await _context.Daily_Report_2s.AddRangeAsync(reports);
        await _context.SaveChangesAsync();

        TempData["Success"] = $"{reports.Count} backup rows imported successfully.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> GetBackUpData()
    {
        var reports = await _context.Daily_Report_2s
            .AsNoTracking()
            .Where(x => x.TitleId == 637 || x.TitleId == 638 || x.TitleId == 639)
            .OrderBy(x => x.Report_Date)
            .ThenBy(x => x.StartHour)
            .ToListAsync();

        var sb = new StringBuilder();
        sb.AppendLine(string.Join(';', BackupHeaders));

        foreach (var dr in reports)
        {
            var row = new string[BackupColumnCount];

            row[0] = dr.Id.ToString(CultureInfo.InvariantCulture);
            row[1] = dr.short_Description ?? string.Empty;
            row[2] = dr.Report_Date.ToString("O", CultureInfo.InvariantCulture);
            row[3] = dr.StartHour.ToString();
            row[4] = dr.EndHour.ToString();
            row[5] = (dr.Work_Performed ?? string.Empty).Replace("\r\n", "\\r\\n");
            row[6] = dr.Extra_Works ?? string.Empty;
            row[7] = dr.DoneBy ?? string.Empty;
            row[8] = MapTitleIdForBackup(dr.TitleId);
            row[9] = dr.ProjectId.ToString(CultureInfo.InvariantCulture);
            row[10] = dr.tobepaid?.ToString(CultureInfo.InvariantCulture);
            row[11] = dr.Signature ?? string.Empty;
            row[12] = dr.Amount.ToString(CultureInfo.InvariantCulture);
            row[13] = dr.Machinery ?? string.Empty;
            row[14] = dr.SafetyHours.ToString();
            row[15] = dr.StandingTime.ToString();
            row[16] = dr.EnteredIntoDataBase?.ToString(CultureInfo.InvariantCulture);
            row[17] = dr.LastEditedInDataBase?.ToString(CultureInfo.InvariantCulture);
            row[18] = dr.Checked_By ?? string.Empty;
            row[19] = dr.Report_Checked.ToString();
            row[20] = dr.SubProjectId?.ToString(CultureInfo.InvariantCulture);
            row[21] = dr.OtherPeople ?? string.Empty;
            row[22] = dr.HasPhotos.ToString();
            row[23] = dr.OtherPeopleIDs ?? string.Empty;

            sb.AppendLine(string.Join(';', row.Select(EscapeCsvField)));
        }

        return File(Encoding.UTF8.GetBytes(sb.ToString()), "text/csv", "dailyreportsbackup.csv");
    }
    

    [HttpGet]
    [Authorize(Roles = "Admin,DivisionAdmin,Manager,ProjectMember,StorageManager")]
    public IActionResult DownloadFile(string filename)
    {
        if (string.IsNullOrWhiteSpace(filename))
        {
            return BadRequest("Filename is required.");
        }

        var safeFileName = Path.GetFileName(filename);
        if (!string.Equals(safeFileName, filename, StringComparison.Ordinal))
        {
            return BadRequest("Invalid filename.");
        }

        var allowedFolder = Path.GetFullPath(Path.Combine(_env.WebRootPath, "AHAK", "Downloads"));
        var fullPath = Path.GetFullPath(Path.Combine(allowedFolder, safeFileName));

        if (!fullPath.StartsWith(allowedFolder, StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest("Invalid file path.");
        }

        if (!System.IO.File.Exists(fullPath))
        {
            return NotFound();
        }

        var contentType = "application/octet-stream";
        return PhysicalFile(fullPath, contentType, safeFileName);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,DivisionAdmin,Manager,ProjectMember,StorageManager")]
    public IActionResult RemoveFile(string filename)
    {
        if (string.IsNullOrWhiteSpace(filename))
        {
            return BadRequest("Filename is required.");
        }

        var safeFileName = Path.GetFileName(filename);
        if (!string.Equals(safeFileName, filename, StringComparison.Ordinal))
        {
            return BadRequest("Invalid filename.");
        }

        var allowedFolder = Path.GetFullPath(Path.Combine(_env.WebRootPath, "AHAK", "Downloads"));
        var fullPath = Path.GetFullPath(Path.Combine(allowedFolder, safeFileName));

        if (!fullPath.StartsWith(allowedFolder, StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest("Invalid file path.");
        }

        if (!System.IO.File.Exists(fullPath))
        {
            return NotFound();
        }

        System.IO.File.Delete(fullPath);

        TempData["Success"] = "File removed.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [Authorize(Roles = "Admin,DivisionAdmin,Manager,ProjectMember,StorageManager")]
    public async Task<IActionResult> GetBoQExtraWorkItems(string theId)
    {
        if (!int.TryParse(theId, out var boQHeadLineId))
        {
            return BadRequest("Invalid id.");
        }

        var data = await _context.BoQHeadLines
                                .AsNoTracking()
                                .Include(x => x.ExtraWorkBoQs)
                                    .ThenInclude(x => x.Headers)
                                        .ThenInclude(x => x.BoQItems)
                                .SingleOrDefaultAsync(x => x.Id == boQHeadLineId);

        if (data is null)
        {
            return NotFound();
        }

        return PartialView("_BoQHeadLine", data);
    }

    [Authorize(Roles = "Admin,DivisionAdmin,Manager,ProjectMember,StorageManager")]
    [HttpGet]
    public async Task<IActionResult> GetUserExtraWorks()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            return Unauthorized();
        }

        var data = await _context.BoQHeadLines
            .AsNoTracking()
            .Include(x => x.Project)
            .Include(x => x.ExtraWorkBoQs)
                .ThenInclude(x => x.Descriptions)
            .Where(x => x.Project.DivisionId == user.DivisionId && x.Type == "ExtraWork")
            .OrderBy(x => x.ProjectId)
            .ThenBy(x => x.BoQnum)
            .ToListAsync();

        var endData = data
            .Where(x => x.ExtraWorkBoQs.Count > 0)
            .ToList();

        return PartialView("_extraworkinfo", endData);
    }

    [Authorize(Roles = "Admin,DivisionAdmin,Manager,ProjectMember,StorageManager")]
    [HttpGet]
    public async Task<IActionResult> GetUserDownloads()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            return Unauthorized();
        }

        var data = await _context.PersonalFiles
            .AsNoTracking()
            .Where(x => x.ApplicationUserId == user.Id)
            .ToListAsync();

        return PartialView("_Downloads2", data);
    }

    [HttpGet]
    public async Task<int> GetAmountDownloads()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            return 0;
        }

        return await _context.PersonalFiles
            .AsNoTracking()
            .CountAsync(x => !x.Downloaded && x.ApplicationUserId == user.Id);
    }

    [HttpGet]
    [Authorize(Roles = "Admin,DivisionAdmin,Manager,ProjectMember,StorageManager")]
    public async Task<IActionResult> DownloadPersonalFile(int? id)
    {
        if (id is null)
        {
            return BadRequest();
        }

        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            return Unauthorized();
        }

        var file = await _context.PersonalFiles
            .SingleOrDefaultAsync(x => x.Id == id && x.ApplicationUserId == user.Id);

        if (file is null)
        {
            return NotFound();
        }

        if (!file.FileExtension.Contains("pdf", StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest("Only PDF files can be downloaded here.");
        }

        if (string.IsNullOrWhiteSpace(file.path) || !System.IO.File.Exists(file.path))
        {
            return NotFound("The physical file does not exist.");
        }

        var fileBytes = await System.IO.File.ReadAllBytesAsync(file.path);

        if (!file.Downloaded)
        {
            file.Downloaded = true;
            _context.Update(file);
            await _context.SaveChangesAsync();
        }

        var safeName = string.IsNullOrWhiteSpace(file.FileName) ? "download.pdf" : file.FileName;
        return File(fileBytes, "application/pdf", safeName);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,DivisionAdmin,Manager,ProjectMember,StorageManager")]
    public async Task<IActionResult> RemovePersonalFile(int? id)
    {
        if (id is null)
        {
            return BadRequest();
        }

        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            return Unauthorized();
        }

        var file = await _context.PersonalFiles
            .SingleOrDefaultAsync(x => x.Id == id && x.ApplicationUserId == user.Id);

        if (file is null)
        {
            return NotFound();
        }

        if (!string.IsNullOrWhiteSpace(file.path) && System.IO.File.Exists(file.path))
        {
            System.IO.File.Delete(file.path);
        }

        _context.PersonalFiles.Remove(file);
        await _context.SaveChangesAsync();

        TempData["Success"] = "File removed.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,DivisionAdmin,Manager,ProjectMember,StorageManager")]
    public async Task<IActionResult> RemoveAllPersonalFiles()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            return Unauthorized();
        }

        var files = await _context.PersonalFiles
            .Where(x => x.ApplicationUserId == user.Id)
            .ToListAsync();

        foreach (var file in files)
        {
            if (!string.IsNullOrWhiteSpace(file.path) && System.IO.File.Exists(file.path))
            {
                System.IO.File.Delete(file.path);
            }
        }

        _context.PersonalFiles.RemoveRange(files);
        await _context.SaveChangesAsync();

        TempData["Success"] = $"{files.Count} file(s) removed.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Index()
    {
        try
        {
            var url = _http.HttpContext?.Request.GetDisplayUrl();

            if (!string.IsNullOrWhiteSpace(url) &&
                (url.Contains("tjaden-maps", StringComparison.OrdinalIgnoreCase) ||
                 url.Contains("mainops-test", StringComparison.OrdinalIgnoreCase)))
            {
                Response.Cookies.Append(
                    CookieRequestCultureProvider.DefaultCookieName,
                    CookieRequestCultureProvider.MakeCookieValue(new RequestCulture("nl-NL")),
                    new CookieOptions
                    {
                        Expires = DateTimeOffset.UtcNow.AddYears(1),
                        HttpOnly = true,
                        IsEssential = true,
                        Secure = Request.IsHttps
                    });
            }

            return View();
        }
        catch (Exception ex)
        {
            return Content($"Error in Index: {ex.Message}");
        }
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult> Footer(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var well = await _context.Wells
            .AsNoTracking()
            .Include(x => x.Project).ThenInclude(x => x.Division)
            .Include(x => x.CoordSystem)
            .Include(x => x.SubProject)
            .SingleOrDefaultAsync(x => x.Id == id);

        if (well is null)
        {
            return NotFound();
        }

        well.BentoniteLayers = await _context.BentoniteWellLayers
            .AsNoTracking()
            .Include(x => x.CastingType)
            .Where(x => x.WellId == well.Id)
            .OrderBy(x => x.meter_start)
            .ToListAsync();

        well.SoilSamples = await _context.SoilSamples
            .AsNoTracking()
            .Where(x => x.WellId == well.Id)
            .OrderBy(x => x.sample_meter)
            .ToListAsync();

        well.FilterLayers = await _context.FilterWellLayers
            .AsNoTracking()
            .Where(x => x.WellId == well.Id)
            .OrderBy(x => x.meter_start)
            .ToListAsync();

        well.SandLayers = await _context.SandWellLayers
            .AsNoTracking()
            .Include(x => x.SandType)
            .Where(x => x.WellId == well.Id)
            .OrderBy(x => x.meter_start)
            .ToListAsync();

        well.WellLayers = await _context.WellLayers
            .AsNoTracking()
            .Include(x => x.Layer)
            .Where(x => x.WellId == well.Id)
            .OrderBy(x => x.Start_m)
            .ToListAsync();

        return View(well);
    }

    [HttpGet]
    public IActionResult MoreInformation()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Directions()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Contact()
    {
        return View();
    }

    [HttpGet]
    public IActionResult ErrorMessage(string text)
    {
        var model = new ErrorModel { ErrorText = text };
        return View("Error", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult SetLanguage(string culture, string returnUrl)
    {
        if (string.IsNullOrWhiteSpace(culture))
        {
            return BadRequest("Culture is required.");
        }

        if (string.IsNullOrWhiteSpace(returnUrl) || !Url.IsLocalUrl(returnUrl))
        {
            return RedirectToAction(nameof(Index));
        }

        Response.Cookies.Append(
            CookieRequestCultureProvider.DefaultCookieName,
            CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
            new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddYears(1),
                HttpOnly = true,
                IsEssential = true,
                Secure = Request.IsHttps
            });

        return LocalRedirect(returnUrl);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [HttpGet]
    public IActionResult Error()
    {
        return View(new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        });
    }

    private static string MapTitleIdForBackup(int titleId) =>
        titleId switch
        {
            637 => "764",
            638 => "762",
            639 => "763",
            _ => titleId.ToString(CultureInfo.InvariantCulture)
        };

    private static string EscapeCsvField(string? value)
    {
        value ??= string.Empty;

        if (!value.Contains(';') && !value.Contains('"') && !value.Contains('\n') && !value.Contains('\r'))
        {
            return value;
        }

        return $"\"{value.Replace("\"", "\"\"")}\"";
    }

    private static bool TryMapDailyReport(string[] row, out Daily_Report_2 report)
    {
        report = null!;

        if (!TryParseInt(row[0], out var id)) return false;
        if (!TryParseDateTime(row[2], out var reportDate)) return false;
        if (!TimeSpan.TryParse(row[3], out var startHour)) return false;
        if (!TimeSpan.TryParse(row[4], out var endHour)) return false;
        if (!TryParseInt(row[8], out var titleId)) return false;
        if (!TryParseInt(row[9], out var projectId)) return false;
        if (!TryParseInt(row[12], out var amount)) return false;
        if (!TryParseBool(row[19], out var reportChecked)) return false;
        if (!TryParseBool(row[22], out var hasPhotos)) return false;

        report = new Daily_Report_2
        {
            Id = id,
            short_Description = row[1],
            Report_Date = reportDate,
            StartHour = startHour,
            EndHour = endHour,
            Work_Performed = (row[5] ?? string.Empty).Replace("\\r\\n", "\r\n"),
            Extra_Works = row[6],
            DoneBy = row[7],
            TitleId = titleId,
            ProjectId = projectId,
            Signature = row[11],
            Amount = amount,
            Machinery = row[13],
            SafetyHours = TryParseTimeSpanOrZero(row[14]),
            StandingTime = TryParseTimeSpanOrZero(row[15]),
            Checked_By = row[18],
            Report_Checked = reportChecked,
            OtherPeople = row[21],
            HasPhotos = hasPhotos,
            OtherPeopleIDs = row[23]
        };

        if (TryParseNullableInt(row[10], out var toBePaid))
        {
            report.tobepaid = toBePaid;
        }

        if (TryParseNullableDateTime(row[16], out var enteredIntoDatabase))
        {
            report.EnteredIntoDataBase = enteredIntoDatabase;
        }

        if (TryParseNullableDateTime(row[17], out var lastEdited))
        {
            report.LastEditedInDataBase = lastEdited;
        }

        if (TryParseNullableInt(row[20], out var subProjectId))
        {
            report.SubProjectId = subProjectId;
        }

        return true;
    }

    private static bool TryParseInt(string? value, out int result) =>
        int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out result);

    private static bool TryParseNullableInt(string? value, out int result)
    {
        result = default;
        return !string.IsNullOrWhiteSpace(value) &&
               int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out result);
    }

    private static bool TryParseDateTime(string? value, out DateTime result) =>
        DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out result);

    private static bool TryParseNullableDateTime(string? value, out DateTime result)
    {
        result = default;
        return !string.IsNullOrWhiteSpace(value) &&
               DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out result);
    }

    private static bool TryParseBool(string? value, out bool result) =>
        bool.TryParse(value, out result);

    private static TimeSpan TryParseTimeSpanOrZero(string? value) =>
        TimeSpan.TryParse(value, out var result) ? result : TimeSpan.Zero;
}