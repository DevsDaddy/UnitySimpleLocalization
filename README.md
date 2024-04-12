# Unity Simple Localization System
![image](https://github.com/DevsDaddy/UnitySimpleLocalization/assets/147835900/ffcff008-27fa-4b3b-ad2d-00c1d8a07811)
**Unity Simple Localization** is a set of free and open source **cross-platform tools** to localize your games or applications powered by Scriptable Objects with Easy-to-Use components and string editor.

**Requirements:**
- **Unity 2019+**;
- **TextMeshPro** for UI Localized Text;

*Future updates will be contain an editor for non-string assets;*

## Get Started
**Unity Simple Localization** has own **Setup Wizzard**, **Locale Creation** and **Editing** tools, Localized Text, Image and Audio Clips Components.

**Installation process:**
- Download and import <a href="https://github.com/DevsDaddy/UnitySimpleLocalization/releases">latest release from this page</a>;
- Run **Setup Wizzard**;
- Edit your Locales;
- See <a href="#usage">usage examples below</a>;

**Configuration Using Setup Wizzard:**<br/>
![image](https://github.com/DevsDaddy/UnitySimpleLocalization/assets/147835900/a71256cc-2543-4c24-8c59-fb4a66aa15e2)
<br/>Using **Setup Wizzard** (Simple Localization -> Setup Wizzard) you can setup your Simple Localization Configuration buy few clicks.
You can select default language, initialization method, fallback string and add available languages.

**Locales Editor:**<br/>
![image](https://github.com/DevsDaddy/UnitySimpleLocalization/assets/147835900/ec54fb31-2a37-4aaf-8404-f19b2424db89)
<br/>Using **Locale Table Editor** (Simple Localization -> Tables -> Edit Locale Table) you can setup your strings for localization.
<br/><br/>**If you need localize Images or Audio Clips - use LocalizedImage component:**<br/>
![image](https://github.com/DevsDaddy/UnitySimpleLocalization/assets/147835900/8f894f65-b346-4e09-b949-e8693c4f1efa)

**Manual Setup:**
You can disable initialization at startup and initialize your instance by code:
```csharp
Localization.Main.Initialize(LocaleDB configuration = null); // Setup Configuration
```

## Usage
**You can simply subscribe to localization switch event and manage your code using current language:**
```csharp
// Event
Localization.Main.OnLanguageSwitched?.AddListener(YOUR_CALLBACK_FOR_EVENT);

// Get Current Language Data
var langData = Localization.Main.GetCurrentLanguage();
```

## Coming Soon (TO-DO)
**I plan to add the following functionality in the near future:**
- Editor for localized Images, Audios and other assets;
- New Table editor for locale tables;

## Join Community
- <a href="https://discord.gg/xuNTKRDebx">Discord Community</a>

## Support Me
**You can support the development and updating of libraries and assemblies by dropping a coin:**
<table>
  <tr><td>Bitcoin (BTC)</td><td>bc1qef2d34r4xkrm48zknjdjt7c0ea92ay9m2a7q55</td></tr>
  <tr><td>Etherium (ETH)</td><td>0x1112a2Ef850711DF4dE9c432376F255f416ef5d0</td></tr>
  <tr><td>Boosty</td><td>https://boosty.to/devsdaddy</td></tr>
</table>
