using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.Utilities.IntegrationTests
{
    [TestClass]
    public class SimpleTests
    {
        [TestMethod]
        public void ShotTest()
        {
            var bitmap = Screenshoter.Shot();
            var path = $"{Path.GetTempFileName()}.bmp";

            bitmap.Save(path);

            Process.Start(new ProcessStartInfo(path)
            {
                UseShellExecute = true,
            });
        }

        [TestMethod]
        public void ShotEachDisplayTest()
        {
            foreach (var rectangle in Screenshoter.GetPhysicalScreens())
            {
                var bitmap = Screenshoter.Shot(rectangle);
                var path = $"{Path.GetTempFileName()}.bmp";

                bitmap.Save(path);

                Process.Start(new ProcessStartInfo(path)
                {
                    UseShellExecute = true,
                });
            }
        }

        [TestMethod]
        public async Task AsyncShotTest()
        {
            using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            var cancellationToken = cancellationTokenSource.Token;

            var bitmap = await Screenshoter.ShotAsync(cancellationToken: cancellationToken);
            var path = $"{Path.GetTempFileName()}.bmp";

            bitmap.Save(path);

            Process.Start(new ProcessStartInfo(path)
            {
                UseShellExecute = true,
            });
        }

        [TestMethod]
        public async Task AsyncMultiShotTest()
        {
            using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            var cancellationToken = cancellationTokenSource.Token;

            await Task.WhenAll(Enumerable
                .Range(0, 10)
                .Select(_ => Screenshoter.ShotAsync(cancellationToken: cancellationToken)));
        }

        [TestMethod]
        public void RectangleShotTest()
        {
            var bitmap = Screenshoter.Shot(Rectangle.FromLTRB(4509, 808, 5278, 1426));
            var path = $"{Path.GetTempFileName()}.bmp";

            bitmap.Save(path);

            Process.Start(new ProcessStartInfo(path)
            {
                UseShellExecute = true,
            });
        }

        [TestMethod]
        public void InverseRectangleShotTest()
        {
            var bitmap = Screenshoter.Shot(Rectangle.FromLTRB(5278, 1426, 4509, 808));
            var path = $"{Path.GetTempFileName()}.bmp";

            bitmap.Save(path);

            Process.Start(new ProcessStartInfo(path)
            {
                UseShellExecute = true,
            });
        }
    }
}
