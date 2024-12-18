namespace Business.Helpers
{
    public static class AgeCounter
    {
        public static int Count(DateTime? birthday)
        {
            var today = DateTime.Today;

            // Calculate the age.
            var age = today.Year - birthday.Value.Year;

            // Go back to the year in which the person was born in case of a leap year
            if (birthday.Value.Date > today.AddYears(-age)) age--;

            return age;
        }
    }
}
