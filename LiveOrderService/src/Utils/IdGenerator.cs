using System.Security.Cryptography;

namespace LiveOrderService.Utils
{
    public static class IdGenerator
    {
        public static uint GenerateId()
        {
            byte[] buffer = new byte[4];
            RandomNumberGenerator.Fill(buffer);
            return BitConverter.ToUInt32(buffer, 0);
        }
    }
}