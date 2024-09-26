using System;

namespace IRemote;

[Serializable]
public class ServerInfo
{
	private string _ip;

	private int _port;

	private string _serverAlias;

	private string _version;

	private string _monitor_IP;

	private int _monitor_port;

	private string _ip_range_wlan;

	private string _public_ip;

	public string IP
	{
		get
		{
			return _ip;
		}
		set
		{
			_ip = value;
		}
	}

	public int Port
	{
		get
		{
			return _port;
		}
		set
		{
			_port = value;
		}
	}

	public string ServerAlias
	{
		get
		{
			return _serverAlias;
		}
		set
		{
			_serverAlias = value;
		}
	}

	public string Version
	{
		get
		{
			return _version;
		}
		set
		{
			_version = value;
		}
	}

	public string MonitorServer_IP
	{
		get
		{
			return _monitor_IP;
		}
		set
		{
			_monitor_IP = value;
		}
	}

	public int MonitorServer_Port
	{
		get
		{
			return _monitor_port;
		}
		set
		{
			_monitor_port = value;
		}
	}

	public string IP_Range_WLAN
	{
		get
		{
			return _ip_range_wlan;
		}
		set
		{
			_ip_range_wlan = value;
		}
	}

	public string Public_IP
	{
		get
		{
			return _public_ip;
		}
		set
		{
			_public_ip = value;
		}
	}

	public ServerInfo()
	{
	}

	public ServerInfo(string ip, int port, string serverAlias, string version, string ip_range_wlan)
	{
		_ip = ip;
		_port = port;
		_serverAlias = serverAlias;
		_version = version;
		_ip_range_wlan = ip_range_wlan;
	}
}
