# Publicera webbplatsen och koppla en egen domän

CodeBrief-sajten ligger i `docs/` och är gjord för **GitHub Pages** (Deploy from a branch → `/docs`).

Det finns **ingen CNAME-fil** i projektet. Lägg inte till en förrän du har köpt en riktig domän.

## GitHub Pages-adressen (gratis)

När repot ligger på GitHub blir sajten typ:

`https://<användarnamn>.github.io/<repo>/`

Exempel: användare `anna`, repo `CodeBrief` → `https://anna.github.io/CodeBrief/`

Om repot heter `<användarnamn>.github.io` hamnar sajten på roten: `https://<användarnamn>.github.io/`

### Slå på Pages

1. Pusha `main` till GitHub.
2. Repo → **Settings** → **Pages**.
3. **Build and deployment** → Source: **Deploy from a branch**.
4. Branch: `main`, folder: `/docs`.
5. Save. Vänta en minut. Öppna den URL GitHub visar.

Ladda ner-knappen pekar mot **GitHub Releases**. Publicera en release med `CodeBrief-Setup-1.0.0.exe` så fungerar nedladdningen.

## Egen domän (valfritt, du köper den själv)

Köp hos t.ex. [Loopia](https://www.loopia.se/), [Namecheap](https://www.namecheap.com/) eller [Cloudflare](https://www.cloudflare.com/). Hitta inte på ett namn här — välj ett som är ledigt.

Två vanliga mönster:

### A) `www.dindoman.se` (CNAME) — enklast

Hos DNS-leverantören, skapa:

| Typ | Namn | Värde |
|---|---|---|
| CNAME | `www` | `<användarnamn>.github.io` |

I GitHub: Settings → Pages → **Custom domain** → `www.dindoman.se` → Save. Kryssa i **Enforce HTTPS** när certifikatet är klart (kan ta upp till 24 h, ofta snabbare).

GitHub skapar då en `CNAME`-fil i `docs/`. Låt den följa med i git så den inte försvinner vid nästa push.

### B) Apex / naken domän (`dindoman.se`) — A-poster

GitHub dokumenterar aktuella IP-adresser här: [Managing a custom domain for GitHub Pages](https://docs.github.com/pages/configuring-a-custom-domain-for-your-github-pages-site).

Typiskt (kontrollera listan mot GitHub, den kan ändras):

| Typ | Namn | Värde |
|---|---|---|
| A | `@` | `185.199.108.153` |
| A | `@` | `185.199.109.153` |
| A | `@` | `185.199.110.153` |
| A | `@` | `185.199.111.153` |
| AAAA | `@` | `2606:50c0:8000::153` |
| AAAA | `@` | `2606:50c0:8001::153` |
| AAAA | `@` | `2606:50c0:8002::153` |
| AAAA | `@` | `2606:50c0:8003::153` |

Samma Custom domain-fält i Pages, t.ex. `dindoman.se`. Många sätter både apex och `www`, och redirectar den ena till den andra.

## Checklista

- [ ] Repo på GitHub, Pages på `/docs`
- [ ] Fungerar på `https://<user>.github.io/<repo>/`
- [ ] Domän köpt (om du vill ha en)
- [ ] DNS CNAME eller A/AAAA enligt ovan
- [ ] Custom domain ifylld i Pages
- [ ] HTTPS på
- [ ] `docs/CNAME` incheckad om GitHub skapade den
