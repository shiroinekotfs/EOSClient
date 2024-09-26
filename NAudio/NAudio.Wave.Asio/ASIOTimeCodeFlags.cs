using System;

namespace NAudio.Wave.Asio;

[Flags]
internal enum ASIOTimeCodeFlags
{
	kTcValid = 1,
	kTcRunning = 2,
	kTcReverse = 4,
	kTcOnspeed = 8,
	kTcStill = 0x10,
	kTcSpeedValid = 0x100
}
