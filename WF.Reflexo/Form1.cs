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

        public Form1()
        {
            InitializeComponent();
            this.Text = "Reflexo";
            this.Size = new Size(400, 400);
            this.StartPosition = FormStartPosition.CenterScreen;

            btncIniciar = new Button()
            {
                Text = "Iniciar",
                Size = new Size(100, 50),
                Location = new Point(150, 300)
            };
            btncIniciar.Click += IniciarJogo;

            lblHistory = new Label()
            {
                Visible = true,
                Size = new Size(200, 300),
                Location = new Point(10, 50),
                Text = "Histórico:"
            };

            lblCorAtual = new Label()
            {
                Visible = true,
                Size = new Size(200, 30),
                Location = new Point(10, 10),
                Font = new Font("Arial", 12, FontStyle.Bold),
                Text = "Cor correta: "
            };

            this.Controls.Add(btncIniciar);
            this.Controls.Add(lblHistory);
            this.Controls.Add(lblCorAtual);

            btnAlvo = new Button()
            {
                Size = new Size(50, 50),
                Visible = false
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
            timer.Interval = random.Next(1000, 3000);
            timer.Start();
        }

        private void MostrarBotaoAlvo(object sender, EventArgs e)
        {
            timer.Stop();
            int x = random.Next(10, this.ClientSize.Width - btnAlvo.Width - 10);
            int y = random.Next(50, this.ClientSize.Height - btnAlvo.Height - 10);
            btnAlvo.Location = new Point(x, y);
            corCorreta = GerarCorAleatoria();
            btnAlvo.BackColor = corCorreta;
            lblCorAtual.Text = $"Cor correta: {corCorreta.Name}";
            lblCorAtual.ForeColor = corCorreta;
            btnAlvo.Visible = true;
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
                    history.Add($"{stopwatch.ElapsedMilliseconds}ms");
                    lblHistory.Text = "Histórico:\n" + string.Join("\n", history);
                    MessageBox.Show($"Tempo de reação: {stopwatch.ElapsedMilliseconds}ms");
                    btnAlvo.Visible = false;
                    Task.Delay(500).ContinueWith(_ => IniciarNovaRodada(), TaskScheduler.FromCurrentSynchronizationContext());
                }
            }
        }

        private Color GerarCorAleatoria()
        {
            Color[] cores = { Color.Red, Color.Green, Color.Blue, Color.Yellow };
            return cores[random.Next(cores.Length)];
        }
    }
}
