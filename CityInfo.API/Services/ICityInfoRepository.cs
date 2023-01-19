using CityInfo.API.Entities;

namespace CityInfo.API.Services;

public interface ICityInfoRepository
{
    public Task<IEnumerable<City>> GetCitiesAsync();

    public Task<(IEnumerable<City>, PaginationMetadata)> GetCitiesAsync(string? name, string? searchQuery,
        int pageNumber, int pageSize);

    public Task<City?> GetCityAsync(int cityId, bool includePointsOfInterest);
    public Task<IEnumerable<PointOfInterest>> GetPointsOfInterestForCityAsync(int cityId);
    public Task<PointOfInterest?> GetPointOfInterestForCityAsync(int cityId, int pointOfInterestId);
    public Task<bool> CityExistsAsync(int cityId);
    public Task AddPointOfInterestForCityAsync(int cityId, PointOfInterest pointOfInterest);
    public void DeletePointOfInterest(PointOfInterest pointOfInterest);
    public Task<bool> CityNameMatchesCityId(string? cityName, int cityId);
    public Task<bool> SaveChangesAsync();
}
