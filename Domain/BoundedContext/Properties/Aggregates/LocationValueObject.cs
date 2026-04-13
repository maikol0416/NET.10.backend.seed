using Domain.DomainShared;

namespace Domain.BoundedContext.Properties;

public record LocationValueObject : ValueObject
{
    public LocationValueObject(string number,
                               string detail,
                               string country,
                               string city,
                               string neighborhood)
    {
        if(string.IsNullOrEmpty(number))
            throw new DomainException("Number cannot be null");
        
        if(string.IsNullOrEmpty(detail))
            throw new DomainException("Detail cannot be null");
        
        if(string.IsNullOrEmpty(country))
            throw new DomainException("Country cannot be null");
        
        if(string.IsNullOrEmpty(city))
            throw new DomainException("City cannot be null");
        
        if(string.IsNullOrEmpty(neighborhood))
            throw new DomainException("Neighborhood cannot be null");

        Number = number;
        Detail = detail;
        Country = country;
        City = city;
        Neighborhood = neighborhood;
    }
    
    public string Number { get; private set; }
    public string Detail { get; private set; }
    public string Country { get; private set; }
    public string City { get; private set; }
    public string Neighborhood { get; private set; }
}
