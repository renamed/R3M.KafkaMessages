using System;
using System.Text;

namespace WritRead.Extensions
{
    public static class TimeSpanExtensions
    {
        public static string GetReadable(this TimeSpan tempoEnvio)
        {
            StringBuilder sb = new StringBuilder();

            if (tempoEnvio.TotalMinutes > 10)
                sb.Append("há mais de 10 minutos");
            else if (tempoEnvio.TotalMinutes < 1)
            {
                if (tempoEnvio.Seconds == 0)
                    sb.Append("agora mesmo");
                else
                {
                    sb.Append($"há {tempoEnvio.Seconds} segundo");
                    if (tempoEnvio.Seconds > 1)
                        sb.Append("s");
                }
            }
            else
            {
                sb.Append($"há {tempoEnvio.Minutes} minuto");
                if (tempoEnvio.Minutes > 1)
                    sb.Append("s");

                if (tempoEnvio.Seconds > 0)
                {
                    sb.Append($" e {tempoEnvio.Seconds} segundo");
                    if (tempoEnvio.Seconds > 1)
                        sb.Append("s");
                }
            }

            return sb.ToString();
        }
    }
}