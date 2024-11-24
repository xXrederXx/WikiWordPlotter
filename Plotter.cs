using ScottPlot;

namespace WebScraper;

public class Plotter{
    private readonly Plot plot;
    private readonly List<double> xPositions;
    private readonly List<double> yValues;

    // settings
    private const bool showLine = true;
    private const bool darkMode = true;

    public Plotter(List<KeyValuePair<string, int>> totalData, int amountToShow)
    {
        List<KeyValuePair<string, int>> data = totalData.Take(amountToShow).ToList();

        xPositions = Enumerable.Range(0, data.Count).Select(x => (double)x).ToList();
        plot = new Plot();
        yValues = data.Select(x => (double)x.Value).ToList();

        MakeBars();

        if(showLine){
            MakeLine();
        }

        List<Tick> ticks = GenerateTicks(data);
        MakeTicks(ticks);

        ApplyStyle();

        plot.SavePng("res.png", 1920, 1080);

    }

    private void ApplyStyle()
    {
        if(darkMode){
            ApplyDarkModeStyle();
        }

        plot.Axes.Bottom.MinimumSize = 100;
        plot.Axes.Right.MinimumSize = 100;

        plot.Axes.Margins(bottom: 0);
    }

    private void ApplyDarkModeStyle()
    {
        // change background and text color
        plot.FigureBackground.Color = new("1c1c1e");
        plot.Axes.Color(new("999999"));

        // set grid line colors
        plot.Grid.XAxisStyle.MajorLineStyle.Color = Colors.White.WithAlpha(30);
        plot.Grid.YAxisStyle.MajorLineStyle.Color = Colors.White.WithAlpha(30);
        plot.Grid.XAxisStyle.MinorLineStyle.Color = Colors.White.WithAlpha(5);
        plot.Grid.YAxisStyle.MinorLineStyle.Color = Colors.White.WithAlpha(5);

        // enable minor grid lines by defining a positive width
        plot.Grid.XAxisStyle.MinorLineStyle.Width = 1;
        plot.Grid.YAxisStyle.MinorLineStyle.Width = 1;
    }

    private void MakeTicks(List<Tick> ticks)
    {
        plot.Axes.Bottom.SetTicks(ticks.Select(x => x.Position).ToArray(), ticks.Select(x => x.Label).ToArray());
        plot.Axes.Bottom.TickLabelStyle.Rotation = 45;
        plot.Axes.Bottom.TickLabelStyle.Alignment = Alignment.MiddleLeft;
    }

    private void MakeBars()
    {
        var barPlot = plot.Add.Bars(xPositions, yValues);
        barPlot.Color = new Color("777777");

        foreach (var bar in barPlot.Bars)
        {
            bar.LineWidth = 0;
        }

    }

    private void MakeLine()
    {
        var linePlot = plot.Add.ScatterLine(xPositions.ToArray(), yValues.ToArray(), new("aa5599"));
        linePlot.LineWidth = 2;
        linePlot.Smooth = true;
        linePlot.SmoothTension = 1f;
    }

    private static List<Tick> GenerateTicks(List<KeyValuePair<string, int>> data){
        List<Tick> ticks = new List<Tick>();
        for (int i = 0; i < data.Count; i++)
        {
            ticks.Add(new Tick(i, data[i].Key));
        }
        return ticks;
    }
}
