using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models
{
    public class HourSchedule
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public double day01 { get; set; }
        public double day02 { get; set; }
        public double day03 { get; set; }
        public double day04 { get; set; }
        public double day05 { get; set; }
        public double day06 { get; set; }
        public double day07 { get; set; }
        public double day08 { get; set; }
        public double day09 { get; set; }
        public double day10 { get; set; }
        public double day11 { get; set; }
        public double day12 { get; set; }
        public double day13 { get; set; }
        public double day14 { get; set; }
    }
}
