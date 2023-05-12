﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2019, 2020, 2021, 2022, 2023  Dirk Stolle

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using updater.data;
using updater.versions;

namespace updater.software
{
    /// <summary>
    /// Firefox Developer Edition (i.e. aurora channel)
    /// </summary>
    public class FirefoxAurora : NoPreUpdateProcessSoftware
    {
        /// <summary>
        /// NLog.Logger for FirefoxAurora class
        /// </summary>
        private static readonly NLog.Logger logger = NLog.LogManager.GetLogger(typeof(FirefoxAurora).FullName);


        /// <summary>
        /// publisher name for signed executables of Firefox ESR
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2024, 6, 19, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// the currently known newest version
        /// </summary>
        private const string currentVersion = "114.0b3";

        /// <summary>
        /// constructor with language code
        /// </summary>
        /// <param name="langCode">the language code for the Firefox Developer Edition software,
        /// e.g. "de" for German,  "en-GB" for British English, "fr" for French, etc.</param>
        /// <param name="autoGetNewer">whether to automatically get
        /// newer information about the software when calling the info() method</param>
        public FirefoxAurora(string langCode, bool autoGetNewer)
            : base(autoGetNewer)
        {
            if (string.IsNullOrWhiteSpace(langCode))
            {
                logger.Error("The language code must not be null, empty or whitespace!");
                throw new ArgumentNullException(nameof(langCode), "The language code must not be null, empty or whitespace!");
            }
            languageCode = langCode.Trim();
            var validCodes = validLanguageCodes();
            if (!validCodes.Contains(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
            checksum32Bit = knownChecksums32Bit()[langCode];
            checksum64Bit = knownChecksums64Bit()[langCode];
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/114.0b3/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "c8ae733496d7532a6b511556284c42196c8768f022c2b5be2a817c2f7a21745aadb162dde4130713c3b03e0c89cdcd532b52e0901a8ee91187e5bc25dfc128cf" },
                { "af", "553e77271567743a7b82cff8926682dec0033adfb493ade5f0e1666ff80f9f8865f36e07bdd4a579630c704fb1530dd6a6498f8160ea27eb656f200b3a167656" },
                { "an", "2110fd8a2bb2bb908ed3a6558fb99fa44ea565c27c0fc43241a9144b62628808dfa008d46a06910e1fd7767655e81b3b42f2730b405623e9ab6e6907d45c1abf" },
                { "ar", "b74c660b44abfe314838cce15c76fbe2ff0d79088a1fa17d974967439bf4e5056bca7489fe70a56fb04ac4ab473d9f624580ebfedfe4fa71dece467008f97419" },
                { "ast", "1ee6a7c2463d6920bb7836a5ef4f371449b1f0fc266409f7cab826bc4a947ee482d79fc27ab711a3121a31b1d0953e7a1d3c6cd0174b38d51ca359d01dcf7cd2" },
                { "az", "178bda381b271350aaa8bb2b004ceb7db6a83227cf9e011e27bfd6a78e49ef62babdcdeb59b726f337b8a864420b5fcc3da441a19d79eafc176630ca56a88a54" },
                { "be", "edd910236e94c43395123672456b48fca0419b6808d3e435acf267aaa1f6e0aee117fe72e88495e7470c863a0fcaa817dbe460012ef45852f946f6b1220e7fc7" },
                { "bg", "3bfde92fd25c7b4d8e4b48030885d050586d0e3afb39db77396ac526d92889aabbf09be23f941a176aa3867d2cd8a29800e66d21983005c85f757354ba3db59c" },
                { "bn", "baafe5502f0afa6526acc6ab5ec1ff6a09611cef134378928ad432ab7c18776ef6369a03abb464c85e0ed066b16aab56daff24782a65099ed8de12116c03ba26" },
                { "br", "6c3691c5bfcfa0052ec0fcf1de90a0e280af19cc1fcd3840d0df3ed57198087f28412ccd2f51c372edcb5af9686ce5dd77c729fbd0dae35e04ce7a527c69df63" },
                { "bs", "1aa50304b0c97ac4c0953d715a2ec82ac72060e59680895dcd30cab90228441e46c5b203a9553fbb92c6e977b5fe82d30652c8c24f114ec18f00b68eef22b662" },
                { "ca", "53f200aa6f8e254bec15cfd11c65e7c76f534f5306cfc2be67f1e04a005cd35f2b2ab10ffe8660aaccc40bd78cbc27d1a9087a0c6bc1e050974b23bde0fa07b6" },
                { "cak", "6634c1110374d3b11b0bff3e12e23be348b8be1b44f7938e75097f7e073e0b3575f3fb10e60a86936cd08f8a061bf28bec63f02f3a9bc4cee70694a90d7660ad" },
                { "cs", "73ccd28f5ebf50c74a9a8b5cbaa2378cfe811ce5949926c09e1c361d09426aadde8b5bae2facf6ee243b4d1b859800d272fe03d9387bf24bddd3014b242e1909" },
                { "cy", "2d6bdc0c2127517ccfbd7d1fe83aac87545d4b5e91cea053b5d94ae614aa7b5484bbceaf81e02a1e15407e6bbc25d9f9d8a28df663f5b6e5ec7dc7828eeeef4e" },
                { "da", "7a0f35e73656b2e837c38a924145694e8b00ceed0c37c5730545d86b1d0a2979a0f803088d5002da9aae8c4cf1a92828bb0011d89e6070c4a5c4ffd1c1983421" },
                { "de", "bd34acd38c4cf210f49bee08cd66d42a03d70088cb4624cbcf3ae6d24cd90ceaf23f49a58f5d5c829d2cd0083b6cbacb9aa1be0267610f244914919ca3e507ca" },
                { "dsb", "21e2861d211d415befb467bc78ac3d82f5f900550699acabfb193219f976170fdb8a412e70886d3aca09102797da7d3c94de49cfbe7b9ae02d360e0ad741c2b7" },
                { "el", "f6377a9c526d0602c97b9e0275c506a8e8584ead04e0a8c6f2407be637cd8fb68f59c08b717618beff77fa005804e8b9e30e427fc81dd122f9652976fb463111" },
                { "en-CA", "52ab9b4a63348a7aa84f9fc67268fdfccae01fac88fbcc77f8da529fec346d01dc79b094ade8eb3ccc0a12e22263897cc0352dd5a3438e8494e44a52c21621fe" },
                { "en-GB", "7fb12897ee7d05e1bf3c432ed5a1fa13169c11735d861f2591e46529338a479c1b803a9e53a1a97cdc5199428e3f45dfbea77befe8d562d7ba682e0d5be06ce0" },
                { "en-US", "8845ebc9838aae505165699e159ca788a3e415958c61fd58fd51a61b67eb118712fdc01f95b508a8fa8838808ba53e80d6c85c62875079893ca252d84375065a" },
                { "eo", "8ac7a0237b01d8c5f42257d9ac7d2d09ea2492c6a673dfdb7f4a769f9db17f64871549d3170a9262a938da982f186084ce6e0c471dc75b2b8e385790fa033e13" },
                { "es-AR", "2802f597769138a6fa94be5b613524c628dff6962b24381d9571d97d539d0127610cda6b2f5a922d12f0dd41adaf78ffe99aa55dc64b4c21655231e4b30c3452" },
                { "es-CL", "1aa60b2f55629e7961b2d686f5ba0639b4db6aa32462e056f4b9129de333808fda53ed1aeb70c416f55951877cdaf776cd7f49e0fa0db064875c82b0e814af50" },
                { "es-ES", "6bbd9c936164136c776995987f203bbc280a5fec426934a05d74c07fd04226095a41021e865041b5841449e201ee708a20f07ef682af74b57b25df8c9d7565c6" },
                { "es-MX", "3e3e05cc3e1c240cbd5aa773c4745816b54975eb63358b28d7913950138a2d13e463332389fa481519a82dbbb547115ec13554b768c25979c88470193c66ac70" },
                { "et", "1938ee097b795e56e5f1217efc62749d8d3d95528ded39986214ecaedd3937806ba6aca7daacf5f9b533024c301b6b849c50b38ee3ca18114d309c55db229e0f" },
                { "eu", "9ec00f4e1f41bea546dde334c0a872308a3c9b40a41c609be9db70f967ef78cc5b6b0b11f39ea2a0e412cb8b95e7a4d00e9cfa9d6289ebe790172219e2d9ca35" },
                { "fa", "15723b287abc93b90c280c9a60694fe7baeb7c3eb27f571037b1682df6e33e6042dfe8437bb3165a2965ba811eef0d271b77883e22daab7e8da4e1f7ca21e9c3" },
                { "ff", "bc9753bbd8da41e78f8e18c9e30075175d54a941928d3ed4d08a8b9da65cfda4e959a7905ee254d6e2a4592a85cb2a2fc00d0acba9875c61a050059af8a97463" },
                { "fi", "b16da015a054800d8c5488d8e5621ff0ab7d3fd164b4db024749d524e761e1c098c58e547179cd3507fed062cd376540f668a902e57f5339f8a822ae568ac411" },
                { "fr", "a8988237197c13ac047f05677d4ddd546dcfbe79cc249a3cfb7e9c4adb985e6144bb9a8729a5823b95164b88132d01dc1795471111adb4538c6670596e6c34cf" },
                { "fur", "8f2642af8e78c56a6db5f30389b118e01d393c26dbcaa001c144b7ecb2d743ac694c4eb4b28c406244892c8362266e718f8072415e72cffb0a3055d3f2a612f1" },
                { "fy-NL", "1366ae2ce4a1de188e9d291bc767cb255edd43f9f12674e8dcd78f01b5bcdf3bdb39399c1fdce99874995e6f48edbd6abbdc704546eeb5c1e783281369bab84a" },
                { "ga-IE", "e989037c419447e662e1cf566617c5338a6b30b7023e364269646deaa98a3454d46ae3eec240bfdc16deecf98601f1e1845dd14b88ce8a98762adfb9f3fe58e4" },
                { "gd", "5164077ef5bc85a40a4a9ac84027eeacc775f7761965391e61757f0ddea11b3417997b3099518a94b9ab82a7574db3681a85257c843307eb656cb93cf0512341" },
                { "gl", "dfad27ed8786e51486be7c8579d4149dabae7ea4f9ebf39461addae5a215f402e1988d27f82934159d9ba1a1c73a4d68cfa35fe099a9aaa990565d8990f3ea66" },
                { "gn", "1db88d2bc9eefc40219f6ed5d3d0f8fc79580c6b9499652493e5d22afe93c872f560ec0f0d4820dd847b931d9d50a77bd98ad10dd38b011a82cfe7c60c943570" },
                { "gu-IN", "7611d9fec3d1c58a6d4837b30a63249c62f165cfa0ebde986e790fe50f2840b8f86cdfbd241145fb657b9476f08b9d94cd360ed94d3c13b91a3e80e12ee671a8" },
                { "he", "14e10de94986b4692d7ccc3e1104543f1d2c8f9ec59a5b6657669dbe8d085f5acd4e88f9eda59d5afe8854f321f758dfdc791050cb88437067ac1e0ad3bd8af8" },
                { "hi-IN", "116e19b72dd7ee5361084d8212d402c962f071d2adfdc3f16242de0755aef5beabf77d219772a33e39f0fdba102ae9c89eb039979f0da1f99103a2d5d4775f86" },
                { "hr", "8eadfd1651ea137039f3d0dab273212078b360037ac9fcb7aeb25d1c7ead24f48327d1b5b3489db4a2029e0e016a065fd211dba6365153a29b8084afce8e6184" },
                { "hsb", "09615b6338799f6a44399e4f00da8e346e225a19bc727088e623f5e8faae16af31692c9ec8c84c3d372964aacf06c178b571b61b9ef43ffb4a7b05d5b5a3e34d" },
                { "hu", "59159b9e289883eb47a9390f5f9f257e8efbc0d3b066c91df2604b4a24f9e5ca12ee607ac7205915f2c1cd09ea60267c6a4f44bcf8653340ce27b415814672cc" },
                { "hy-AM", "7dc67f100d00d36647a553e6fee7584c75f4a6777f8973e20087fafc425f24dd0083ed51c3073f7c991b8ae52f7bedd7d73cffb6858d2ef2ff54057440325b2c" },
                { "ia", "87b0e01d314f74eb8a0ed7fe5396917e66acf1bb39334edbb50e4db2e70a4c55d9747920907dc51d3be1a6079c85dcffe26d7b3252ed4d3f9db6454a0d24221c" },
                { "id", "380d70ffb4045367ca4a14c8c705c14066dbf51b142a2e7b40c735d86d27d537295d73f0641d16385b4970cff0c9be09d25368d5b7b3841d093481b4e6c809a7" },
                { "is", "289243ff16288d690e9ab5abc58713adc80e0b2ec653a7444e24153188ae4817ca75c953e2ef63318a9780a3b5a0baa00d576b2e1a125b6b75fbb829bf365adc" },
                { "it", "4e4598268c5f43953022bc93b8b48515c4e87b516a8f6091124d7545b9a5468f4260d81f6b6108b0720db3bf3f02142f526140b65d85097d6a8ebb9cc75b2b02" },
                { "ja", "6b9233bd5681a4ec68cfc05763fa2c6a7ca23e8032db7e4fdf924300df634acb4dfda959bf809f7184c43185a1ac9c6021d5814faf2ac8e6efb3ce5f10b6889c" },
                { "ka", "1de7db3e47861d538309290bd55a9032a080256742d86d5f02a95a8cb8fee3e795b541597a80a9210d54df903379de6fb2d2328e520b54d36e37a0ba7c4529d4" },
                { "kab", "d0b90c3cd50b5a3e3a822d9d6f0faf5a4d4e35e33a83684e1e6c3bef81d08198bebc7cef49af2bdfdc4e17a45247a166fd4d153156e234f8408de5e1233ab7d8" },
                { "kk", "80e42e6e899629d653deb71d7ffe3cec7c3d1da32dd78daec07c6139c18a6193a02b44abfd640ad23217146c9004c118b08fb175c7f57d5347651a91f9dca9bd" },
                { "km", "5f1065a39af927840bc8e9691eac56ee567bcbf6f35250e1e262060fc0624e88e65f1a1f6ea69ffda3e93cba37434495efc6e931a54313a5028a45ca69a05798" },
                { "kn", "304d0537e73ba5b40c9f5993f2f462632454e6e51da5f74a8a5dd6887506d4798d5a369b931f0c842eed68421ed7c462cd77edc83b4708ce10458ef2e092eb4e" },
                { "ko", "85c364e38e797367f0e8d305b1f6e0eb1060a85f22b05b381065983b687fd3f50167899ea2e1efa67bd147f4b56a89d7d4750a3d6b334a24d39f7f100f2419e3" },
                { "lij", "7c2de8f76c6a935c2845a7dd6a5855d4fc71e52fa93176c17f062844ba5601eec5d9697c5b60b732e76fcc67f04f5ea5ce8f59537f754b6f35bc771c5aed3ac0" },
                { "lt", "f91385f2b681436810864a92bcd2a9d45b423067d965dc11e06567e6527b8b51af672e8a9e4032345974aeccd13bc3c11f91faea5a6c454bde4de6a9b7b24192" },
                { "lv", "31913a1a4c730a06307fdf9ab5b4e91cb67024ff690078f0e39d9ef84676fcd49ffdbf5d05261671f5277c7a26b501fe04fb7c80ff78e4bda309168edff90c75" },
                { "mk", "3ff103084cb1c27e2deec81f09a7b483e9beeecb0a920efd4359be757135583b962588977a88e6c0965d79c5aada5295901e94d6a54f61d7021ba86928791388" },
                { "mr", "251919f01573ece20f4276a9f6c72e0f334406fb649dd82e0ffaa3de0062d1ccc8e677edfc0e3661605a85bfe802ecab80b8de1429512e79da8e25da19724192" },
                { "ms", "064227c52ff35cd06daf6b49a6c8bd757bc3d04ef785b3acccd5aedfadfd53b0f7439003721cff33445aef495ecc59e824754dbd08258ec254f3caef46d0387e" },
                { "my", "e479eae8bf19b4e56c16286abe859a765786481f103f041f6bc437f6b0dec390aff9af9a03c74d4b554ec10b08f3479ccc9a127804edada1713e262d2af47a6c" },
                { "nb-NO", "7b00ea9efd4cd2d793eef33f227f5583557af12605526177e33e712aacf63f769f2f2ef48eaf425e28c791f21130c93a4a9455b043409d73ce4593d3d11abcd0" },
                { "ne-NP", "08ef06fde80f105f51d93af590883267014ff57a65703260fcb5d75b65b78713d6fd0aad091ff3e924b7139fdf3fa5c615dfe25757f9593bb7cbc55d7e2d085b" },
                { "nl", "b79e0edd6dc87ae2eb0a4746c90b0fc9aee5252c24f715651f219fc596580c47e22fb5311fbdd62d4a1793ff84d91d0849d78f8cfe01e897289e6f1888a9817a" },
                { "nn-NO", "0f9f8fc2759a0551137af151e167302bc5da461cc774b699de12875a75ae632a8363f0063a45a7e7d46b867a4a302a5c476f3be0d5aaea066dfbd192d98324a9" },
                { "oc", "0a952a64209ff045f5e914255adab6cea3b9f23b687252f3cc90898aec749b3fcc137ce9cd65ab83bfb027f2fca55506b0fa75ef8d66a7fae53a47643e03625f" },
                { "pa-IN", "adefc15f75426d75411ce9ba710f605888f708638f6710e4ad4d480bda4cf11b1549f671251018948d10bc010fbb38ad24dfbeb988b8b023fe558f8e9b4117c9" },
                { "pl", "6526339daa4ff63d3f6f6d45028de33d03b07f8849d520342b5d0e3cbfb0794aa0bf03a65551b110c2b51542d28d8bc7a44756329e1e6c095fee88eb2f559d30" },
                { "pt-BR", "222a74c69e65f06e1e6026c5d6d4dc070901b5e6804c6d0e9913ef2ce15b4c8d3f5616771655049b4dec46bb006771e799fab4538a92b21c716a8a202fe7d2bd" },
                { "pt-PT", "d66b095a8fbba66c07cde895b5bbad405653e76e98c83b34081b3d74dcfb19927d5c905fd0eb7acd968dd00222abb410706a025228c0cd6eab677596c3b2747b" },
                { "rm", "5d25e6a60ab04d8bd28e80466d96a1d08501060f46dab3165b6ca57b1a03f89c8801a5ea39ba36db47346908514eea2a3db21f4f5bc531cd6c7ba97d1cc1183d" },
                { "ro", "796e30186b19207e3138903b979c46f72d0cbcc04fb4c4eea6318a47b96550cf1c1257433df859b394294411f3e4ce1bc8ee0b4cc8e08de905c6805208c08a78" },
                { "ru", "5a5c74957375b3b8e04dffe6bb1ce18050b99fae5764c0f279169971b7eb1c26c683ec5b5bbaeecc76fd5906845d45e45b0d7af48ba03d029881e97517efdbf7" },
                { "sc", "0681de96456872c02ea830f856e41f67e988b923ab12ce4d84904eaee51b45e5dcf4b112e6afe7bcb66aba769a7207ffe0fddf01c27da91c8463f0c877d7c1ad" },
                { "sco", "c00ce05a7cdba6a7b0000ac0ae4a77787d7fc88bf93375a84207b5eff574b88c57bff6d794eba6760fe0c384fc8f147dfbce624db7bd761df8039623739b573c" },
                { "si", "b2fdea1c13ef0fb5743529f60b6eaaf9a687f6546b0273c7f7ed2bf61b5964067697ae9cd88cb3825b210f6488e67302de85d54c55622ef076a441941f5333f3" },
                { "sk", "7d9011d0a22f5d4053eca6be585d812cc69e8108e6761f4ba8bae01eb90af57df2cb373b8de05668b2043cbf62f95be4b4ff40198065a8d43e4f5374c02bf6e6" },
                { "sl", "212ca28ff6c2746225a9f1b9030b86fd5028d75dad11c0dd64322747cc1124bd7df247fd8ce9d3c8c2b4b77f82dda49339e1f73e90cd1ce71469b4bf094883cd" },
                { "son", "77e241e7b564b5f11116d7dd93b40e70bcb95bdb14dc98df2ccd20d0d81fdafcce8e036ede48a87e44dc2c6c21d2037ef509d2b30d8aaf69e73b60c5e56fe2bf" },
                { "sq", "56c1c8bbecd496ea095325bdb988abad050d3ee1fd9b807b3854c7b0a288e49301fb68fbc8f911019a9c19ab11239e274e8ddba4f01a297d1df48c0fd7f066a4" },
                { "sr", "89d910d9ce327b2fd9574957b9e472808a0870dc4ca58fe6fcbc93de47e42eb8152ccb637f3b783e382f03754102778f291eb2f65d712567ed1affb97432e841" },
                { "sv-SE", "6dd7eec89d74f6ff2abd7c404ebe0c52ddd7eb8739b0f7fb8917638c00d389627256e1a10b69d9a6ea13e118388c9f5b337d3ae32ed8d914dcd8991f072d2d99" },
                { "szl", "aa184a6e2bb9a470d06545ad2e04771e0f970f1700bfe2edc68773b2232e645ca3c70ffeda7525532a8e561aed6f57912d782ed0e8a2c3906e017950b6343f5a" },
                { "ta", "f39375014e4787c4ecca8992210f228c2e887e57af4abebf9978ee5c9750f5e99ef768cadf8af61d19487697e94202e2f1f10ee57a2a81c44aab9474d0ac3e15" },
                { "te", "6c80208d6d6c739fb8f1bea4f5dad5cb3f582a4a374a3f2ef3f27f7e3471450344ce5bdf3524ffc28161204bb4c9384dac12d167c61d410a46ba6cac9c5867f5" },
                { "tg", "a9bfdc4b0ad085473aaa5f8449c72316e2805230bd4e093b5772f9a0c56aed13343f800698bc76a12d4943e035c1757e0a85b7e669cbd23d251b2e5054b09e26" },
                { "th", "a146f689af022f0dd30e4c89a9914adbb5baa4dadf3d611bf27cd469829cb082219b3a0f79bc31c6350616e99f15b153d0608945010666ab7f55529c817f0c55" },
                { "tl", "f6a158003b42dc0d16a6fa58d37afe407ead48d1c878559af011cdc08f1d470759cb941564311e08e9d101724565fb2e7065935d632def80548922c1edcf373b" },
                { "tr", "50b44fe4ed2613d69b137fc0a27a0bf2137257af0992e531e2996ac7bfc47d607212ff2329b32a6f0c4e3aa1469f699547f6f8a43df8805b79299856e83b6b93" },
                { "trs", "ce6b8b5be3a566cc2db497e7ef6fa5c68bccb217583016dc171d71a9575613539ef5f9f3168a1c29e99f42de8d57c0e9159b3ac58e8b31c09ca25f912f5d92c5" },
                { "uk", "eb776812b9b03b73f0219485d6aafc120ecc0c34d59d908731678764fd34ee1f011a225d82ccfce49ea5612ba77d185bb8805dd430f944b3357a0f513f94d2f6" },
                { "ur", "3bb350035e4f343a099ef637e93e7ae456e77b94971a47e1ad0a6dced634ddc25b011c3692fde08109565d7fdb5ddfc95b28e4ceffb1b96a6fc5a0878faea094" },
                { "uz", "5c9b246c791d19b429609639e671f9788d78997880037e33acb65af060b92a25690caf9e664fa103b8f3203eae2e33d5e86fc42ed8e0fcd68a33e6f6967ebf7f" },
                { "vi", "563d7e5ee8f6c2e8c01fcf5817da9558a07da0dc1fc6ede2b6226c873708b705a716a0ff296f95db755a16a7cffb9044a7fbc93cccfa16bab5aed52215ca8c6a" },
                { "xh", "de1222a71d1fbe793b2049be12cf4e2cb02e8d5c251bb22dc2c196c8625a646dfc4aa9aeab6b092055722dc452c40259e1887a04f468ba6ecc9fc8d444ca9334" },
                { "zh-CN", "cccb1d787d350f3eec9c05033f2a47251c920b4822c9b65e7e4d1ea09eb058c5c7c2001e5872384f161c9b8925e8d0cc8c13a05699e4baae39db23269e717f1a" },
                { "zh-TW", "de035c9f07a87ffec98bf37c01a2bcbefaa6ab1c7da018a2df880c752a0ec28bf1a03e893945e9437d8b0348caabcb1c291b62253c93f6a48d972307cda18f6b" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/114.0b3/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "c5b336a5b919be0fea56a8a72966e80adb072dca61b1ebfcb57c656e2474ad155a15d28cd1a0d6fdcab7e41fe96ba08e3fc3ab8ecfcffc65b56001fa8bbdcb9d" },
                { "af", "ea5de746606ade2de7049b5e8a2548cc26d8c7ddfc5690e6a7ca7972ffb5a36c9eb3057da90145a7ef7ba431f999e9a2acf33f123a13732c52447e761108494a" },
                { "an", "858426cef1a31d6f69f0560d89bd01d5dff63023d37a42582287924c7ced62eee3826d23799ff20e093c7815139353090218caec471f0da0b30fb6e080702008" },
                { "ar", "a075d5598f63818ed29a8110459cfd5bfe3109ecb95e8f1ded1a83282b87587e5a3aeecb7f36321a42a95fdf70dd50081f4e342be5409f3ecc927d52078f810b" },
                { "ast", "43a1faeb02f0c3e349429b401b87b1cd9c68113a78f52569d94ca694b9ad7dc1ed2b007d28632328a69c9948785c4f3137c9713c6f57c5567821aeb1a38b3266" },
                { "az", "f2d47aa44b4b5a4e6e997915456827835589f0e59113f6e9ccc1226e8e5e02c53e37c0df24d7af130fe2a9565acc27ed32c46db1326df5f08c8762211c5f92b4" },
                { "be", "e23456d631007da42b8dbe298da1caf598ea9a36af39e6cbacd4bd0ff702c3c0439f50e7548d19c7c43c8fe56235c712ad4b28d562efa1618ca5a06e66db1ee4" },
                { "bg", "191088289e9d06cd7ceb00da1b9ee362ea908df7c1ac17e576cb318507cb4877f2b46568eeb0234a1a3babdb389740c75977b832d7b483a70d8bde423d98edff" },
                { "bn", "6188d3e3bcad360e5d1902f9b55c0c707d1bc34621904e271046f35bc8cadb93d12dc99bd67e5963471907d1f0071297490800c6b8110c037714568cd05d59b5" },
                { "br", "0d921608ca633af8afcd006281ed66cd36d6aaf50f19b52477d8b996be13e4a1eb2a7a20cceb51aed24b234de3f20b5d63ba0728252382434ba183de0e2b2d46" },
                { "bs", "5b1b30031b269fb43200e091a1571bf56b8407f0c3815b0f9e19fda90c9e2c144fbb8c737b610bfdb8dc8d021762d17273f90683491e62ef7fb0e200ded50c0d" },
                { "ca", "d31fa34d660569f9e67bd3661488623d747c9c078608409e0e169b3e0e150090db19eb959534022e0ee21da0ea9917893ec7fde975929e3efad4a1ad8394343c" },
                { "cak", "2ef51f911a22c260450a5edf6f866b4391629f76fbaae7876ff252e06d52edbe7e06e7bc31f0112090b5f94b672b3d45bb5acf84fc442b362bd4d407c4a3e0b0" },
                { "cs", "daa1bf43ed3def9c91c89831a7b215566aa9e476794d6b6f7ad9563258541695a9d78283152b938044f91be94b2e5aa83026f00bb7da8833f6285110d5560d5e" },
                { "cy", "f6e6d2b319f8c0dc01db23b8c16e4c3e070a1beae3646629cb6d5e80f765d03948380bda8733942f7cef3d8444e930fd3742018d348278377e44eb92366ab099" },
                { "da", "bcc01ed5f8b56c83a8c9fa3acb30464a5139af32523c860ecf38c4c10e6e5831413273286da07831b24e0f713493d90cee9c2492bbef72a6bcafd47476c9c360" },
                { "de", "d21a2d7936840b30e68bb7b85576f9bb128288618b53cf073ee4833cb03176ac80aea3fbbd681d233b4720e7b3e2ab09d679d046595146ec2b74321813476086" },
                { "dsb", "23fb48dde5d63728fe61acc993c878f6bcb1587d5fa6d9c191e30c61002d9a9c25189c4da026187921dc04f79e200e944e789bde1197a8d01e5938493d663cfa" },
                { "el", "414796cfa71463094a8f5a20e547948dc8eb5e7c28fc38269bfbec2ea7537dbb6fb2bfabbbdca520b01d5ecd883a42d03bb7da299c6c5894386231b7fa77a7ee" },
                { "en-CA", "a3433c7087090ce14f298ecb357d43653c3091a8b237d89f48f48a26a717b977986398345494c8f0ba96f7550b2d055836ca1a65d46d47deb9bf600da622b8d5" },
                { "en-GB", "c633a8622a4c970560bd4f5e6119c51eadd12f9ef097a3b9a5e287c19a0a0727198fb0671595b6a0844f21f00569f5780deb41e13a2af011773e329379a69252" },
                { "en-US", "34bb7c8d1e8e3474ef89bf68eb2a4e3fb3333e767eb961f4397167a1bd1a4110f57e7820d5b85e7c53642c3bf4210891886527da926cb54f162d193ef1cf59ff" },
                { "eo", "e7f1b8e75583f3f95b5d6f5a2be19397c216e96edb946a24500ce7431162f036e3e27001ca8cdf1a113866af054f68c97b26432b97ccd74cd84b31da782104e4" },
                { "es-AR", "535254e96902dc88447f4d41d0550e55a3e901581316c418f71c2d9bfd416c23130b653b5a7f40a2e77d1835540fd5fee9d1cc8eae01fcbe66690a0134197c91" },
                { "es-CL", "b266be8cdea6adf06d3a54413939f8df72ef8df8595b695c5eac51e35496ce5e6acd3a604807568326da674ccc12189435203619e9178507f24c0b91e02cbeff" },
                { "es-ES", "90802e9b727aba29e59c4f36a3d6696543b1c6efefaf3875381618ced6d91c9d2e35676c3bb6de1fd5c292dd790196f543ac8daa2ab7a88db760054719b76269" },
                { "es-MX", "1b784a310319853be39d68da451d73819071cab23e1bf4d27a14e4138b565661dbb93cce56502f081a991cc2c464aade8cac0988fc5545852b1affb4c189c6d2" },
                { "et", "acbf439a52021aaf7a4dd532a21e87736a1ec2f0ee1b15a975a74968c472753b652354e77292b3592954541eb13dd2b86f0111c5dfc66107f78a9b3af5e1ea69" },
                { "eu", "d5f628e70726eb9b319bd2a77845901013829d16097b90211bbe0ef4d1fa2df7c31b996b6e2e706d67511c3a78d018b3caef8d932780bf017ff550c1b1d20188" },
                { "fa", "c9b3ca96609a0e8968afc1bd559489c3318a44645ab2c23833d5af2633970e64591352364c8eb8b8be3b5f49fb0cd9081c79ff5e9e2b31b0d84ff2299e700a85" },
                { "ff", "cc2f53674174334e165e13114bb5426373b0de71596e85d4504784f59118e6f4feb0164153c41afdc6002a7e432ff0bf65bd8365d6763f86efd3aef319d7ebdb" },
                { "fi", "56dc3ccef8d9e0859b85a8359afd781654bb942b4fd4611d9f3abdb98ac113fa559249325899b7b9996d0619b8c560d6c9f8dfaf62b65e362d262c5787486eff" },
                { "fr", "3dc44325a92d4a5b92d4bf3bbd416482516b4239c75ee5448eacc0e2e777738a1b592d5c6d8f03c9d556f5be682b0df9785fe9531ea79e8ed212612b2b54772a" },
                { "fur", "88cb5d18ea123ede25f41282915a7ac92a820cb34cc8127e919f53bffb48a8320a21046000732b0c9b96f9c63ced81b264a50a0facc302254b5c697f642ed48a" },
                { "fy-NL", "0241a9892b44b910d4f06ce02a6c87721e9d636beb9f79cc74d81ae14c416edfa654144ce0aa427d6467e583b5ab5fcc0f780f3e4194d99226d149de21c26ab1" },
                { "ga-IE", "5518cbb829268a5a98a4537be92b392a9ee7fc10d5315c7b7e7968f8ec35e116645652a8b48384157e4d844ed6936e4bf973033154016c494773068490529bf4" },
                { "gd", "10eec0fabf569f0924550543b3d255b7ac7e5b510162be10570bb27c6121710b67dd1c63eec73de96445288fd2f4f623d66b5768a44210d086756e240e0f2167" },
                { "gl", "286a0d9ef8e024e9eb99f93ce6c5370f1bca82e72adc12fcf55303bd95e72d8c34e8c8bbe9087e3f3ee55ee5de97e3a2b47363c32b2586d6093a28d2cc0827a8" },
                { "gn", "98f78a9c33484d54a7e34276ba9428b29edfbe4c052c46aee984df67b949bd57b441d08b8c0dc1fcc29801a16a5145fbd16131ba39fa96ccdd459b67aa720582" },
                { "gu-IN", "c4c2bf4b99c655369ee34c9643ec770bea9835ecfe42c859534814f75f6636064d8c9ad009ce076c8d49a75836e8ad1d84bcf2993edb896b24b7d8fc2b80cf82" },
                { "he", "a91f704d7d510da0c87b29d062a0b3fd8773bb54c23c28fab227ebcb17fe786325ef9bb7c7b0704d46732b8b92e75dd91e50adf75516b2f76d7dc271fb2d1f91" },
                { "hi-IN", "a9d1d1e8357059968aacb68cfd4e8420ca6c8fd34957cf7c5f5718d7fee3aa4a3b06877ad4111dc5d6a0df35ff6a27c3d0aa4393532a74c83a34e87f071d07c6" },
                { "hr", "3674ea7258d1fae44acd0e4bb97ed425de725424a1a4c3a7f34b4d2656346e388abf8cd37e78b4f0bc9c800216e3e0babef762dbcffa217c9e04a61289adae39" },
                { "hsb", "df33fc9156839ab6885c3c7e76d3fa219d1070e4992ec41534e6ffa5bf69647163f9db9e4974ac386f26c5d87ceee1b4c9ad07cbba9c9552073861545db9576a" },
                { "hu", "298d54f341c20b9ac11f4902f0f7c6974e81053592b54fad572abb8f56600d931c51fceafe8eee567a76f6a13f2830b63ba0cd38b3e22a6dab2b199bf9879b49" },
                { "hy-AM", "0c2415d3fca586aeecf671e4d12d20395ae27ae0a505885228d215139c605b7cf6d989d86c7d3880064af11aac634d961dd68e62251e471d77262ec954d3e63b" },
                { "ia", "71db64d1a362acd265716dbd442e9a239bea25ad775f6e21af1ea565ff7e4f48034a03831ae20b00a9a98c889a383e1cf6b6c456682d1cfc5466c64218983b3a" },
                { "id", "a8ea46993fc21004d88bebaa028f1370e0b8effc061743cd9809bb0b20eedf009718dacc4059be3336895a5701f3eedbe61d69ccc622936fc06c18a12bcaf7ef" },
                { "is", "8b1c3d5e093fc97b3214fcaed09af138e6c145e332644e8ff83d3624ca2e3e9fd4f028f518f505567f9b9bb05db7bfbd3ed12eb306a78e4dbb7069bcd8756a93" },
                { "it", "bd422f47aab95c646416f048ecf2265d00cba6fd58e5e5e815c50bda6bf240b986696d33b581054a8cd329abd4deb1ec5e7f0d986c1410e60b4e16c463556a05" },
                { "ja", "35558c0ad733159bff993911bc4680ec998bd7fd3243392bac74b7ee5d534bd834621a532fe94f524f4f6d265f57c00b82385c42cb52d1561c4efe7bdd428980" },
                { "ka", "597faaf9299282bacabe746dd4d7764afea194d13a1032cbdaf5eb4455b3c7208c0f87b51ed08ea46994fec172d62008d37957c92e7b5cc81a9c0263fb444afa" },
                { "kab", "eb90df90ec7dcd92b740295008271b835397a5e8c4a4be028075c366d696d7598fd69c1119101b0675d7daa7bb2ce0bdd2bf01a380493fa8ade7696f8b7889b7" },
                { "kk", "ce8c88046ae8d946d43ebe0e00ddd01de20ccf882f40ebd031bcd25a7db857b0907caaaff2d1baf05674f383097dbf7d486281ffbb948254f112fdfaf59138c7" },
                { "km", "45daa4c233fd82d6e98b48e226095b7c45864d06d5143fed239b7901c8335d9c6153d0d48db85b00a0a7d3b21763ff62fcb5820093fba7242fb7414ee48fb483" },
                { "kn", "2da54815fc9723b440acbabbde73fe080ad7b958febfdb97811e1a6ccaad7ec4ebc8177f5d013484488df0f4cb4a4364f175d1334f1843e0b796280791814770" },
                { "ko", "12ff8b4a284565ea1cf13356029429afd217ff5591837c951da09b9b68f76e39e9b97fd66191eae5796ba4867b708cf8126e561652b2771486fe91df029a2505" },
                { "lij", "64cc56c38f18472e14f8259784efd6d761da367c50494cd27662087d97a09898bac1b90935d401821e315eea960b47f7377413611c18d594eef026e97a1366f4" },
                { "lt", "018610fa79ea44df09c1ce99bab630e70af89ea03c58b37aed3d558fde3a34fe088c548350ac238b22b0d08301c18bf882a1d04ca6a3fdc11673bdd920ad4587" },
                { "lv", "9a5af97d3282e977208530e8b897030c91363522f3f6f85f6dda17a17525e35ae7617af90b3d5a930716c7bda05519f2e630b3e1b36afbcd7db8f06e80d28275" },
                { "mk", "2666bbfa63c7de90ed05b1ed221320b8353c4e5e9be5180460052f3794815b1042baf597e5a75150866109b66002de0f006bd20f5b4e0bc82eba5d8756ea9ead" },
                { "mr", "60417108282d287e5b13f19f168b100a6ace0a0848941af6071230bf10f86925a5f63a1bd3645b27c0a2216892ad9522ab5edb11a1ea525b2a31544505ba2833" },
                { "ms", "a7215759fc55f092d02ef718d09995693ea9297f3430933fbeb168146424478cd7e9a67835bd40b468556bcab7d2ed9187b5dc0c78d368be5e9556851334d28f" },
                { "my", "cde5b5a0992e7c2b99493b255bd9ffd0e64c5d380b89db9277f6b7cc4d5bf1fd84fcff4896449a65b85a98ed1576fb5fdc9cc68f716a97b5d02029ab0ac59b79" },
                { "nb-NO", "3292aaf0c8610aadbe397b93b2c1b113b66cd26efca04b18a5fef090330bc77b0727984426f7a32c50e2067dabe35240d96968a676b46412af160053824b2cc6" },
                { "ne-NP", "62a3fb8c943bd7bd2914feab281bf57fa659afbdc368deb12225b23541c403da49da6340798338415dcc4b76aaca8ef9bcc9ae44157806df7bfcb62abd059a61" },
                { "nl", "50dbf25377eb996bd8b4254f024389a32607ae054397d003ba5135359ecdd853ab4a78351e9b19ced7a1f58111b20b5769f0307d5dac6bfd82d6e3374a6c5918" },
                { "nn-NO", "d2b71c14cf5ae60556d1dcd93258c6b1e5ae299d41e8dc287185bf3cc8c93f5626e9821b7a278a5faf58956b4fcd3f98140e359bb9e58aceea97b494c6da1883" },
                { "oc", "57c24f41b279223a44b59b804eec0ed93609573401516b58a621753e47908e2b1d1bcad938a410706f6f29d58de421c6819da567984340a2f5db5f9e99737806" },
                { "pa-IN", "7bdfb6d9f583976f0e31ca88a24f63733a745326127ec3c74a599e2366ed2965cabe020e29846eb72a7425614559adc1ac138476495a3b8e2defd6118c46e671" },
                { "pl", "d2cb18021363d0d51182870eeb39ffdc40028813e0c5054c9de2042d01f402c1d1defb4a00c9510da874d3bd12067f9d9f5c39248a487537bd40628fb8dd6319" },
                { "pt-BR", "6a3aff1dcb4f1020e36f34cc46b7f7068c6caa3517031cce4b0e36c9242cc20435c08e6eba6b20422524d21f2f63a18020bd91fe6497bbf2fc227c01bd52e8e6" },
                { "pt-PT", "9bc79f0d183e18c70a5b70ca16121f01401775527542edb5cb0dc29b5ee109aed08379e7c492473d2ecb728c7568ec4890434cca839dff607f78330217483b4b" },
                { "rm", "2e382a4f4bea9c0640edfe7370b53cc0b3c1e77f9b6e7dee7190b8944a73a00ab3f434c4ee611cf29a0fbddf96df0f98941d57eed832ce292086c576f85257cf" },
                { "ro", "b1c173d23694e0682f38951c2acc0c0854a488b2796def7a10785b3bf0e2a62cd4ba20cd58fdf5468939bc2fc98a2ded90bae5131cda617a127ddcdbdf9f33ca" },
                { "ru", "f661d2d01cde3b89ff8439700665336b5d271ab89aadb8ed3ca5267e7a8768b9605736ddb3177d485557696477bfab04a2fda099f8fd3a7b0836f31b0a8bb192" },
                { "sc", "0c04bac5e205d04dda3375b9d9b3c85a435df6a5b1e83bd9a75e11b3d3721f9bec0efddcbf68fca2e012542653cab2f0424f2e898fc3aa45c9659c9c2f07d29b" },
                { "sco", "fc9317c7c44f1fd4c1e43fc88e620697b6b00b08d1bd40f288a6192293b875e92503782b43389e29951e130d8ecc5b977cb7eac895850e9e0edecc55d8b43902" },
                { "si", "f8e531015296113e77c6e4b9c94c39e635fbb85d3be57fd7574ef6e83bbd134d399d88fc462ac004c76716a5146837199c1e98676ee0625538925983249701f0" },
                { "sk", "28598ec0e3f7f068260c3799e0387d855831f32a4033202900415fd0cd67e2e8d4c743474798e37b9d79a9833f6b5891e5e45dd99c2989b670aa23dad64a2a8d" },
                { "sl", "63ec1ddc78edfe47fc1d59cd09fdc3606c074f29e43ffbb3173d877b285bf0ffc8e468c0f0618f028b97729d650afbc499f6c6697b792a4802cfc52699b91665" },
                { "son", "9f68b0e1ccf9f9c518d32c7e28969fa7e49f9df8d8710d08416ef8ec793a0fdffb878c2acf5623920f789de86c3ddd161e823b9ae605cd3570cb96f0d85f3f96" },
                { "sq", "b4feecf5dcb29197b5ea2878257c858ea9d39dda11d7e301a23321b52b7e974713940461570959effbe0bb2a5f0c6b6c36a6982fd2641ec3197ac15d23132134" },
                { "sr", "92ec3ab0b3197da8ec95abcd8d5771c2f822ad9b8b156042ef20182cb2b0fa63b2f52da8e3a30111f7e2289ca6b8ef55f5ed0d6eaed4a2389dddc5736a0f5e83" },
                { "sv-SE", "b062237358d44d13f4bfd8d949423a01b34468cb9fabfd942b533da1faca3c17089bc238274b86f82daa1d397191cf060364f643fafbab17fcd0bace1047f2bc" },
                { "szl", "175d34e9220c51b431f5e372f9eecb0b010bd677abe39a74e97f7ef891f372fdb798d9b8bc42adfd4486d9fe2d053b288c9f584637b897e9e428ecfdb070af20" },
                { "ta", "d1924cb6b8651a16c347ba6a33c6b35685a687d95f1c3c68ebf3aa62ad8354e9a5c8252bb4aee8c10f5b9d83df1259e2f7122a327b402c3b424ca5ef922e4d8d" },
                { "te", "6968402e7071f8c69c4776591019aa6398cc73743f0b14262331d9139f39a8f98cfae8b42e2c62d6a8f1f5b9e39be47b9a23554403dc1d4b620471db1ed48f1a" },
                { "tg", "18b87ed3a39429b38dbc0385a688fd50fec13f4cc8ebcfd6fde8daa964daeaded2eb47a442261cf2642d34df8790bf5c3cd36580752e0c376c2823fc614e6284" },
                { "th", "c9c676207bb8aed17a7b9b3290d90be81fa2fd2704f440e25fc337287eaab61c24e899934db7d90eb6a68eb759acba249556b9b14f566cb5568c522360912412" },
                { "tl", "38c61d944dee62b93944fdc6a4d078c3d968b11db0692d3355200a287dad9d75000252fa7e7e8d9716c9f905eb7f56498f8576a6c9443561fef5a1bd9df07c1c" },
                { "tr", "6f377108aa78402861561578eb6f309d7fc8c3f1d8ea2539f7f65e9026a00e9dde7a298c4d77bd1a3c14b66317a46c3a1d4f16583b0345dddc74b52ada927c36" },
                { "trs", "36207e399b878a6c6807eafb40f8bfe6060b2d7f6831a3e44543ab53d89aa21013b81a6356a4df429fbacc0a37e098a3429b8eb50b9aa93cbddf11a7f05c273a" },
                { "uk", "710961c27c335cfdd2797375d3652fd0f19f6d0c0cc5acf41f162fa9b0123a636dabec694ebcff414de2f932fd24f3a09a2ba3b6af20234a42e696e16dae9a34" },
                { "ur", "302d26857cb6083ee9526bd388278fa5884152a82951c7295c6ec70102ade8fee731618eca2eb9b889dac39a475495380e6f7d5ab46f4299b6312ef00e9b6ad7" },
                { "uz", "3c9e737da504c4f89721a23a88df84dd015dbf3ec29752851575a1ca74468f22d742512d9276c3ae1767703dcab707f2941f70c9ce6c5a8e8bb3e7f39eb06dd4" },
                { "vi", "f1721621f2faf71fa20cab7b2854f1bb7dff16788bc5a2752af27d20edd3c0e020e03e0f531bf42442a0cee6128923638fe3c182b33c2e0cd2a4b81003daeff0" },
                { "xh", "a22cd1f0fbb30bf68e93e7de50833fe5ecca264d4b2b278cbd52ceb6a4f621cec9bc9dbb8476c4f56bd7e70f57cda0a7b05d45f9aadb0c014a750b2aec890959" },
                { "zh-CN", "78b13f0a6556cdf1c6893fafe6127b6be5faef851f6b03d9b216e48563a5d82b3a76f8ec1de81bfa66d9bf4c8d2f9fc353f8c5db6c0587903de1fd82f71152cc" },
                { "zh-TW", "07efafb008ce688ffe7712507b392044b93e2fdc3911b1f57b3be128c89d10b4a882783d38b4b679cef4dd8bf77b22e6b148893eba1135b4de7e92f6a9955267" }
            };
        }


