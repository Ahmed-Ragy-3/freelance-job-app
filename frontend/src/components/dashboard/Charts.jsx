import { Line, Bar, Doughnut } from "react-chartjs-2";
import {
  Chart as ChartJS, CategoryScale, LinearScale, PointElement, LineElement,
  BarElement, ArcElement, Tooltip, Legend, Filler,
} from "chart.js";

ChartJS.register(CategoryScale, LinearScale, PointElement, LineElement, BarElement, ArcElement, Tooltip, Legend, Filler);

const baseOpts = {
  responsive: true,
  maintainAspectRatio: false,
  plugins: { legend: { display: false } },
  scales: { x: { grid: { display: false } }, y: { grid: { color: "rgba(120,120,120,0.1)" } } },
};

export function LineChart({ labels, data, label = "Value", color = "#10b981" }) {
  return (
    <div className="h-56">
      <Line
        options={baseOpts}
        data={{
          labels,
          datasets: [{ label, data, borderColor: color, backgroundColor: color + "33", fill: true, tension: 0.4, pointRadius: 3 }],
        }}
      />
    </div>
  );
}
export function BarChart({ labels, data, label = "Value", color = "#10b981" }) {
  return (
    <div className="h-56">
      <Bar options={baseOpts} data={{ labels, datasets: [{ label, data, backgroundColor: color, borderRadius: 6 }] }} />
    </div>
  );
}
export function PieChart({ data }) {
  return (
    <div className="h-56">
      <Doughnut
        options={{ responsive: true, maintainAspectRatio: false, plugins: { legend: { position: "bottom" } }, cutout: "65%" }}
        data={{
          labels: data.map((d) => d.name),
          datasets: [{ data: data.map((d) => d.value), backgroundColor: ["#10b981","#3b82f6","#f59e0b","#ef4444","#8b5cf6","#ec4899"], borderWidth: 0 }],
        }}
      />
    </div>
  );
}
