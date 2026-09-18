(function () {
  "use strict";

  // Fyll i när GitHub-repot finns. Tomma värden → auto från github.io, annars placeholder.
  var GITHUB = { owner: "", repo: "" };

  var strings = {
    sv: {
      skip: "Hoppa till innehållet",
      "brand.tag": "v1.0 · Windows",
      "nav.how": "Så funkar det",
      "nav.features": "Funktioner",
      "nav.pricing": "Pris",
      "nav.faq": "FAQ",
      "nav.download": "Ladda ner",
      "nav.menu": "Meny",
      "spec.1": "Windows 10/11 x64",
      "spec.2": "20 gratis analyser",
      "spec.3": "Lokal motor",
      "spec.4": "Valfri AI-nyckel",
      "spec.5": "CB1-licens",
      "spec.6": "PDF-export",
      "hero.eyebrow": "För studenter och nybörjare · Windows 10/11",
      "hero.title.1": "Förstå koden.",
      "hero.title.2": "Hitta buggarna.",
      "hero.lede":
        "CodeBrief är en skrivbordsapp som tar en kodsnutt, gissar språket och skriver en pedagogisk rapport på svenska: översikt, pseudokod, flöde, buggar/säkerhet och ett förslag på rättning.",
      "hero.cta": "Ladda ner till Windows",
      "hero.secondary": "Se hur det funkar",
      "hero.kbd": "analyserar i appen",
      "hero.p1": "20 analyser gratis, sedan Pro-licens",
      "hero.p2": "Lokal analys — koden stannar på datorn",
      "hero.p3": "Valfri AI med din egen nyckel",
      "app.subtitle": "Kodanalys och pedagogiska rapporter",
      "app.trial": "Provperiod · 20 analyser",
      "mock.license": "Licens",
      "mock.settings": "Inställningar",
      "mock.open": "Öppna",
      "mock.md": "Markdown",
      "mock.pdf": "PDF",
      "mock.analyze": "Analysera",
      "mock.source": "Källkod",
      "mock.report": "Rapport",
      "mock.lines": "5 rader · 98 tecken",
      "mock.complexity": "Låg komplexitet",
      "mock.status": "Klar · lokal motor",
      "mock.remain": "Provperiod · 19 analyser kvar",
      "mock.lang": "01 · Språk och miljö",
      "mock.overview": "Funktionen summerar en lista och räknar medelvärde. Komplexitet: låg.",
      "mock.bugs": "04 · Buggar och säkerhet",
      "mock.bugTitle": "Division med noll",
      "mock.bugDetail": "Tom lista ger ZeroDivisionError. Kontrollera längden innan division.",
      "mock.fix": "06 · Korrigerad kod",
      "mock.fixDetail": "Förslag: returnera 0 eller kasta ett tydligt fel när listan är tom.",
      "how.kicker": "Tre steg",
      "how.title": "Så funkar det",
      "how.lede":
        "Ingen webbtjänst att logga in på. Du installerar appen, klistrar in kod och läser rapporten till höger.",
      "how.1.title": "Klistra in eller öppna",
      "how.1.body":
        "Klistra in källkod, släpp en fil eller öppna med Ctrl+O. CodeBrief gissar språket (C#, Python, Java, JavaScript/TypeScript, C/C++, Go, Rust med flera).",
      "how.2.title": "Analysera",
      "how.2.body":
        "Tryck Analysera (Ctrl+Enter). Den lokala motorn kör alltid. Har du en egen OpenAI- eller Anthropic-nyckel kan du välja den i Inställningar.",
      "how.3.title": "Läs rapporten",
      "how.3.body":
        "Sex sektioner: översikt, pseudokod, flöde, buggar/säkerhet, förbättringar och rättad kod. Exportera till Markdown eller PDF.",
      "rail.1": "Översikt",
      "rail.2": "Pseudokod",
      "rail.3": "Flöde",
      "rail.4": "Buggar",
      "rail.5": "Förbättringar",
      "rail.6": "Rättad kod",
      "feat.kicker": "Vad du får",
      "feat.title": "En rapport som hjälper dig lära",
      "feat.lede": "Samma sektioner som i appen — byggda för att förklara kod, inte bara flagga fel.",
      "feat.1.title": "Språk och översikt",
      "feat.1.body":
        "Offline-detektion av språk och ramverk, plus en kort sammanfattning av vad koden gör och hur komplex den är.",
      "feat.2.title": "Pseudokod",
      "feat.2.body": "Koden skrivs om i enklare steg så du kan följa logiken utan att fastna i syntax.",
      "feat.3.title": "Flöde",
      "feat.3.body": "Indata, steg och utdata — så du ser vad som kommer in, vad som händer och vad som kommer ut.",
      "feat.4.title": "Buggar och säkerhet",
      "feat.4.body": "Heuristik som letar efter vanliga fel, kantfall och säkerhetsrisker, med allvarlighetsgrad.",
      "feat.5.title": "Förslag på rättning",
      "feat.5.body":
        "Förklaring plus korrigerad kod när analysen tycker att något behöver ändras. Bra att jämföra med din lösning.",
      "feat.6.title": "Lokal analys",
      "feat.6.body": "Utan API-nyckel lämnar koden aldrig din dator. Fungerar offline. Syntaxfärgning i editorn ingår.",
      "feat.7.title": "Valfri AI",
      "feat.7.body":
        "Lägg in din egen OpenAI- eller Anthropic-nyckel. Den lagras krypterad med Windows DPAPI och skickas bara till den provider du valt — inte till någon CodeBrief-server.",
      "feat.8.title": "PDF och Markdown",
      "feat.8.body":
        "Exportera rapporten till skolan, labben eller din egen anteckning. Markdown med Ctrl+S, PDF med Ctrl+Shift+P.",
      "feat.langs.label": "Språkdetektion",
      "feat.langs.body":
        "C#, Python, C/C++, Java, JavaScript, TypeScript, Go, Rust, PHP, Ruby, SQL, HTML, CSS, Kotlin, Swift, PowerShell, Bash.",
      "privacy.kicker": "Integritet",
      "privacy.title": "Din kod skickas inte till oss",
      "privacy.body":
        "CodeBrief har ingen molnanalys av sig själv. Den lokala motorn kör på din PC. Väljer du AI går koden till OpenAI eller Anthropic med din nyckel. CodeBrief har ingen egen server som tar emot kod eller nycklar.",
      "privacy.l1": "Lokal heuristik fungerar utan internet",
      "privacy.l2": "API-nyckel krypteras för ditt Windows-konto",
      "privacy.l3": "Ingen spårning på den här sidan",
      "price.kicker": "Pris",
      "price.title": "Prova först. Licens när du behöver mer.",
      "price.lede":
        "Ingen webbutik i appen. Du laddar ner, testar 20 analyser och aktiverar en Pro-nyckel under Licens när provperioden är slut.",
      "price.trial.name": "Provperiod",
      "price.trial.pill": "Gratis",
      "price.trial.cost": "20 analyser",
      "price.trial.body": "Gratis i appen. Varje körning räknas. Lokal analys och export ingår. Inget konto.",
      "price.trial.l1": "Alla sex rapportsektioner",
      "price.trial.l2": "Markdown- och PDF-export",
      "price.trial.l3": "Valfri egen AI-nyckel",
      "price.trial.cta": "Börja med att ladda ner",
      "price.pro.name": "Pro-licens",
      "price.pro.pill": "I appen",
      "price.pro.cost": "Nyckel i appen",
      "price.pro.body":
        "RSA-signerad nyckel som börjar med CB1. Klistra in den under Licens. Gäller tills utgångsdatumet i nyckeln.",
      "price.pro.l1": "Analyser utan 20-gränsen",
      "price.pro.l2": "Samma funktioner som i trial",
      "price.pro.l3": "Ingen Stripe-checkout i appen",
      "price.pro.cta": "Hur får jag en nyckel?",
      "dl.kicker": "Version 1.0.0",
      "dl.title": "Ladda ner CodeBrief för Windows",
      "dl.body":
        "Installer: CodeBrief-Setup-1.0.0.exe (64-bitars, Inno Setup). Kräver Windows 10 eller 11 x64. Ingen separat .NET-installation behövs för den publicerade .exe-filen.",
      "dl.meta": "Windows 10/11 x64 · Inno Setup · self-contained",
      "dl.cta": "Hämta från GitHub Releases",
      "dl.source": "Källkod på GitHub",
      "dl.note":
        "När en release är publicerad pekar knappen dit. Tills dess: bygg med scripts/publish.ps1 eller kör från källkod.",
      "faq.kicker": "Vanliga frågor",
      "faq.title": "För studenter",
      "faq.1.q": "Skickas min kod någonstans?",
      "faq.1.a":
        "Inte med standardläget. Lokal heuristik körs på din dator. Om du klistrar in en OpenAI- eller Anthropic-nyckel under Inställningar skickas koden till den providern du valt. CodeBrief har ingen egen molnserver för analys.",
      "faq.2.q": "Vad händer efter 20 analyser?",
      "faq.2.a":
        "Provperioden tar slut. Appen ber dig aktivera en licensnyckel under Licens. Nyckeln börjar med CB1. och är RSA-signerad. Det finns ingen inbyggd kortbetalning.",
      "faq.3.q": "Finns det till Mac, Linux eller webben?",
      "faq.3.a":
        "Nej. CodeBrief 1.0 är en Windows-app (WPF) för Windows 10/11 x64. Den här sidan är bara information — analysen körs inte i webbläsaren.",
      "faq.4.q": "Behöver jag internet?",
      "faq.4.a": "Nej för lokal analys och språkdetektion. Ja om du använder OpenAI eller Anthropic.",
      "faq.5.q": "Hur får jag en Pro-licens?",
      "faq.5.a":
        "Utgivaren skapar en CB1.-nyckel (tools/CodeBrief.LicenseGen). Du klistrar in den i appen. Kontakta den som ger ut CodeBrief — det finns ingen automatisk webbshop i version 1.0.",
      "faq.6.q": "Kan jag lämna in rapporten i skolan?",
      "faq.6.a":
        "Du kan exportera till PDF eller Markdown. Använd det som hjälp att förstå koden. Följ din skolas regler för hjälpmedel och plagiering.",
      "faq.7.q": "Vilken licens har källkoden?",
      "faq.7.a": "MIT. QuestPDF i PDF-exporten används under Community License.",
      "footer.tag": "Windows-app för att förstå kod och hitta fel. Version 1.0.0.",
      "footer.github": "GitHub",
      "footer.license": "LICENSE (MIT)",
      "footer.domain": "Egen domän",
      "footer.download": "Ladda ner",
      "footer.copy": "© 2026 CodeBrief. Inget påhittat kassasystem — licens aktiveras i appen."
    },
    en: {
      skip: "Skip to content",
      "brand.tag": "v1.0 · Windows",
      "nav.how": "How it works",
      "nav.features": "Features",
      "nav.pricing": "Pricing",
      "nav.faq": "FAQ",
      "nav.download": "Download",
      "nav.menu": "Menu",
      "spec.1": "Windows 10/11 x64",
      "spec.2": "20 free analyses",
      "spec.3": "Local engine",
      "spec.4": "Optional AI key",
      "spec.5": "CB1 license",
      "spec.6": "PDF export",
      "hero.eyebrow": "For students and learners · Windows 10/11",
      "hero.title.1": "Understand the code.",
      "hero.title.2": "Find the bugs.",
      "hero.lede":
        "CodeBrief is a desktop app that takes a snippet, detects the language, and writes a teaching report: overview, pseudocode, flow, bugs/security, and a suggested fix. The app UI is Swedish.",
      "hero.cta": "Download for Windows",
      "hero.secondary": "See how it works",
      "hero.kbd": "runs analysis in the app",
      "hero.p1": "20 analyses free, then a Pro license",
      "hero.p2": "Local analysis — code stays on your PC",
      "hero.p3": "Optional AI with your own key",
      "app.subtitle": "Code analysis and teaching reports",
      "app.trial": "Trial · 20 analyses",
      "mock.license": "License",
      "mock.settings": "Settings",
      "mock.open": "Open",
      "mock.md": "Markdown",
      "mock.pdf": "PDF",
      "mock.analyze": "Analyze",
      "mock.source": "Source",
      "mock.report": "Report",
      "mock.lines": "5 lines · 98 chars",
      "mock.complexity": "Low complexity",
      "mock.status": "Ready · local engine",
      "mock.remain": "Trial · 19 analyses left",
      "mock.lang": "01 · Language and context",
      "mock.overview": "The function sums a list and computes the average. Complexity: low.",
      "mock.bugs": "04 · Bugs and security",
      "mock.bugTitle": "Division by zero",
      "mock.bugDetail": "An empty list raises ZeroDivisionError. Check the length before dividing.",
      "mock.fix": "06 · Corrected code",
      "mock.fixDetail": "Suggestion: return 0 or raise a clear error when the list is empty.",
      "how.kicker": "Three steps",
      "how.title": "How it works",
      "how.lede": "There is no web login. Install the app, paste code, and read the report on the right.",
      "how.1.title": "Paste or open",
      "how.1.body":
        "Paste source, drop a file, or open with Ctrl+O. CodeBrief guesses the language (C#, Python, Java, JavaScript/TypeScript, C/C++, Go, Rust, and more).",
      "how.2.title": "Analyze",
      "how.2.body":
        "Press Analyze (Ctrl+Enter). The local engine always works. If you have your own OpenAI or Anthropic key, choose it in Settings.",
      "how.3.title": "Read the report",
      "how.3.body":
        "Six sections: overview, pseudocode, flow, bugs/security, improvements, and corrected code. Export to Markdown or PDF.",
      "rail.1": "Overview",
      "rail.2": "Pseudocode",
      "rail.3": "Flow",
      "rail.4": "Bugs",
      "rail.5": "Improvements",
      "rail.6": "Corrected code",
      "feat.kicker": "What you get",
      "feat.title": "A report that helps you learn",
      "feat.lede": "The same sections as in the app — built to explain code, not only flag errors.",
      "feat.1.title": "Language and overview",
      "feat.1.body": "Offline detection of language and frameworks, plus a short summary and complexity.",
      "feat.2.title": "Pseudocode",
      "feat.2.body": "The code is rewritten as simpler steps so you can follow the logic without the syntax.",
      "feat.3.title": "Flow",
      "feat.3.body": "Inputs, steps, and outputs — what comes in, what happens, and what comes out.",
      "feat.4.title": "Bugs and security",
      "feat.4.body": "Heuristics for common bugs, edge cases, and security issues, with severity.",
      "feat.5.title": "Suggested fix",
      "feat.5.body": "An explanation plus corrected code when the analyzer thinks something should change.",
      "feat.6.title": "Local analysis",
      "feat.6.body": "Without an API key, code never leaves your computer. Works offline. Syntax highlighting included.",
      "feat.7.title": "Optional AI",
      "feat.7.body":
        "Paste your own OpenAI or Anthropic key. It is stored encrypted with Windows DPAPI and sent only to the provider you chose — not to a CodeBrief server.",
      "feat.8.title": "PDF and Markdown",
      "feat.8.body": "Export the report for class, a lab, or your notes. Markdown with Ctrl+S, PDF with Ctrl+Shift+P.",
      "feat.langs.label": "Language detection",
      "feat.langs.body":
        "C#, Python, C/C++, Java, JavaScript, TypeScript, Go, Rust, PHP, Ruby, SQL, HTML, CSS, Kotlin, Swift, PowerShell, Bash.",
      "privacy.kicker": "Privacy",
      "privacy.title": "Your code is not sent to us",
      "privacy.body":
        "CodeBrief has no cloud analysis of its own. The local engine runs on your PC. If you choose AI, the code goes to OpenAI or Anthropic with your key. CodeBrief has no server that receives code or keys.",
      "privacy.l1": "Local heuristics work without internet",
      "privacy.l2": "API keys are encrypted for your Windows account",
      "privacy.l3": "No tracking on this page",
      "price.kicker": "Pricing",
      "price.title": "Try first. License when you need more.",
      "price.lede":
        "There is no in-app store. Download, use 20 analyses, then activate a Pro key under License when the trial ends.",
      "price.trial.name": "Trial",
      "price.trial.pill": "Free",
      "price.trial.cost": "20 analyses",
      "price.trial.body": "Free in the app. Each run counts. Local analysis and export included. No account.",
      "price.trial.l1": "All six report sections",
      "price.trial.l2": "Markdown and PDF export",
      "price.trial.l3": "Optional AI with your own key",
      "price.trial.cta": "Start by downloading",
      "price.pro.name": "Pro license",
      "price.pro.pill": "In the app",
      "price.pro.cost": "Key in the app",
      "price.pro.body":
        "An RSA-signed key starting with CB1. Paste it under License. Valid until the expiry date in the key.",
      "price.pro.l1": "Analyses without the 20-run cap",
      "price.pro.l2": "The same features as the trial",
      "price.pro.l3": "No Stripe checkout in the app",
      "price.pro.cta": "How do I get a key?",
      "dl.kicker": "Version 1.0.0",
      "dl.title": "Download CodeBrief for Windows",
      "dl.body":
        "Installer: CodeBrief-Setup-1.0.0.exe (64-bit, Inno Setup). Requires Windows 10 or 11 x64. The published .exe is self-contained — no separate .NET install.",
      "dl.meta": "Windows 10/11 x64 · Inno Setup · self-contained",
      "dl.cta": "Get it from GitHub Releases",
      "dl.source": "Source on GitHub",
      "dl.note":
        "The button points at Releases once one exists. Until then: build with scripts/publish.ps1 or run from source.",
      "faq.kicker": "FAQ",
      "faq.title": "For students",
      "faq.1.q": "Does my code leave this machine?",
      "faq.1.a":
        "Not in the default mode. Local heuristics run on your PC. If you paste an OpenAI or Anthropic key in Settings, the code is sent to that provider. CodeBrief has no analysis cloud of its own.",
      "faq.2.q": "What happens after 20 analyses?",
      "faq.2.a":
        "The trial ends. The app asks you to activate a license key under License. Keys start with CB1. and are RSA-signed. There is no built-in card payment.",
      "faq.3.q": "Is there Mac, Linux, or a web app?",
      "faq.3.a":
        "No. CodeBrief 1.0 is a Windows app (WPF) for Windows 10/11 x64. This site is information only — analysis does not run in the browser.",
      "faq.4.q": "Do I need internet?",
      "faq.4.a": "No for local analysis and language detection. Yes if you use OpenAI or Anthropic.",
      "faq.5.q": "How do I get a Pro license?",
      "faq.5.a":
        "The publisher issues a CB1. key (tools/CodeBrief.LicenseGen). You paste it in the app. Contact whoever distributes CodeBrief — version 1.0 has no automatic web shop.",
      "faq.6.q": "Can I hand the report in at school?",
      "faq.6.a":
        "You can export PDF or Markdown. Use it to understand the code. Follow your school’s rules on tools and plagiarism.",
      "faq.7.q": "What license is the source under?",
      "faq.7.a": "MIT. QuestPDF in the PDF export is used under the Community License.",
      "footer.tag": "A Windows app to understand code and find issues. Version 1.0.0.",
      "footer.github": "GitHub",
      "footer.license": "LICENSE (MIT)",
      "footer.domain": "Custom domain",
      "footer.download": "Download",
      "footer.copy": "© 2026 CodeBrief. No fake checkout — licenses are activated in the app."
    }
  };

  var titles = {
    sv: "CodeBrief — Förstå kod och hitta buggar",
    en: "CodeBrief — Understand code and find bugs"
  };

  function githubUrls() {
    if (GITHUB.owner && GITHUB.repo) {
      var base = "https://github.com/" + GITHUB.owner + "/" + GITHUB.repo;
      return { repo: base, releases: base + "/releases/latest", license: base + "/blob/main/LICENSE" };
    }

    var host = location.hostname;
    if (host.endsWith(".github.io")) {
      var user = host.slice(0, -".github.io".length);
      var parts = location.pathname.split("/").filter(Boolean);
      var repo = parts[0] || user + ".github.io";
      var auto = "https://github.com/" + user + "/" + repo;
      return { repo: auto, releases: auto + "/releases/latest", license: auto + "/blob/main/LICENSE" };
    }

    return {
      repo: "https://github.com/OWNER/REPO",
      releases: "https://github.com/OWNER/REPO/releases/latest",
      license: "https://github.com/OWNER/REPO/blob/main/LICENSE"
    };
  }

  function applyLang(lang) {
    var dict = strings[lang] || strings.sv;
    document.documentElement.lang = lang;
    document.title = titles[lang] || titles.sv;
    document.querySelectorAll("[data-i18n]").forEach(function (el) {
      var key = el.getAttribute("data-i18n");
      if (dict[key]) el.textContent = dict[key];
    });
    var sv = document.getElementById("lang-sv");
    var en = document.getElementById("lang-en");
    if (sv) sv.setAttribute("aria-pressed", lang === "sv" ? "true" : "false");
    if (en) en.setAttribute("aria-pressed", lang === "en" ? "true" : "false");
    try {
      localStorage.setItem("codebrief-lang", lang);
    } catch (e) {}
  }

  function wireNav() {
    var btn = document.getElementById("menu-btn");
    var nav = document.getElementById("nav");
    if (!btn || !nav) return;

    function close() {
      nav.classList.remove("open");
      btn.setAttribute("aria-expanded", "false");
    }

    btn.addEventListener("click", function () {
      var open = nav.classList.toggle("open");
      btn.setAttribute("aria-expanded", open ? "true" : "false");
    });

    nav.querySelectorAll("a").forEach(function (link) {
      link.addEventListener("click", close);
    });

    document.addEventListener("keydown", function (event) {
      if (event.key === "Escape") close();
    });
  }

  function wireLang() {
    var sv = document.getElementById("lang-sv");
    var en = document.getElementById("lang-en");
    if (sv) sv.addEventListener("click", function () { applyLang("sv"); });
    if (en) en.addEventListener("click", function () { applyLang("en"); });
  }

  function wireGithub() {
    var urls = githubUrls();
    var release = document.getElementById("release-link");
    var source = document.getElementById("source-link");
    var footerGh = document.getElementById("footer-github");
    var footerLic = document.getElementById("footer-license");
    var cta = document.getElementById("cta-download");
    if (release) release.href = urls.releases;
    if (source) source.href = urls.repo;
    if (footerGh) footerGh.href = urls.repo;
    if (footerLic) footerLic.href = urls.license;
    if (cta && urls.releases.indexOf("OWNER/REPO") === -1) cta.href = urls.releases;
  }

  var stored = "sv";
  try {
    stored = localStorage.getItem("codebrief-lang") || "sv";
  } catch (e) {}
  applyLang(stored === "en" ? "en" : "sv");
  wireNav();
  wireLang();
  wireGithub();
})();
