using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.RegularExpressions;
using System.Threading;

const string PUSHBULLET_ACCESS_TOKEN = "<YOUR TOKEN HERE>";
const string PUSHBULLET_ENDPOINT = "https://api.pushbullet.com/v2/ephemerals";
Regex FRIEND_CODE_REGEX = new Regex(@"(?:\d[ ]*){12}", RegexOptions.Compiled);
HttpClient client = new HttpClient();
long LastClipboardChangeCount = OsxClipboard.GetChangeCount();

while (true)
{
    if (TryGetNewClipboardContent(out var text) && TryFormatFriendCode(text, out var friendCode))
    {
        Console.WriteLine($"Sending friend code to clipboard: {friendCode}");
        var content = JsonContent.Create(new
        {
            type = "push",
            push = new
            {
                body = friendCode,
                source_device_iden = "ujpah72o0sjAoRtnM0jc",
                source_user_iden = "ujpah72o0",
                type = "clip"
            },
        });
        content.Headers.Add("Access-Token", PUSHBULLET_ACCESS_TOKEN);
        try
        {
            var response = await client.PostAsync(PUSHBULLET_ENDPOINT, content);
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Error ${response.StatusCode}");
                string responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine(responseBody);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Message :{e.Message}");
        }
    }
    Thread.Sleep(250);
}

bool TryGetNewClipboardContent(out string content)
{
    var changeCount = OsxClipboard.GetChangeCount();
    var clibboardUpdated = LastClipboardChangeCount != changeCount;
    LastClipboardChangeCount = changeCount;
    if (clibboardUpdated)
    {
        var text = OsxClipboard.GetText();
        if (!string.IsNullOrWhiteSpace(text))
        {
            content = text;
            return true;
        }
    }
    content = null;
    return false;
}

bool TryFormatFriendCode(string friendCode, out string formattedFriendCode)
{
    var match = FRIEND_CODE_REGEX.Match(friendCode);
    if (!match.Success)
    {
        formattedFriendCode = null;
        return false;
    }
    formattedFriendCode = Regex.Replace(match.Value, @"\s+", string.Empty);
    return true;
}
