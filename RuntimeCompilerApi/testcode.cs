using System.Linq;
using System;
using System.Text;

class Program {
	public static string Main() {
		int i = 4001;
		int j = 5001;
		StringBuilder builder = new StringBuilder();
		builder.Append("Hello world FROM FILE");
		builder.Append("\nFirst variable: " + i);
		builder.Append("\nSecond variable: " + j);
		builder.Append("\nSum: " + (i + j));
		Console.WriteLine("heuehue");
		return builder.ToString();
	}
}