        /// <summary>
        /// Gets an enumerable collection of valid language codes.
        /// </summary>
        /// <returns>Returns an enumerable collection of valid language codes.</returns>
        public static IEnumerable<string> validLanguageCodes()
        {
            return knownChecksums32Bit().Keys;
        }


        /// <summary>
        /// Gets the currently known information about the software.
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the known
        /// details about the software.</returns>
        public override AvailableSoftware knownInfo()
        {
            var signature = new Signature(publisherX509, certificateExpiration);
            return new AvailableSoftware("Firefox Developer Edition (" + languageCode + ")",
                currentVersion,
                "^Firefox Developer Edition( [0-9]{2}\\.[0-9]([a-z][0-9])?)? \\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Firefox Developer Edition( [0-9]{2}\\.[0-9]([a-z][0-9])?)? \\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32 bit installer
                new InstallInfoExe(
                    // URL is formed like "https://ftp.mozilla.org/pub/devedition/releases/60.0b9/win32/en-GB/Firefox%20Setup%2060.0b9.exe".
                    "https://ftp.mozilla.org/pub/devedition/releases/" + currentVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + currentVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64 bit installer
                new InstallInfoExe(
                    // URL is formed like "https://ftp.mozilla.org/pub/devedition/releases/60.0b9/win64/en-GB/Firefox%20Setup%2060.0b9.exe".
                    "https://ftp.mozilla.org/pub/devedition/releases/" + currentVersion + "/win64/" + languageCode + "/Firefox%20Setup%20" + currentVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum64Bit,
                    signature,
                    "-ms -ma")
                );
        }


        /// <summary>
        /// Gets a list of IDs to identify the software.
        /// </summary>
        /// <returns>Returns a non-empty array of IDs, where at least one entry is unique to the software.</returns>
        public override string[] id()
        {
            return new string[] { "firefox-aurora", "firefox-aurora-" + languageCode.ToLower() };
        }


        /// <summary>
        /// Tries to find the newest version number of Firefox Developer Edition.
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        public string determineNewestVersion()
        {
            string url = "https://ftp.mozilla.org/pub/devedition/releases/";

            string htmlContent;
            var client = HttpClientProvider.Provide();
            try
            {
                var task = client.GetStringAsync(url);
                task.Wait();
                htmlContent = task.Result;
            }
            catch (Exception ex)
            {
                logger.Warn("Error while looking for newer Firefox Developer Edition version: " + ex.Message);
                return null;
            }

            // HTML source contains something like "<a href="/pub/devedition/releases/54.0b11/">54.0b11/</a>"
            // for every version. We just collect them all and look for the newest version.
            var versions = new List<QuartetAurora>();
            var regEx = new Regex("<a href=\"/pub/devedition/releases/([0-9]+\\.[0-9]+[a-z][0-9]+)/\">([0-9]+\\.[0-9]+[a-z][0-9]+)/</a>");
            MatchCollection matches = regEx.Matches(htmlContent);
            foreach (Match match in matches)
            {
                if (match.Success)
                {
                    versions.Add(new QuartetAurora(match.Groups[1].Value));
                }
            } // foreach
            versions.Sort();
            if (versions.Count > 0)
            {
                return versions[versions.Count - 1].full();
            }
            else
                return null;
        }


        /// <summary>
        /// Tries to get the checksums of the newer version.
        /// </summary>
        /// <returns>Returns a string array containing the checksums for 32 bit and 64 bit (in that order), if successful.
        /// Returns null, if an error occurred.</returns>
        private string[] determineNewestChecksums(string newerVersion)
        {
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            /* Checksums are found in a file like
             * https://ftp.mozilla.org/pub/devedition/releases/60.0b9/SHA512SUMS
             * Common lines look like
             * "7d2caf5e18....2aa76f2  win64/en-GB/Firefox Setup 60.0b9.exe"
             */

            logger.Debug("Determining newest checksums of Firefox Developer Edition (" + languageCode + ")...");
            string sha512SumsContent;
            if (!string.IsNullOrWhiteSpace(checksumsText) && (newerVersion == currentVersion))
            {
                // Use text from earlier request.
                sha512SumsContent = checksumsText;
            }
            else
            {
                // Get file content from Mozilla server.
                string url = "https://ftp.mozilla.org/pub/devedition/releases/" + newerVersion + "/SHA512SUMS";
                var client = HttpClientProvider.Provide();
                try
                {
                    var task = client.GetStringAsync(url);
                    task.Wait();
                    sha512SumsContent = task.Result;
                    if (newerVersion == currentVersion)
                    {
                        checksumsText = sha512SumsContent;
                    }
                }
                catch (Exception ex)
                {
                    logger.Warn("Exception occurred while checking for newer"
                        + " version of Firefox Developer Edition (" + languageCode + "): " + ex.Message);
                    return null;
                }
            } // else
            if (newerVersion == currentVersion)
            {
                if (cs64 == null || cs32 == null)
                {
                    fillChecksumDictionaries();
                }
                if (cs64 != null && cs32 != null && cs32.ContainsKey(languageCode) && cs64.ContainsKey(languageCode))
                {
                    return new string[2] { cs32[languageCode], cs64[languageCode] };
                }
            }
            var sums = new List<string>();
            foreach (var bits in new string[] { "32", "64" })
            {
                // look for line with the correct data
                var reChecksum = new Regex("[0-9a-f]{128}  win" + bits + "/" + languageCode.Replace("-", "\\-")
                    + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
                Match matchChecksum = reChecksum.Match(sha512SumsContent);
                if (!matchChecksum.Success)
                    return null;
                // checksum is the first 128 characters of the match
                sums.Add(matchChecksum.Value[..128]);
            } // foreach
            // return list as array
            return sums.ToArray();
        }


        /// <summary>
        /// Takes the plain text from the checksum file (if already present) and extracts checksums from that file into a dictionary.
        /// </summary>
        private static void fillChecksumDictionaries()
        {
            if (!string.IsNullOrWhiteSpace(checksumsText))
            {
                if ((null == cs32) || (cs32.Count == 0))
                {
                    // look for lines with language code and version for 32 bit
                    var reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs32 = new SortedDictionary<string, string>();
                    MatchCollection matches = reChecksum32Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value[136..].Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs32.Add(language, matches[i].Value[..128]);
                    }
                }

                if ((null == cs64) || (cs64.Count == 0))
                {
                    // look for line with the correct language code and version for 64 bit
                    var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs64 = new SortedDictionary<string, string>();
                    MatchCollection matches = reChecksum64Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value[136..].Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs64.Add(language, matches[i].Value[..128]);
                    }
                }
            }
        }

