namespace Application.Services.Interfaces
{
    public interface ICountryNameHandler
    {
        public string GenerateCountryName();
        public bool IsCountryNameValid(string countryName);
    }
}
