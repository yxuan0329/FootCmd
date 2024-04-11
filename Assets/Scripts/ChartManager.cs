using UnityEngine;
using System.Collections.Generic;
using XCharts.Runtime;

public class ChartManager : MonoBehaviour
{
    private LineChart chart;

    public List<float> chartData, chartData2;
    private int chartDataCount = 25;
    private int pointer = 0;
    void Start() {
        var chart = gameObject.GetComponent<LineChart>();
        if (chart == null)
        {
            chart = gameObject.AddComponent<LineChart>();
            chart.Init();
            chart.SetSize(580, 300);
            chart.GetOrAddChartComponent<Title>().show = true;
            chart.GetOrAddChartComponent<Title>().text = "Data";

            chart.GetOrAddChartComponent<Tooltip>().show = true;
            chart.GetOrAddChartComponent<Legend>().show = false;
        }
        for(int i= 0; i<chartDataCount; i++)
        {
            chartData.Add(10f);
            chartData2.Add(600f);
        }
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)){ AddData(Random.Range(10, 900), 600); } // for debug
    }

    public void AddData(float data, float threshold)
    {
        // Debug.Log(data);
        chartData[pointer]=data;
        chartData2[pointer]=threshold;
        pointer = (pointer+1) % chartDataCount;
        chart = gameObject.GetComponent<LineChart>();
        
        var xAxis = chart.GetOrAddChartComponent<XAxis>();
        var yAxis = chart.GetOrAddChartComponent<YAxis>();
        xAxis.show = true;
        yAxis.show = true;
        xAxis.type = Axis.AxisType.Time;
        xAxis.axisLabel.show = false;
        yAxis.type = Axis.AxisType.Value;

        xAxis.splitNumber = 10;
        xAxis.boundaryGap = true;

        chart.RemoveData();
        var serie1 = chart.AddSerie<Line>("Serie 1"); 
        serie1.AnimationEnable(false);
        serie1.symbol.size = 1;
        var serie2 = chart.AddSerie<Line>("Serie 2"); 
        serie2.AnimationEnable(false);
        serie2.symbol.size = 1;

        for (int i = 0; i < chartDataCount; i++)
        {
            //chart.AddXAxisData("x" + i); // X-axis --> timestamp
            chart.AddData(0, chartData[pointer]); // Y-data
            chart.AddData(1, chartData2[pointer]); // Y-data
            pointer = (pointer+1) % chartDataCount;
        }
    }
}