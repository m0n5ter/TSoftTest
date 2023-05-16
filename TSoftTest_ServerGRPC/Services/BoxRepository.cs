using Google.Protobuf.WellKnownTypes;
using TSoftTest.Shared;
using TSoftTest_Shared;

namespace TSoftTest_ServerGRPC.Services;

public class BoxRepository
{
    private readonly object _boxIndexSync = new();
    private int _boxIndex;
    private int _currentId;
    private List<Task> _workers;

    public BoxRepository()
    {
        _boxIndex = 0;
        var rng = Random.Shared;

        Boxes = Enumerable.Range(0, Constants.BoxCount).Select(_ =>
        {
            var w = rng.NextDouble() * Constants.FieldWidth * 0.05 + Constants.FieldWidth * 0.01;
            var h = rng.NextDouble() * Constants.FieldHeight * 0.05 + Constants.FieldWidth * 0.01;
            var speed = rng.NextDouble() * Constants.FieldWidth * 0.1 + Constants.FieldWidth * 0.01;

            return new BoxClass
            {
                Id = Interlocked.Increment(ref _currentId),
                Width = w,
                Height = h,
                X = rng.NextDouble() * (Constants.FieldWidth - w),
                Y = rng.NextDouble() * (Constants.FieldHeight - h),
                SpeedX = speed * (rng.NextDouble() > 0.5 ? 1 : -1),
                SpeedY = speed * (rng.NextDouble() > 0.5 ? 1 : -1),
                Color = (uint) ((0xFF << 24) | (rng.Next(100, 255) << 16) | (rng.Next(100, 255) << 8) | rng.Next(100, 255)),
                LastMoveTime = DateTime.UtcNow.Ticks
            };
        }).ToList();

        _workers = Enumerable.Range(0, Math.Min(Constants.BoxCount, Constants.WorkerCount)).Select(i => WorkerLoop()).ToList();
    }

    public List<BoxClass> Boxes { get; }

    private Task WorkerLoop()
    {
        return Task.Run(async () =>
        {
            while (true)
            {
                BoxClass box;

                lock (_boxIndexSync)
                {
                    box = Boxes[_boxIndex++];
                    if (_boxIndex >= Constants.BoxCount) _boxIndex = 0;
                }

                Move(box);
                await Task.Delay(0);
            }
        });
    }

    private static void Move(BoxClass box)
    {
        var now = DateTime.UtcNow;
        var secondsSinceLastMove = (now - new DateTime(box.LastMoveTime)).TotalSeconds;

        CalculateCoordinate(() => box.X, d => box.X = d, () => box.SpeedX, d => box.SpeedX = d, box.Width, Constants.FieldWidth, secondsSinceLastMove);
        CalculateCoordinate(() => box.Y, d => box.Y = d, () => box.SpeedY, d => box.SpeedY = d, box.Height, Constants.FieldHeight, secondsSinceLastMove);

        box.LastMoveTime = now.Ticks;
    }

    private static void CalculateCoordinate(Func<double> getPosition, Action<double> setPosition, Func<double> getSpeed, Action<double> setSpeed, double boxSize, double fieldSize, double seconds)
    {
        var offset = seconds * getSpeed();

        while (true)
        {
            setPosition(getPosition() + offset);

            if (offset > 0)
            {
                if (getPosition() + boxSize < fieldSize) break;

                offset = fieldSize - getPosition() - boxSize;
                setSpeed(-getSpeed());
            }
            else
            {
                if (getPosition() >= 0) break;
                offset = -getPosition();
                setSpeed(-getSpeed());
            }
        }
    }
}