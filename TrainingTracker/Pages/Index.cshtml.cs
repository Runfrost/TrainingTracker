using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;

public class IndexModel : PageModel
{
    public List<Workout> Workouts { get; set; } = new();

    public void OnGet()
    {
        // Dummydata, ersätt med databaslogik
        Workouts = new List<Workout>
        {
            new Workout { Date = DateTime.Today, Type="Löpning" },
            new Workout { Date = DateTime.Today.AddDays(-1), Type="Styrka" },
            new Workout { Date = DateTime.Today.AddDays(-3), Type="Yoga" }
        };
    }
}

public class Workout
{
    public DateTime Date { get; set; }
    public string Type { get; set; }
}
