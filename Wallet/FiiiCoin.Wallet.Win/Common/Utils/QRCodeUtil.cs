using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiiiCoin.Wallet.Win.Common
{
    public class QRCodeUtil : InstanceBase<QRCodeUtil>
    {
        public string GenerateQRCodes(string strCode)
        {
            string path = Path.GetTempFileName();

            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(strCode, QRCodeGenerator.ECCLevel.Q);

            QRCode qrCode = new QRCode(qrCodeData);
            Image image = qrCode.GetGraphic(10);
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                image.Save(fs, ImageFormat.Png);
                fs.Close();
            }
            return path;
        }
    }
}
