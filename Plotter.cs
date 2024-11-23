using ScottPlot;

namespace WebScraper;

public class Plotter{
    private readonly Plot plot;
    public Plotter(List<KeyValuePair<string, int>> totalData, int amountToShow)
    {
        List<KeyValuePair<string, int>> data = totalData.Take(amountToShow).ToList();

        plot = new Plot();
        List<Tick> ticks = SetUpValuesAndCalcTicks(data);
        ApplyTicksAndStyle(ticks);

        plot.SavePng("res.png", 1920, 1080);

    }

    private void ApplyTicksAndStyle(List<Tick> ticks)
    {
        plot.Axes.Bottom.SetTicks(ticks.Select(x => x.Position).ToArray(), ticks.Select(x => x.Label).ToArray());
        plot.Axes.Bottom.TickLabelStyle.Rotation = 45;
        plot.Axes.Bottom.TickLabelStyle.Alignment = Alignment.MiddleLeft;

        plot.Axes.Bottom.MinimumSize = 100;
        plot.Axes.Right.MinimumSize = 100;
    }

    private List<Tick> SetUpValuesAndCalcTicks(List<KeyValuePair<string, int>> data)
    {
        List<Tick> ticks = new List<Tick>();
        for (int i = 0; i < data.Count; i++)
        {
            KeyValuePair<string, int> item = data[i];
            plot.Add.Scatter(i, item.Value);
            ticks.Add(new Tick(i, item.Key));
        }

        return ticks;
    }
}
