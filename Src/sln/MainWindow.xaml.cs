using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Speech.Synthesis;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace TTS.ForSchool
{
  public partial class MainWindow : Window
  {
    readonly SpeechSynthesizer _synth = new SpeechSynthesizer();
    readonly string _s = @"
25.	Ne regardez pas le livre.
26.	Ouvrez votre livre.
27.	Fermez votre livre.
28.	Attendez.
29.	Encore une fois.
30.	Peut-on utiliser le dictionnaire?
31.	C'est pour quand? 
32.	Vous pouvez m'aider?
33.	Venez ici.
34.	Venez au tableau.
35.	Est-ce qu'il y a un volontaire?
36.	Vous avez 10 minutes pour faire le test.
37.	Travaillez par groupe de deux personnes.
38.	C'est fini.
39.	Faites des groupes de trois ersonnes.
40.	Bonne journée.
41.	À demain. 
42.	Bonnes vacances. 
43.	Joyeux Noël.

1.	Écoutez et répétez les phrases. 
2.	J'ai une question.
3.	Qu'est-ce que ça veut dire?
4.	Qu'est-ce que c'est?
5.	Ça se prononce comment? 
6.	Je ne comprends pas.
7.	Je n'ai pas bien compris.
8.	Vous pouvez expliquer encore une fois, s'ilvous plaît?
9.	Je ne comprends pas ce qu'il faut faire.
10.	Est-ce qu'il y a des devoirs?
11.	Je suis désolé, j'ai oublié mes devoirs à la maison.
12.	Est-ce que c'est possible d'apporter mon devoir demain?
13.	Est-ce que je peux aller aux toilettes?
14.	Excuez-moi, je ne me sens pas bien. 
15.	Je ne sais pas.
16.	J'ai oublié.	
17.	Je serai absent demain. 
18.	Je serai absent la semaine prochaine.
19.	J'étais absent la semaine dernière.
20.	Qui est absent?
";
    string _curPhrase;

    public MainWindow()
    {
      InitializeComponent();
      KeyUp += new KeyEventHandler((s, e) =>
      {
        switch (e.Key)
        {
          case Key.Escape: { Close(); Application.Current.Shutdown(); } break;
        }
      }); //tu:
      MouseLeftButtonDown += new MouseButtonEventHandler((s, e) => DragMove()); //tu:

      Loaded += onLoaded;
    }

    void speakAsyncCurPhrase(string newPhrase = null)
    {
      if (newPhrase != null && tbxCurPhrs != null)
      {
        tbxCurPhrs.Text = _curPhrase = newPhrase;
      }

      _synth.SpeakAsyncCancelAll();
      _synth.SpeakAsync($"{_curPhrase}");
    }

    async void onLoaded(object s, RoutedEventArgs e)
    {
      try
      {
        var lst = _synth.GetInstalledVoices(/*new CultureInfo("fr-CA")*/);
        foreach (var voice in lst) lbv.Items.Add($"{voice.VoiceInfo.Name}");

        _synth.SpeakAsync($"{lst.Count} voices are installed.");
        await Task.Delay(55);

        _synth.SelectVoice(lst.FirstOrDefault().VoiceInfo.Name);
        //_synth.SpeakAsync($"{_s} ");

        lbx.Items.Clear();
        foreach (var item in _s.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries))
        {
          lbx.Items.Add(item);
        }
      }
      catch (Exception ex) { MessageBox.Show(ex.ToString(), "Exception"); }
    }

    void onEn(object s, RoutedEventArgs e) { _synth.SelectVoiceByHints(VoiceGender.NotSet, VoiceAge.NotSet, 0, new CultureInfo("en")); speakAsyncCurPhrase("Hello, everybody!"); }
    void onFr(object s, RoutedEventArgs e) { _synth.SelectVoiceByHints(VoiceGender.NotSet, VoiceAge.NotSet, 0, new CultureInfo("fr")); speakAsyncCurPhrase("Bonjour à tous, je cherche désespérément "); }
    void onRu(object s, RoutedEventArgs e) { _synth.SelectVoiceByHints(VoiceGender.NotSet, VoiceAge.NotSet, 0, new CultureInfo("ru")); speakAsyncCurPhrase("Всем привет!"); }
    void onCh(object s, RoutedEventArgs e) { _synth.SelectVoiceByHints(VoiceGender.NotSet, VoiceAge.NotSet, 0, new CultureInfo("zh")); speakAsyncCurPhrase("Nee How. Nee cheer la ma."); }

    async void onSay1secAllVoices(object s, RoutedEventArgs e)
    {
      _synth.SpeakAsyncCancelAll();
      try
      {
        var lst = _synth.GetInstalledVoices();

        _synth.SpeakAsync($"{lst.Count} voices are installed."); foreach (var voice in lst) Debug.WriteLine(voice.VoiceInfo.Name);

        //_synth.SpeakAsync("Default voice says"); speakSynchCurPhrase(fr);
        //_synth.SelectVoiceByHints(VoiceGender.NotSet, VoiceAge.NotSet, 3, new CultureInfo("fr")); _synth.Speaksync("French says"); _synth.SpeakAsync(fr);
        //_synth.SelectVoiceByHints(VoiceGender.NotSet, VoiceAge.NotSet, 3, new CultureInfo("fr-CA")); _synth.Speaksync("French Canadian says"); _synth.SpeakAsync(fr);
        //_synth.SelectVoiceByHints(VoiceGender.NotSet, VoiceAge.NotSet, 3, new CultureInfo("en")); _synth.Speaksync("English says"); _synth.SpeakAsync(fr);

        foreach (var voice in lst)
        {
          Debug.WriteLine(voice.VoiceInfo.Name);
          try
          {
            _synth.SelectVoice(voice.VoiceInfo.Name);
            _synth.SpeakAsync($"I'm {voice.VoiceInfo.Name}. Bonjour à tous, je cherche désespérément. Привет! Ё пэ рэ сэ тЭ... ");
            await Task.Delay(999);
          }
          catch (Exception ex) { MessageBox.Show(ex.ToString(), voice.VoiceInfo.Name); }
        }
      }
      catch (Exception ex) { MessageBox.Show(ex.ToString(), "Exception"); }
    }
    void onSayAllVoices(object s, RoutedEventArgs e)
    {
      foreach (var voice in _synth.GetInstalledVoices())
      {
        Debug.WriteLine(voice.VoiceInfo.Name);
        try
        {
          _synth.SpeakAsyncCancelAll();
          _synth.SelectVoice(voice.VoiceInfo.Name);
          _synth.SpeakAsync($"I'm {voice.VoiceInfo.Name}. Bonjour à tous, je cherche désespérément. Привет! Как дам в глаз! Путин дурак! Ё пэ рэ сэ тЭ... ");
        }
        catch (Exception ex) { MessageBox.Show(ex.ToString(), voice.VoiceInfo.Name); }
      }
    }
    void onSpeakCurPhrase(object s, RoutedEventArgs e) => speakAsyncCurPhrase(tbxCurPhrs.Text);

    void lbVoice_SelectionChanged(object s, SelectionChangedEventArgs e)
    {
      if (e.AddedItems.Count < 1) return;

      var speaker = e.AddedItems[0].ToString();

      _synth.SpeakAsyncCancelAll();
      _synth.SelectVoice(speaker);
      _synth.SpeakAsync($"I'm {speaker}. Bonjour à tous, je cherche désespérément. Привет! Как дам в глаз! Путин дурак! Ё пэ рэ сэ тЭ... ");
    }
    void lbTask_SelectionChanged(object s, SelectionChangedEventArgs e)
    {
      if (e.AddedItems.Count < 1) return;

      speakAsyncCurPhrase(e.AddedItems[0].ToString());
    }

    void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
      _synth.Rate = (int)((Slider)sender).Value;
      speakAsyncCurPhrase($"{_synth.Rate} ");
    }
  }
}
/*


...You were spot on. 
Using this registry copy I solved it. 
Export the whole Token Directory of 
HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Speech_OneCore\Voices             to a file. Replace every 
HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Speech_OneCore\Voices\Tokens      with 
HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Speech\Voices\Tokens              in the file and run the file (I removed the voices I alread have before). 

    
  
  
  Change CPU to x64 !!!!!!!!. ==> renders only 3 DESKTOP voices in the list, all others are gone. Jan 2019.







  Here is how it looks after renaming at ASUS@Nymi:

  Windows Registry Editor Version 5.00

[HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Speech\Voices]

[HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Speech\Voices\Tokens]

[HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Speech\Voices\Tokens\MSTTS_V110_enAU_CatherineM]
@="Microsoft Catherine - English (Australia)"
"C09"="Microsoft Catherine - English (Australia)"
"CLSID"="{179F3D56-1B0B-42B2-A962-59B7EF59FE1B}"
"LangDataPath"=hex(2):25,00,77,00,69,00,6e,00,64,00,69,00,72,00,25,00,5c,00,53,\
  00,70,00,65,00,65,00,63,00,68,00,5f,00,4f,00,6e,00,65,00,43,00,6f,00,72,00,\
  65,00,5c,00,45,00,6e,00,67,00,69,00,6e,00,65,00,73,00,5c,00,54,00,54,00,53,\
  00,5c,00,65,00,6e,00,2d,00,41,00,55,00,5c,00,4d,00,53,00,54,00,54,00,53,00,\
  4c,00,6f,00,63,00,65,00,6e,00,41,00,55,00,2e,00,64,00,61,00,74,00,00,00
"VoicePath"=hex(2):25,00,77,00,69,00,6e,00,64,00,69,00,72,00,25,00,5c,00,53,00,\
  70,00,65,00,65,00,63,00,68,00,5f,00,4f,00,6e,00,65,00,43,00,6f,00,72,00,65,\
  00,5c,00,45,00,6e,00,67,00,69,00,6e,00,65,00,73,00,5c,00,54,00,54,00,53,00,\
  5c,00,65,00,6e,00,2d,00,41,00,55,00,5c,00,4d,00,33,00,30,00,38,00,31,00,43,\
  00,61,00,74,00,68,00,65,00,72,00,69,00,6e,00,65,00,00,00

[HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Speech\Voices\Tokens\MSTTS_V110_enAU_CatherineM\Attributes]
"Age"="Adult"
"DataVersion"="11.0.2015.0909"
"Gender"="Female"
"Language"="C09"
"Name"="Microsoft Catherine"
"SayAsSupport"="spell=NativeSupported; cardinal=NativeSupported; ordinal=NativeSupported; date=NativeSupported; time=NativeSupported; telephone=NativeSupported; computer=NativeSupported; address=NativeSupported; currency=NativeSupported; message=NativeSupported; media=NativeSupported; url=NativeSupported; alphanumeric=NativeSupported"
"SharedPronunciation"=""
"Vendor"="Microsoft"
"Version"="11.0"

[HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Speech\Voices\Tokens\MSTTS_V110_enAU_JamesM]
@="Microsoft James - English (Australia)"
"C09"="Microsoft James - English (Australia)"
"CLSID"="{179F3D56-1B0B-42B2-A962-59B7EF59FE1B}"
"LangDataPath"=hex(2):25,00,77,00,69,00,6e,00,64,00,69,00,72,00,25,00,5c,00,53,\
  00,70,00,65,00,65,00,63,00,68,00,5f,00,4f,00,6e,00,65,00,43,00,6f,00,72,00,\
  65,00,5c,00,45,00,6e,00,67,00,69,00,6e,00,65,00,73,00,5c,00,54,00,54,00,53,\
  00,5c,00,65,00,6e,00,2d,00,41,00,55,00,5c,00,4d,00,53,00,54,00,54,00,53,00,\
  4c,00,6f,00,63,00,65,00,6e,00,41,00,55,00,2e,00,64,00,61,00,74,00,00,00
"VoicePath"=hex(2):25,00,77,00,69,00,6e,00,64,00,69,00,72,00,25,00,5c,00,53,00,\
  70,00,65,00,65,00,63,00,68,00,5f,00,4f,00,6e,00,65,00,43,00,6f,00,72,00,65,\
  00,5c,00,45,00,6e,00,67,00,69,00,6e,00,65,00,73,00,5c,00,54,00,54,00,53,00,\
  5c,00,65,00,6e,00,2d,00,41,00,55,00,5c,00,4d,00,33,00,30,00,38,00,31,00,4a,\
  00,61,00,6d,00,65,00,73,00,00,00

[HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Speech\Voices\Tokens\MSTTS_V110_enAU_JamesM\Attributes]
"Age"="Adult"
"DataVersion"="11.0.2015.0909"
"Gender"="Male"
"Language"="C09"
"Name"="Microsoft James"
"SayAsSupport"="spell=NativeSupported; cardinal=NativeSupported; ordinal=NativeSupported; date=NativeSupported; time=NativeSupported; telephone=NativeSupported; computer=NativeSupported; address=NativeSupported; currency=NativeSupported; message=NativeSupported; media=NativeSupported; url=NativeSupported; alphanumeric=NativeSupported"
"SharedPronunciation"=""
"Vendor"="Microsoft"
"Version"="11.0"

[HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Speech\Voices\Tokens\MSTTS_V110_enCA_LindaM]
@="Microsoft Linda - English (Canada)"
"1009"="Microsoft Linda - English (Canada)"
"CLSID"="{179F3D56-1B0B-42B2-A962-59B7EF59FE1B}"
"LangDataPath"=hex(2):25,00,77,00,69,00,6e,00,64,00,69,00,72,00,25,00,5c,00,53,\
  00,70,00,65,00,65,00,63,00,68,00,5f,00,4f,00,6e,00,65,00,43,00,6f,00,72,00,\
  65,00,5c,00,45,00,6e,00,67,00,69,00,6e,00,65,00,73,00,5c,00,54,00,54,00,53,\
  00,5c,00,65,00,6e,00,2d,00,43,00,41,00,5c,00,4d,00,53,00,54,00,54,00,53,00,\
  4c,00,6f,00,63,00,65,00,6e,00,43,00,41,00,2e,00,64,00,61,00,74,00,00,00
"VoicePath"=hex(2):25,00,77,00,69,00,6e,00,64,00,69,00,72,00,25,00,5c,00,53,00,\
  70,00,65,00,65,00,63,00,68,00,5f,00,4f,00,6e,00,65,00,43,00,6f,00,72,00,65,\
  00,5c,00,45,00,6e,00,67,00,69,00,6e,00,65,00,73,00,5c,00,54,00,54,00,53,00,\
  5c,00,65,00,6e,00,2d,00,43,00,41,00,5c,00,4d,00,34,00,31,00,30,00,35,00,4c,\
  00,69,00,6e,00,64,00,61,00,00,00

[HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Speech\Voices\Tokens\MSTTS_V110_enCA_LindaM\Attributes]
"Age"="Adult"
"DataVersion"="11.0.2015.0909"
"Gender"="Female"
"Language"="1009"
"Name"="Microsoft Linda"
"SayAsSupport"="spell=NativeSupported; cardinal=NativeSupported; ordinal=NativeSupported; date=NativeSupported; time=NativeSupported; address=NativeSupported; telephone=NativeSupported; computer=NativeSupported; currency=NativeSupported; message=NativeSupported; name=NativeSupported; media=NativeSupported; url=NativeSupported; alphanumeric=NativeSupported"
"SharedPronunciation"=""
"Vendor"="Microsoft"
"Version"="11.0"

[HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Speech\Voices\Tokens\MSTTS_V110_enCA_RichardM]
@="Microsoft Richard - English (Canada)"
"1009"="Microsoft Richard - English (Canada)"
"CLSID"="{179F3D56-1B0B-42B2-A962-59B7EF59FE1B}"
"LangDataPath"=hex(2):25,00,77,00,69,00,6e,00,64,00,69,00,72,00,25,00,5c,00,53,\
  00,70,00,65,00,65,00,63,00,68,00,5f,00,4f,00,6e,00,65,00,43,00,6f,00,72,00,\
  65,00,5c,00,45,00,6e,00,67,00,69,00,6e,00,65,00,73,00,5c,00,54,00,54,00,53,\
  00,5c,00,65,00,6e,00,2d,00,43,00,41,00,5c,00,4d,00,53,00,54,00,54,00,53,00,\
  4c,00,6f,00,63,00,65,00,6e,00,43,00,41,00,2e,00,64,00,61,00,74,00,00,00
"VoicePath"=hex(2):25,00,77,00,69,00,6e,00,64,00,69,00,72,00,25,00,5c,00,53,00,\
  70,00,65,00,65,00,63,00,68,00,5f,00,4f,00,6e,00,65,00,43,00,6f,00,72,00,65,\
  00,5c,00,45,00,6e,00,67,00,69,00,6e,00,65,00,73,00,5c,00,54,00,54,00,53,00,\
  5c,00,65,00,6e,00,2d,00,43,00,41,00,5c,00,4d,00,34,00,31,00,30,00,35,00,52,\
  00,69,00,63,00,68,00,61,00,72,00,64,00,00,00

[HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Speech\Voices\Tokens\MSTTS_V110_enCA_RichardM\Attributes]
"Age"="Adult"
"DataVersion"="11.0.2015.0909"
"Gender"="Male"
"Language"="1009"
"Name"="Microsoft Richard"
"SayAsSupport"="spell=NativeSupported; cardinal=NativeSupported; ordinal=NativeSupported; date=NativeSupported; time=NativeSupported; address=NativeSupported; telephone=NativeSupported; computer=NativeSupported; currency=NativeSupported; message=NativeSupported; name=NativeSupported; media=NativeSupported; url=NativeSupported; alphanumeric=NativeSupported"
"SharedPronunciation"=""
"Vendor"="Microsoft"
"Version"="11.0"

[HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Speech\Voices\Tokens\MSTTS_V110_enGB_GeorgeM]
@="Microsoft George - English (United Kingdom)"
"809"="Microsoft George - English (United Kingdom)"
"CLSID"="{179F3D56-1B0B-42B2-A962-59B7EF59FE1B}"
"LangDataPath"=hex(2):25,00,77,00,69,00,6e,00,64,00,69,00,72,00,25,00,5c,00,53,\
  00,70,00,65,00,65,00,63,00,68,00,5f,00,4f,00,6e,00,65,00,43,00,6f,00,72,00,\
  65,00,5c,00,45,00,6e,00,67,00,69,00,6e,00,65,00,73,00,5c,00,54,00,54,00,53,\
  00,5c,00,65,00,6e,00,2d,00,47,00,42,00,5c,00,4d,00,53,00,54,00,54,00,53,00,\
  4c,00,6f,00,63,00,65,00,6e,00,47,00,42,00,2e,00,64,00,61,00,74,00,00,00
"VoicePath"=hex(2):25,00,77,00,69,00,6e,00,64,00,69,00,72,00,25,00,5c,00,53,00,\
  70,00,65,00,65,00,63,00,68,00,5f,00,4f,00,6e,00,65,00,43,00,6f,00,72,00,65,\
  00,5c,00,45,00,6e,00,67,00,69,00,6e,00,65,00,73,00,5c,00,54,00,54,00,53,00,\
  5c,00,65,00,6e,00,2d,00,47,00,42,00,5c,00,4d,00,32,00,30,00,35,00,37,00,47,\
  00,65,00,6f,00,72,00,67,00,65,00,00,00

[HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Speech\Voices\Tokens\MSTTS_V110_enGB_GeorgeM\Attributes]
"Age"="Adult"
"DataVersion"="11.0.2013.1022"
"Gender"="Male"
"Language"="809"
"Name"="Microsoft George"
"SayAsSupport"="spell=NativeSupported; cardinal=GlobalSupported; ordinal=NativeSupported; date=GlobalSupported; time=GlobalSupported; telephone=NativeSupported; computer=NativeSupported; address=NativeSupported; currency=NativeSupported; message=NativeSupported; media=NativeSupported; url=NativeSupported; alphanumeric=NativeSupported"
"SharedPronunciation"=""
"Vendor"="Microsoft"
"Version"="11.0"

[HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Speech\Voices\Tokens\MSTTS_V110_enGB_HazelM]
@="Microsoft Hazel - English (United Kingdom)"
"809"="Microsoft Hazel - English (United Kingdom)"
"CLSID"="{179F3D56-1B0B-42B2-A962-59B7EF59FE1B}"
"LangDataPath"=hex(2):25,00,77,00,69,00,6e,00,64,00,69,00,72,00,25,00,5c,00,53,\
  00,70,00,65,00,65,00,63,00,68,00,5f,00,4f,00,6e,00,65,00,43,00,6f,00,72,00,\
  65,00,5c,00,45,00,6e,00,67,00,69,00,6e,00,65,00,73,00,5c,00,54,00,54,00,53,\
  00,5c,00,65,00,6e,00,2d,00,47,00,42,00,5c,00,4d,00,53,00,54,00,54,00,53,00,\
  4c,00,6f,00,63,00,65,00,6e,00,47,00,42,00,2e,00,64,00,61,00,74,00,00,00
"VoicePath"=hex(2):25,00,77,00,69,00,6e,00,64,00,69,00,72,00,25,00,5c,00,53,00,\
  70,00,65,00,65,00,63,00,68,00,5f,00,4f,00,6e,00,65,00,43,00,6f,00,72,00,65,\
  00,5c,00,45,00,6e,00,67,00,69,00,6e,00,65,00,73,00,5c,00,54,00,54,00,53,00,\
  5c,00,65,00,6e,00,2d,00,47,00,42,00,5c,00,4d,00,32,00,30,00,35,00,37,00,48,\
  00,61,00,7a,00,65,00,6c,00,00,00

[HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Speech\Voices\Tokens\MSTTS_V110_enGB_HazelM\Attributes]
"Age"="Adult"
"DataVersion"="11.0.2016.0129"
"Gender"="Female"
"Language"="809"
"Name"="Microsoft Hazel"
"SayAsSupport"="spell=NativeSupported; cardinal=GlobalSupported; ordinal=NativeSupported; date=GlobalSupported; time=GlobalSupported; telephone=NativeSupported; computer=NativeSupported; address=NativeSupported; currency=NativeSupported; message=NativeSupported; media=NativeSupported; url=NativeSupported; alphanumeric=NativeSupported"
"SharedPronunciation"=""
"Vendor"="Microsoft"
"Version"="11.0"

[HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Speech\Voices\Tokens\MSTTS_V110_enGB_SusanM]
@="Microsoft Susan - English (United Kingdom)"
"809"="Microsoft Susan - English (United Kingdom)"
"CLSID"="{179F3D56-1B0B-42B2-A962-59B7EF59FE1B}"
"LangDataPath"=hex(2):25,00,77,00,69,00,6e,00,64,00,69,00,72,00,25,00,5c,00,53,\
  00,70,00,65,00,65,00,63,00,68,00,5f,00,4f,00,6e,00,65,00,43,00,6f,00,72,00,\
  65,00,5c,00,45,00,6e,00,67,00,69,00,6e,00,65,00,73,00,5c,00,54,00,54,00,53,\
  00,5c,00,65,00,6e,00,2d,00,47,00,42,00,5c,00,4d,00,53,00,54,00,54,00,53,00,\
  4c,00,6f,00,63,00,65,00,6e,00,47,00,42,00,2e,00,64,00,61,00,74,00,00,00
"VoicePath"=hex(2):25,00,77,00,69,00,6e,00,64,00,69,00,72,00,25,00,5c,00,53,00,\
  70,00,65,00,65,00,63,00,68,00,5f,00,4f,00,6e,00,65,00,43,00,6f,00,72,00,65,\
  00,5c,00,45,00,6e,00,67,00,69,00,6e,00,65,00,73,00,5c,00,54,00,54,00,53,00,\
  5c,00,65,00,6e,00,2d,00,47,00,42,00,5c,00,4d,00,32,00,30,00,35,00,37,00,53,\
  00,75,00,73,00,61,00,6e,00,00,00

[HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Speech\Voices\Tokens\MSTTS_V110_enGB_SusanM\Attributes]
"Age"="Adult"
"DataVersion"="11.0.2013.1022"
"Gender"="Female"
"Language"="809"
"Name"="Microsoft Susan"
"SayAsSupport"="spell=NativeSupported; cardinal=NativeSupported; ordinal=NativeSupported; date=NativeSupported; time=NativeSupported; address=NativeSupported; telephone=NativeSupported; computer=NativeSupported; currency=NativeSupported; message=NativeSupported; name=NativeSupported; media=NativeSupported; url=NativeSupported; alphanumeric=NativeSupported"
"SharedPronunciation"=""
"Vendor"="Microsoft"
"Version"="11.0"

[HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Speech\Voices\Tokens\MSTTS_V110_enUS_DavidM]
@="Microsoft David - English (United States)"
"409"="Microsoft David - English (United States)"
"CLSID"="{179F3D56-1B0B-42B2-A962-59B7EF59FE1B}"
"LangDataPath"=hex(2):25,00,77,00,69,00,6e,00,64,00,69,00,72,00,25,00,5c,00,53,\
  00,70,00,65,00,65,00,63,00,68,00,5f,00,4f,00,6e,00,65,00,43,00,6f,00,72,00,\
  65,00,5c,00,45,00,6e,00,67,00,69,00,6e,00,65,00,73,00,5c,00,54,00,54,00,53,\
  00,5c,00,65,00,6e,00,2d,00,55,00,53,00,5c,00,4d,00,53,00,54,00,54,00,53,00,\
  4c,00,6f,00,63,00,65,00,6e,00,55,00,53,00,2e,00,64,00,61,00,74,00,00,00
"VoicePath"=hex(2):25,00,77,00,69,00,6e,00,64,00,69,00,72,00,25,00,5c,00,53,00,\
  70,00,65,00,65,00,63,00,68,00,5f,00,4f,00,6e,00,65,00,43,00,6f,00,72,00,65,\
  00,5c,00,45,00,6e,00,67,00,69,00,6e,00,65,00,73,00,5c,00,54,00,54,00,53,00,\
  5c,00,65,00,6e,00,2d,00,55,00,53,00,5c,00,4d,00,31,00,30,00,33,00,33,00,44,\
  00,61,00,76,00,69,00,64,00,00,00

[HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Speech\Voices\Tokens\MSTTS_V110_enUS_DavidM\Attributes]
"Age"="Adult"
"DataVersion"="11.0.2016.0129"
"Gender"="Male"
"Language"="409"
"Name"="Microsoft David"
"SayAsSupport"="spell=NativeSupported; cardinal=GlobalSupported; ordinal=NativeSupported; date=GlobalSupported; time=GlobalSupported; telephone=NativeSupported; currency=NativeSupported; net=NativeSupported; url=NativeSupported; address=NativeSupported; alphanumeric=NativeSupported; Name=NativeSupported; media=NativeSupported; message=NativeSupported; companyName=NativeSupported; computer=NativeSupported; math=NativeSupported; duration=NativeSupported"
"SharedPronunciation"=""
"Vendor"="Microsoft"
"Version"="11.0"

[HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Speech\Voices\Tokens\MSTTS_V110_enUS_MarkM]
@="Microsoft Mark - English (United States)"
"409"="Microsoft Mark - English (United States)"
"CLSID"="{179F3D56-1B0B-42B2-A962-59B7EF59FE1B}"
"LangDataPath"=hex(2):25,00,77,00,69,00,6e,00,64,00,69,00,72,00,25,00,5c,00,53,\
  00,70,00,65,00,65,00,63,00,68,00,5f,00,4f,00,6e,00,65,00,43,00,6f,00,72,00,\
  65,00,5c,00,45,00,6e,00,67,00,69,00,6e,00,65,00,73,00,5c,00,54,00,54,00,53,\
  00,5c,00,65,00,6e,00,2d,00,55,00,53,00,5c,00,4d,00,53,00,54,00,54,00,53,00,\
  4c,00,6f,00,63,00,65,00,6e,00,55,00,53,00,2e,00,64,00,61,00,74,00,00,00
"VoicePath"=hex(2):25,00,77,00,69,00,6e,00,64,00,69,00,72,00,25,00,5c,00,53,00,\
  70,00,65,00,65,00,63,00,68,00,5f,00,4f,00,6e,00,65,00,43,00,6f,00,72,00,65,\
  00,5c,00,45,00,6e,00,67,00,69,00,6e,00,65,00,73,00,5c,00,54,00,54,00,53,00,\
  5c,00,65,00,6e,00,2d,00,55,00,53,00,5c,00,4d,00,31,00,30,00,33,00,33,00,4d,\
  00,61,00,72,00,6b,00,00,00

[HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Speech\Voices\Tokens\MSTTS_V110_enUS_MarkM\Attributes]
"Age"="Adult"
"DataVersion"="11.0.2013.1022"
"Gender"="Male"
"Language"="409"
"Name"="Microsoft Mark"
"SayAsSupport"="spell=NativeSupported; cardinal=GlobalSupported; ordinal=NativeSupported; date=GlobalSupported; time=GlobalSupported; telephone=NativeSupported; currency=NativeSupported; net=NativeSupported; url=NativeSupported; address=NativeSupported; alphanumeric=NativeSupported; Name=NativeSupported; media=NativeSupported; message=NativeSupported; companyName=NativeSupported; computer=NativeSupported; math=NativeSupported; duration=NativeSupported"
"SharedPronunciation"=""
"Vendor"="Microsoft"
"Version"="11.0"

[HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Speech\Voices\Tokens\MSTTS_V110_enUS_ZiraM]
@="Microsoft Zira - English (United States)"
"409"="Microsoft Zira - English (United States)"
"CLSID"="{179F3D56-1B0B-42B2-A962-59B7EF59FE1B}"
"LangDataPath"=hex(2):25,00,77,00,69,00,6e,00,64,00,69,00,72,00,25,00,5c,00,53,\
  00,70,00,65,00,65,00,63,00,68,00,5f,00,4f,00,6e,00,65,00,43,00,6f,00,72,00,\
  65,00,5c,00,45,00,6e,00,67,00,69,00,6e,00,65,00,73,00,5c,00,54,00,54,00,53,\
  00,5c,00,65,00,6e,00,2d,00,55,00,53,00,5c,00,4d,00,53,00,54,00,54,00,53,00,\
  4c,00,6f,00,63,00,65,00,6e,00,55,00,53,00,2e,00,64,00,61,00,74,00,00,00
"VoicePath"=hex(2):25,00,77,00,69,00,6e,00,64,00,69,00,72,00,25,00,5c,00,53,00,\
  70,00,65,00,65,00,63,00,68,00,5f,00,4f,00,6e,00,65,00,43,00,6f,00,72,00,65,\
  00,5c,00,45,00,6e,00,67,00,69,00,6e,00,65,00,73,00,5c,00,54,00,54,00,53,00,\
  5c,00,65,00,6e,00,2d,00,55,00,53,00,5c,00,4d,00,31,00,30,00,33,00,33,00,5a,\
  00,69,00,72,00,61,00,00,00

[HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Speech\Voices\Tokens\MSTTS_V110_enUS_ZiraM\Attributes]
"Age"="Adult"
"DataVersion"="11.0.2013.1022"
"Gender"="Female"
"Language"="409"
"Name"="Microsoft Zira"
"SayAsSupport"="spell=NativeSupported; cardinal=GlobalSupported; ordinal=NativeSupported; date=GlobalSupported; time=GlobalSupported; telephone=NativeSupported; currency=NativeSupported; net=NativeSupported; url=NativeSupported; address=NativeSupported; alphanumeric=NativeSupported; Name=NativeSupported; media=NativeSupported; message=NativeSupported; companyName=NativeSupported; computer=NativeSupported; math=NativeSupported; duration=NativeSupported"
"SharedPronunciation"=""
"Vendor"="Microsoft"
"Version"="11.0"



*/
