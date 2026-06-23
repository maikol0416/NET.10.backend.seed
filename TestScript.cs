using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Infraestructure.Entity;
using Infraestructure.Entity.Context;
using Domain.BoundedContext.Properties;
using Domain.DomainShared;

class Program
{
    static async System.Threading.Tasks.Task Main()
    {
        var services = new ServiceCollection();
        services.AddDbContext<EntityDbContext>(options =>
            options.UseInMemoryDatabase("TestDb"));
        
        var sp = services.BuildServiceProvider();
        var db = sp.GetRequiredService<EntityDbContext>();
        
        var location = new LocationValueObject("1", "2", "3", "4", "5");
        var agg = new PhysicalStructureAgg("Test", "Nit", 10, location, new System.Collections.Generic.List<CommonAreaValueObject>(), new System.Collections.Generic.List<Tower>());
        agg.AddTower("Tower 1");
        
        db.PhysicalStructure.Add(agg);
        await db.SaveChangesAsync();
        
        var towerId = agg.Towers.First().Id;
        
        // Now remove it
        var tracked = await db.PhysicalStructure.Include(p => p.Towers).FirstAsync();
        tracked.RemoveTower(towerId);
        
        try {
            await db.SaveChangesAsync();
            Console.WriteLine("SUCCESS");
        } catch (Exception ex) {
            Console.WriteLine("ERROR: " + ex.GetType().Name + " - " + ex.Message);
        }
    }
}
