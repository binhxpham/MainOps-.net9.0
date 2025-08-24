using System;
using System.ComponentModel.DataAnnotations;

namespace CustomDataAnnotations
{
    public class CurrentDateAttribute : ValidationAttribute
    {
        public CurrentDateAttribute()
        {
        }

        public override bool IsValid(object value)
        {
            var dt = (DateTime)value;
            if (dt >= DateTime.Now)
            {
                return true;
            }
            return false;
        }
        
    }
    public class IsPositiveAttribute : ValidationAttribute
    {
        public IsPositiveAttribute()
        {
        }

        public override bool IsValid(object value)
        {
            var val = (int)value;
            if (val >= 1)
            {
                return true;
            }
            return false;
        }

    }

}