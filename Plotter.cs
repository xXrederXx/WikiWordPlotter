using ScottPlot;

namespace WebScraper;

public class Plotter
{

    /// <summary>
    /// The plot used
    /// </summary>
    private readonly Plot plot;

    /// <summary>
    /// This is esentaly a list from 0 to the number of elements plotet
    /// </summary>
    private readonly List<double> xPositions;

    /// <summary>
    /// These are all the yValues
    /// </summary>
    private readonly List<double> yValues;

    // settings
    /// <summary>
    /// Turn this off or on to show or dont show the line
    /// </summary>
    private const bool showLine = true;

    /// <summary>
    /// Decides if it renders in darkmode
    /// </summary>
    private const bool darkMode = true;

    /// <summary>
    /// This class will plot the given data to a res.png file
    /// </summary>
    /// <param name="totalData">All the data, it is a list of key - value pairs, the keys are the words and the values are the times it got counted</param>
    /// <param name="amountToShow">This is the number of Word, which are shown on the graph</param>
    public Plotter(List<KeyValuePair<string, int>> totalData, int amountToShow, string imagePath)
    {
        List<KeyValuePair<string, int>> data = totalData.Take(amountToShow).ToList();

        xPositions = Enumerable.Range(0, data.Count).Select(x => (double)x).ToList();
        plot = new Plot();
        yValues = data.Select(x => (double)x.Value).ToList();

        MakeBars();

        if (showLine)
        {
            MakeLine();
        }

        List<Tick> ticks = GenerateTicks(data);
        MakeTicks(ticks);

        ApplyStyle();

        plot.SavePng(imagePath, 1920, 1080);

    }

    /// <summary>
    /// This applys the style of the plot
    /// </summary>
    private void ApplyStyle()
    {
        if (darkMode)
        {
            ApplyDarkModeStyle();
        }

        plot.Axes.Bottom.MinimumSize = 100;
        plot.Axes.Right.MinimumSize = 100;

        plot.Axes.Margins(bottom: 0);
    }

    /// <summary>
    /// This is used to apply a dark mode based style
    /// </summary>
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

    /// <summary>
    /// Makes all the given Ticks appeare on the Graph
    /// <summary>
    /// <param name="ticks">The ticks to draw</param>
    private void MakeTicks(List<Tick> ticks)
    {
        plot.Axes.Bottom.SetTicks(ticks.Select(x => x.Position).ToArray(), ticks.Select(x => x.Label).ToArray());
        plot.Axes.Bottom.TickLabelStyle.Rotation = 45;
        plot.Axes.Bottom.TickLabelStyle.Alignment = Alignment.MiddleLeft;
    }

    /// <summary>
    /// This function draws the Bars on to the plot
    /// </summary>
    private void MakeBars()
    {
        ScottPlot.Plottables.BarPlot barPlot = plot.Add.Bars(xPositions, yValues);
        barPlot.Color = new Color("777777");

        foreach (Bar bar in barPlot.Bars)
        {
            bar.LineWidth = 0;
        }

    }

    /// <summary>
    /// This can be used to draw a line on the plot
    /// </summary>
    private void MakeLine()
    {
        ScottPlot.Plottables.Scatter linePlot = plot.Add.ScatterLine(xPositions.ToArray(), yValues.ToArray(), new("aa5599"));
        linePlot.LineWidth = 2;
        linePlot.Smooth = true;
        linePlot.SmoothTension = 1f;
    }

    /// <summary>
    /// This function calculates all the Ticks
    /// </summary>
    /// <param name="data">The data used to generate the Ticks</param>
    /// <returns>A list of the calculated ticks</returns>
    private static List<Tick> GenerateTicks(List<KeyValuePair<string, int>> data)
    {
        List<Tick> ticks = new List<Tick>();
        for (int i = 0; i < data.Count; i++)
        {
            ticks.Add(new Tick(i, data[i].Key));
        }
        return ticks;
    }
}
