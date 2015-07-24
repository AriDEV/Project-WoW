﻿// Copyright (c) Multi-Emu.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Framework.Cryptography.WoW
{
    public class RsaData
    {
        public RSAParameters RsaParams;

        public RsaData(int keySize = 2048)
        {
            RSACryptoServiceProvider.UseMachineKeyStore = false;

            var rsaProvider = new RSACryptoServiceProvider(keySize);

            rsaProvider.PersistKeyInCsp = false;

            // Execution time depends on the keySize
            RsaParams = rsaProvider.ExportParameters(true);

            // Let's use little-endian
            RsaParams.D        = RsaParams.D.Reverse().ToArray();
            RsaParams.DP       = RsaParams.DP.Reverse().ToArray();
            RsaParams.DQ       = RsaParams.DQ.Reverse().ToArray();
            RsaParams.Exponent = RsaParams.Exponent.Reverse().ToArray();
            RsaParams.InverseQ = RsaParams.InverseQ.Reverse().ToArray();
            RsaParams.Modulus  = RsaParams.Modulus.Reverse().ToArray();
            RsaParams.P        = RsaParams.P.Reverse().ToArray();
            RsaParams.Q        = RsaParams.Q.Reverse().ToArray();

            // We just need it for rsa data generation
            rsaProvider = null;
        }

        void WritePublicByteArray(ref StringBuilder sb, string name, byte[] data)
        {
            sb.Append($"    public static byte[] {name} = {{ ");

            for (int i = 0; i < data.Length; i++)
            {
                if (i == data.Length - 1)
                    sb.Append($"0x{data[i] :X2} }};");
                else
                    sb.Append($"0x{data[i]:X2}, ");
            }

            sb.AppendLine();
        }

        public void WriteRSAParamsToFile(string file)
        {
            using (var sw = new StreamWriter(new FileStream(file, FileMode.Append, FileAccess.Write)))
            {
                var sb = new StringBuilder();

                sb.AppendLine("class RsaStore");
                sb.AppendLine("{");

                // Write all private & public rsa parameters.
                WritePublicByteArray(ref sb, "D", RsaParams.D);
                WritePublicByteArray(ref sb, "DP", RsaParams.DP);
                WritePublicByteArray(ref sb, "DQ", RsaParams.DQ);
                WritePublicByteArray(ref sb, "Exponent", RsaParams.Exponent);
                WritePublicByteArray(ref sb, "InverseQ", RsaParams.InverseQ);
                WritePublicByteArray(ref sb, "Modulus", RsaParams.Modulus);
                WritePublicByteArray(ref sb, "P", RsaParams.P);
                WritePublicByteArray(ref sb, "Q", RsaParams.Q);

                sb.AppendLine("}");

                sw.WriteLine(sb.ToString());
            }

            // Reset all values
            RsaParams = new RSAParameters();
        }
    }
}
