//V1

//methods to validate user input, specifically when entering an estimated time for a custom chore.
//InputValidation.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrapApple
{
    public static class InputValidationUtil
    {
        public static bool ValidateEstimatedTime(string input, out int estimatedTime)
        {
            estimatedTime = -1;
            //Checking if input is null or empty etc
            if (string.IsNullOrWhiteSpace(input))
            {
                return false;
            }
            //try to parse the input as an integer
            if (int.TryParse(input, out estimatedTime))
            {
                //if parsing succeeds, check if the estimated time is >=0
                return estimatedTime >= 0;
            }
            return false;
        }

    }
}