        /// <summary>
        /// Determines whether or not the method searchForNewer() is implemented.
        /// </summary>
        /// <returns>Returns true, if searchForNewer() is implemented for that
        /// class. Returns false, if not. Calling searchForNewer() may throw an
        /// exception in the later case.</returns>
        public override bool implementsSearchForNewer()
        {
            return true;
        }


        /// <summary>
        /// Looks for newer versions of the software than the currently known version.
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the information
        /// that was retrieved from the net.</returns>
        public override AvailableSoftware searchForNewer()
        {
            logger.Info("Searching for newer version of Firefox Developer Edition (" + languageCode + ")...");
            string newerVersion = determineNewestVersion();
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            // If versions match, we can return the current information.
            var currentInfo = knownInfo();
            if (newerVersion == currentInfo.newestVersion)
                // fallback to known information
                return currentInfo;
            string[] newerChecksums = determineNewestChecksums(newerVersion);
            if ((null == newerChecksums) || (newerChecksums.Length != 2)
                || string.IsNullOrWhiteSpace(newerChecksums[0])
                || string.IsNullOrWhiteSpace(newerChecksums[1]))
                // fallback to known information
                return null;
            // replace all stuff
            string oldVersion = currentInfo.newestVersion;
            currentInfo.newestVersion = newerVersion;
            currentInfo.install32Bit.downloadUrl = currentInfo.install32Bit.downloadUrl.Replace(oldVersion, newerVersion);
            currentInfo.install32Bit.checksum = newerChecksums[0];
            currentInfo.install64Bit.downloadUrl = currentInfo.install64Bit.downloadUrl.Replace(oldVersion, newerVersion);
            currentInfo.install64Bit.checksum = newerChecksums[1];
            return currentInfo;
        }


        /// <summary>
        /// Lists names of processes that might block an update, e.g. because
        /// the application cannot be updated while it is running.
        /// </summary>
        /// <param name="detected">currently installed / detected software version</param>
        /// <returns>Returns a list of process names that block the upgrade.</returns>
        public override List<string> blockerProcesses(DetectedSoftware detected)
        {
            return new List<string>();
        }


        /// <summary>
        /// language code for the Firefox Developer Edition version
        /// </summary>
        private readonly string languageCode;


        /// <summary>
        /// checksum for the 32 bit installer
        /// </summary>
        private readonly string checksum32Bit;


        /// <summary>
        /// checksum for the 64 bit installer
        /// </summary>
        private readonly string checksum64Bit;


        /// <summary>
        /// static variable that contains the text from the checksums file
        /// </summary>
        private static string checksumsText = null;

        /// <summary>
        /// dictionary of known checksums for 32 bit versions (key: language code; value: checksum)
        /// </summary>
        private static SortedDictionary<string, string> cs32 = null;

        /// <summary>
        /// dictionary of known checksums for 64 bit version (key: language code; value: checksum)
        /// </summary>
        private static SortedDictionary<string, string> cs64 = null;
    } // class
} // namespace
