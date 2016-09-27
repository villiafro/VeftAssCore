namespace CoursesAPI.Services.Utilities
{
	public class DateTimeUtils
	{
		public static bool IsLeapYear(int year)
		{
			//a year is a leap year if it is divisible by 4
			if(year%4==0){
				//unless it is divisible by 100
				if(year%100!=0){
					return true;
				}
				else{
					//except when it is divisible by 400
					if(year%400==0){
						return true;
					}
				}
			}
			return false;
		}
	}
}
