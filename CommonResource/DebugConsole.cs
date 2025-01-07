using System;
using System.IO;
using System.Runtime.InteropServices;

public class DebugConsole
{
    [DllImport("kernel32.dll")]
    private static extern bool AllocConsole();

    [DllImport("kernel32.dll")]
    private static extern bool FreeConsole();
    public DebugConsole()
    {

    }

    public void Open()
    {
        /* 参考ページ：C#(Windows Formアプリケーション)でコンソールの表示、非表示、出力方法(Console.WriteLine())
         * https://github.com/murasuke/AllocConsoleCSharp
         */

        // Console表示
        AllocConsole();
        // コンソールとstdoutの紐づけを行う。無くても初回は出力できるが、表示、非表示を繰り返すとエラーになる。
        Console.SetOut(new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true });
        //コンソールの文字エンコードを指定。これがないとBVE本体からの情報が文字化けする。
        Console.OutputEncoding = System.Text.Encoding.GetEncoding("utf-8");

    }

    public void Close()
    {
        FreeConsole();
    }
}