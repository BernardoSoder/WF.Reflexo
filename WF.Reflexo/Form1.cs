using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace WF.Reflexo
{
    public partial class Form1 : Form
    {
        private Button btncIniciar;
        private Button btnAlvo;
        private System.Windows.Forms.Timer timer;
        private Random random;
        private Stopwatch stopwatch;
        private List<string> history = new List<string>();
        private Label lblHistory;
        private Label lblCorAtual;
        private Color corCorreta;
        private int tempoMaximoClique = 500;
        private bool primeiroClique = false;
        private int tempoPrimeiroClique;
        private int currentAlvoSize = 100;

        public Form1()
        {
            InitializeComponent();
            this.Text = "Reflexo";
            this.Size = new Size(600, 600);
            this.StartPosition = FormStartPosition.CenterScreen;

            btncIniciar = new Button()
            {
                Text = "Iniciar",
                Size = new Size(150, 60),
                Location = new Point(220, 500),
                Font = new Font("Arial", 14, FontStyle.Bold)
            };
            btncIniciar.Click += IniciarJogo;

            lblHistory = new Label()
            {
                Visible = true,
                Size = new Size(300, 400),
                Location = new Point(10, 80),
                Font = new Font("Arial", 12, FontStyle.Regular),
                ForeColor = Color.Blue,
                Text = "Histórico:"
            };

            lblCorAtual = new Label()
            {
                Visible = true,
                Size = new Size(300, 40),
                Location = new Point(10, 20),
                Font = new Font("Arial", 14, FontStyle.Bold),
                Text = "Cor correta: "
            };

            this.Controls.Add(btncIniciar);
            this.Controls.Add(lblHistory);
            this.Controls.Add(lblCorAtual);

            btnAlvo = new Button()
            {
                Size = new Size(currentAlvoSize, currentAlvoSize),
                Visible = false,
                Font = new Font("Arial", 10, FontStyle.Bold)
            };
            btnAlvo.MouseClick += btnAlvoClick;
            this.Controls.Add(btnAlvo);

            timer = new System.Windows.Forms.Timer();
            timer.Tick += MostrarBotaoAlvo;

            random = new Random();
            stopwatch = new Stopwatch();
        }

        private void IniciarJogo(object sender, EventArgs e)
        {
            btncIniciar.Enabled = false;
            IniciarNovaRodada();
        }

        private void IniciarNovaRodada()
        {
            btnAlvo.Size = new Size(currentAlvoSize, currentAlvoSize);
            timer.Interval = random.Next(1000, 3000);
            timer.Start();
        }

        private void MostrarBotaoAlvo(object sender, EventArgs e)
        {
            timer.Stop();
            int x = random.Next(10, this.ClientSize.Width - btnAlvo.Width - 10);
            int y = random.Next(80, this.ClientSize.Height - btnAlvo.Height - 10);
            btnAlvo.Location = new Point(x, y);
            corCorreta = GerarCorAleatoria();
            btnAlvo.BackColor = corCorreta;
            lblCorAtual.Text = "Cor correta: " + corCorreta.Name;
            lblCorAtual.ForeColor = corCorreta;
            btnAlvo.Visible = true;
            btnAlvo.BringToFront();
            stopwatch.Restart();
            primeiroClique = false;
        }

        private void btnAlvoClick(object sender, MouseEventArgs e)
        {
            if (btnAlvo.BackColor.ToArgb() != corCorreta.ToArgb())
            {
                MessageBox.Show("Clique errado! Espere a cor correta.");
                return;
            }
            if (!primeiroClique)
            {
                primeiroClique = true;
                tempoPrimeiroClique = (int)stopwatch.ElapsedMilliseconds;
            }
            else
            {
                int tempoEntreCliques = (int)stopwatch.ElapsedMilliseconds - tempoPrimeiroClique;
                if (tempoEntreCliques > tempoMaximoClique)
                {
                    MessageBox.Show("Muito lento! Tente novamente.");
                    primeiroClique = false;
                }
                else
                {
                    stopwatch.Stop();
                    int reactionTime = (int)stopwatch.ElapsedMilliseconds;
                    history.Add(reactionTime + "ms");
                    lblHistory.Text = "Histórico:\n" + string.Join("\n", history) + "\nMédia: " + MediaHistorico();
                    MessageBox.Show("Tempo de reação: " + reactionTime + "ms");
                    if (currentAlvoSize > 20)
                    {
                        currentAlvoSize -= 5;
                    }
                    btnAlvo.Visible = false;
                    Task.Delay(500).ContinueWith(_ => IniciarNovaRodada(), TaskScheduler.FromCurrentSynchronizationContext());
                }
            }
        }

        private string MediaHistorico()
        {
            int count = 0;
            double soma = 0;
            for (int i = history.Count - 1; i >= 0 && count < 5; i--)
            {
                string s = history[i].Replace("ms", "");
                if (double.TryParse(s, out double time))
                {
                    soma += time;
                    count++;
                }
            }
            double media = (count > 0) ? soma / count : 0;
            return media.ToString("F2") + "ms";
        }

        private Color GerarCorAleatoria()
        {
            Color[] cores = { Color.Red, Color.Green, Color.Blue, Color.Yellow };
            return cores[random.Next(cores.Length)];
        }
    }
}
