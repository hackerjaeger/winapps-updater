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
        private const string currentVersion = "117.0b7";

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
            // https://ftp.mozilla.org/pub/devedition/releases/117.0b7/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "8468e8b450630b6bfe5b9617bacd4e275fbbf12db529adbe0b7e45d56851a1693c02e768d9d4dd21dec3fe14ba3b257fcf7d5296f531a3b342ff6ced7834a7c7" },
                { "af", "13e16afa30a09d59944a195b3eeb07d2d3f3fa61ce7411ecd8a48df8763939bac594fe3512554dc4679bef992e2e080e1cfe7cfd00b644874201f4c4e84c95b7" },
                { "an", "ab169ba7197ec2c27f464d9cc53ce048c244ff2fb923a47b18e67c6bb272a699b7c2d544d26ac51f166516d157e790f4862b75d7555e154cf649075378498f7a" },
                { "ar", "480a8b4c42c96d243101fc553e988795c55916a5b64eac758e35afdf483c9d6eacfb864ecc5ea95ba553df462eaf5ea19774480a26f0f27508809dd43fe5403d" },
                { "ast", "64e036ad63c6510cc3c513a0e56ea8f2a33f49207a4a3d2d623561aaafb690f664d66b6f96252616bc1e79f5fc6be1304dac180d031c73becb695e95a4b7b8e4" },
                { "az", "016922ed1cb960ca3a0a2f2c5215da2f4b3e093cbb6f325d06435e09c52ca1d8a3559ad6a1f79e677651cbebd1ea017edbc0a064f306902515c89494d9fbf8e1" },
                { "be", "60a319126ca341d23aaa31abc4c379ef38c3ba40bda8c0eaf5c48118f1af7d58d97985211849a37ff77f83f08774f1638eb8b8156d541dc163524ef3a7b2e072" },
                { "bg", "6eefc1ba888ce27e2a442c15263246736de031adf3ae4cd4858860c891c4303999e6dd71d50957bf257f6fcea8f7b6b625c6048bcc3b9ced1d5d90c8b7842ca3" },
                { "bn", "8405ea5dd2237220c0a4479f6b2ef8cfc70c4215a399597b2d84aaa13dbb577658f923ab3de9093c8ccb9d4bdd19727ad04f82da3e61f215ae57d93d0cc646c5" },
                { "br", "38247eb68027de37ad8c9d82f09be8427e33207ad7435879e601224e681d4525e0a60498aef899fe32b2b8a30b9671b054b0b04d2c3f3ca692e1b51acc8d075d" },
                { "bs", "1c9dcd266e77256dc5c680ec47514761456be7137cb07912cb784cfd2b714d3068e4a7a0ba00a1b5424c3e94ac028d51d0fef8d7b0e54d20077ec8b110f40521" },
                { "ca", "b9b8b765adfe17f42484b3550e751492936795f3736773a883fb1ba21020791709db24b6f574e6fd971eec304325914566a1dcd0048bd7360a75bf71ee783144" },
                { "cak", "d9ea0168f0a68b75c42b62bded778614d36a33c4bbac23b5cec615797354686157165dd8c5b1e06fb88d388de76eb0007a26279638dd6dfff0af27737b6d6e6b" },
                { "cs", "f18072f5a720cdca334ee78ebcb708bb545299a828930bcff9c20c1b58d00f338e2241c1fd820774b577c5c0a8c27566e2df14e2a2c96c2eaf0d65e333275a9f" },
                { "cy", "cf0c4af41e3b3c9886a66c9d0b6c89123c3183541394c38ce0b1c3a8da0fbbc507bccc8d323940520f7d90c875b0d5df76e793ea69e4ddc24afe4e0b9ac796b2" },
                { "da", "21f3a2b73489f55f50b503d97c365ed9783a622d0ae22cca4a7d79d53e189340cc4b32457e1f9c8aa9eee631816d257e8e6e3db45dc5046f482178fecfb84732" },
                { "de", "230f676bd28e961c6aa7527660376601e798f9d1593a87ba7e8cc6908786bf0f0c5304c540af017c1537727b92c7d1615fc5c4238300e00e185334a1e601503c" },
                { "dsb", "eac307247116d8abf2f911ba573745624309687d1f0ed93fa2ab5498e05545cef98b4c8ac1b4b5ddf2146c9231cf5220299280ff88ab26d8d6ef05e6f178a475" },
                { "el", "495b3f987bf31ef993bf155457b904cfc233d1e060db4d81379c87768a114c99411da853d5c187e0ba45bc090a594d4a9f16d7bdf37d9eb2ae26b918f539676a" },
                { "en-CA", "ffff27ae7dbe8a4413e0ea0a4414df7b4df450ab6ba083baf5e029ce1d0dec049d0ada77f54c19275da31d5779ca808b9e73ebbd0eabeb055128104fef788080" },
                { "en-GB", "67d5cf0b2630eb2b6c65efc27d0d0679d12425d6a0257b2be9be4340dcd4dacbf2ccea89248c879d6d3f215c86a9a6e03eb886f73fd73e3a21f095197f4b2d36" },
                { "en-US", "4ec5a21cb9b5d6df0e917ecea1e4020d0d2281c5fb5ad95b27f59a827d8aa52da210672ace7d89b23b7c2a8206b89545d861630e4f3b9114a2f6d17fc04e31e2" },
                { "eo", "9192e0b55af181aa4d9ac83507efad7786a6d41b3fb59fb1a5fa70181a674223a6ed88d353c5621925bb913271a7072e0526f42cdf9584e69b069432843e5146" },
                { "es-AR", "c19c5df2be779d1b82f3754c136355529b323b06c63835cc9533cbf0b0ce907282a79665e952c2aec0f8470a9671f47359d4e40cc6206255072889d8568af189" },
                { "es-CL", "8e6067556bbdedf8c6545cd3bd26b83bec79c2b18348ea2699bc256dac6e15e739659ef306f8f47383a52ea16b0bfe823c462b1efba98a89610686b718dbb242" },
                { "es-ES", "775351b7dc4e0be7e622e2871e83d6def7f4a1d23daedb398432aa6a34f8e3bafff994755a29fc5def9fcc782e0e247dc79ac8623a166daa7b52d1decee869f5" },
                { "es-MX", "8d343577a2a2d642366751bf4c2491529f572ac8a4dfa3c5503a0aceb11c87847b7d8d94aa1cdbfd555a0a682ce493e2b3d768d294fa96dfe4954e618b9201e6" },
                { "et", "b96eeeb6ffb69c73ddd48e72bc99a9b441cb71139f4730fed87fea3e2927a0068c73c716f86a2d3d0821298c41bea89132ee5ca7f70feffe3981a2e550b41b0d" },
                { "eu", "227fdb852564a46ed69cfdc2f2fea0b61691576c3b23d7fadb50efe2b3ca22f5cc50b4bb9403c01e3cb8070d0c6e5370f876699ab6059f3705ca1a5716a692ec" },
                { "fa", "b00a5429e5b2e04b151065cd643ca8f2a4efa8e933211305d22691bc08c229d74c9ed289fe7998acec701637e7163a793b5a83901a755f2141eb3f78a4180a29" },
                { "ff", "172b68e6b9d41746394b53167612d7b9b95e09ca9ec07b1852e0f9f86c7ffb90e292467a7a4105d277cb5b880cf91e41c4a1ebe979168bce67e02bbea05925b1" },
                { "fi", "928736168ae7e6787a8955c172fd254cf72f16ec6fe36672bce4163d1a0c1f54558ac633b636fede9260aba461810c83bd18946945cfcf7893d332145a264bd8" },
                { "fr", "7cfa6f5c0dba50f86da2eea8ff9f4bb92486877f560b99a735b949b8897dc70e75d68ffe83160634b94e6f2872f4c25bfd29719a5a3cf3ae5934db723ad54dcb" },
                { "fur", "52d3d4c1299a78c8a4b68d77b1d9b44a3f87275c9b3e9f21eceb599bf4b17fd8f5ecc9b7c39dc0bc86b38b7fdd2b5d289438fd3ded537fc1e4085f0b1d2ae74a" },
                { "fy-NL", "870caed47b1288fb46e33173ad273b774e52ff7c0cde07d1429d3e82e4bccdde9a1e37eefd1777f11ed076e8dcc8dcc4a174e159ab6c2928b4a4ea59858689c7" },
                { "ga-IE", "a25976374c998bf30951078310921c2cc0abc9970bf195c348bd0d8f25506d736f52cbe8898875016eb39430ff903e6d9ffad11f5fed421773319cdd4a20e818" },
                { "gd", "c690fb2dbe342ac623001ed406e85fffb1a3a742c8d971d5250ecb634e1957cf839ad0f82f4f4827b08a324433a38ec2f85646b8ef88be754d809c659bb87f5b" },
                { "gl", "e1402877b714f939fe43ec91abe25a15d7b7fde30d955b29f3f68ba14abd2cde4b630d4fce1aa778e5ba202005d1e1e6cc8ef7a98fba1c9fe13ba71272788599" },
                { "gn", "1a9192a7b89291009b93c416109b946c729a92bcfcadf28caa9ee0bcefe5c1bb06fac132eaa6ae6f6bff350c5c08a12a5406a4a5e85098b5806d7ff4470e04e2" },
                { "gu-IN", "29e2c10092a4b9590f7ceac86c24fbf9d1a27ed4c6ab657e7acfc6299feafa4ecb8893d2a6c182b88c05b01565f0a6862dbd325ee6cc86e8e2e5de3912f1fc48" },
                { "he", "7079bbaf2619fea84c37000ccbf2ca3dcf304c8086887730720cc64bac1ab7e078a58bc429699c66c5a52e47c30e3cf3a5e409884ac7bb7e54493ef8f801b7cd" },
                { "hi-IN", "43a324fee8dc363074f227af09ce611ac8bf15c0405a916dd667d459d100429618cbe7fb153acbfaac68458dc8187c05c813eeddd337e7baa0930d3dce1528bd" },
                { "hr", "eb0643b0856be48e6cfebe7a9eaf223ef5a682a1aa703aa7e4462ad80ce779a4fbe70e1cbac8c39494fba23721af538c9259997de2d26b145a53492156b1251d" },
                { "hsb", "8fef21f941b765881a7b63984bbfcc7010a9d06178d2714734c2db56b4cddd5c767f5a4c113560a8ca115f78749485adf31858d5c76daab8952bc35e06dd24ba" },
                { "hu", "3b8a9fa980fbcc6bdb922c1ae0e59af2fc1e98c301471e0ec50ba9d3e7f66e57e3779e95db59d8c299b856917a7a35f7d0cc4a651f7c64e78666063a99d7e512" },
                { "hy-AM", "d715b48b30ec1d503880bd73cf0b52306d55b390d8dd5e8cc92d08be198fe8242ad7dd023618d78ff63ccff37c5541e7379bd07b75903e2b7b6f7590969753a9" },
                { "ia", "0b51fcd21b418db738dfea8c415a3f4e119ec246ca39f4667222b7a8d8964d6471c0d4645100057e753fed9184bd80ce1cfc0082da6a5d9b8f54b57a75697112" },
                { "id", "eb6784fe82a45b8646913eadf5ad2c9f5069d8ddb7cc7326cd89b84949de2c8b90a4fbf3fa554f9e086c55ff6a86f06900b77e4ba5c245d11cdd092f3360a4df" },
                { "is", "f1712fa338e14334d41056f89f5b96e2816edce835d3c189089b411c2cdd2c859b810de7c1714ab7c1b5de2d76cab3d1ae9a12bd2d7abcc761af18ff1f370575" },
                { "it", "eaab49e0d4e5ef6247eada25552640dc672b24fbdbca104c499d24f74b7e458a5fa72823eab42a620b25a92dea1491c9c7ca41ae12be8c221d24b4b969635e4a" },
                { "ja", "8e2adc547d6360d4ab96428000451248f5ea9d356bac7b438a59afe10572a403475038246f8581503cec53e9def585750d50c6f83c63b208387cd8f0962ecfa7" },
                { "ka", "fc4f63fcf71465bdf51ef5eebb24a19513ed9c2b4e0c61431dca0706ec76a6b3f0424b4526fb49c05711e0dd062a134d4ad2aab9bfee7495105e9a0abaa6bcd0" },
                { "kab", "dcc8d01d48fb9e25a0b945fe2a3cc7c82d61905c85977501aaf5c31874e4038f157d3dd54968be44ff61664a02c619c5e9cad693436b5ac8b7439eedbf1b8046" },
                { "kk", "e538f55352ad91726f27579c3350bdbaab585570eaa551c09f6f118dbe3d8023d0e999b3521628e93698b6cfdbd7cf1f0723e3b03598d3b0fd0a152e04c76e91" },
                { "km", "1a5c0766f452c373636a469fd3712ecfb9710c563aeb2ee69d5602ad9e1fa60e908e9bfee4f135bd9d5ecc04d561118c80f9100c73a3f19780af1974892a2311" },
                { "kn", "b59c5054ae52099232b62615d767ae2f60d2e49a4bec4820e70053407f4c6b42f13a5f665e1379e7db5ad02875483b2c9bb57ca883f16aa35d8592f03ff56624" },
                { "ko", "83d59298e6d2d7d5138c8ab2f362bc93b733ce2814861fbe27e5647d68aded3c7cb287af546082db5ae1e990f7bf4fc4d1bb36702266effc852483d2bd31575c" },
                { "lij", "3e86717373835290e671a2d8101469aebd2eb16d99980c4cb6d0cac6064c46148010115b6eb3a815dbe955f641792d4872785576c17f3e3396d6b42de195e28d" },
                { "lt", "b229bbd1360595e254efc38b84414409ef52b1522f54758f75a29f8638ab1aa001d8adf38aa6305996d84f4a913d60139d0f50952741816eac6ef4b3b088a768" },
                { "lv", "ad44e356de9ed0ce093f623bd3a95fa94387664570b4fad6244234cba6d35dd82e96e3309b9dc04bbb7ceb70e4208b4a10d110e502c60feb88fa24d01d59ad57" },
                { "mk", "88731f2e2e0694e8b541a35023f724bd73a5c8f268dbe573a68cd680614520c9aae2f548e3c0277fac87797b9497e94a4fd120f4c3362a29afa0ac0f817f3e00" },
                { "mr", "4818be697b565fd1308d1212eb77521b5c378c419b00bd2b8c21e4dec94abfbf051dcff4bb5027089779a43d67bc0f779494b765ba078318ecc0ec2bb581702b" },
                { "ms", "fc39293ce8bae3324322f89772736a0c449c61fdf82b31122266152ad3de28079bd9ede7e19505686987a32d4e9337e9e744df2e3f69c81d47bb938c0cf8566d" },
                { "my", "c67915a3628f4f0291f13e566914d01e469ab966fe028a1cd8f0e84aee85c61891ab9be3b6b77c89577871276244070b19e26ef8a06a91e53945c53238c2c869" },
                { "nb-NO", "91e38b27fb610f73f8b644f134db4dd4ac1b7bf7379eae076a4090d2b3c0305feb3e410f72da9ef00222125a9d6b2ab4aff6a76f41934caab170adf42b9cdc63" },
                { "ne-NP", "92c474e041e44b04e9f6eec64f8c09f3e18edb521c7df18663137ffaad5c90ab4dbfb6a6132f1e8dd813ace5d77f29d92ee03cb1bf5fb06281d6dc647a242235" },
                { "nl", "84db1018e744725f09345352ffc4baa149ffe24653ba238e65046fd8a8b370c7e5044e3953dc952b2f347f1972d746ae04e5b3f0a958bda94ca15c3ceddf8646" },
                { "nn-NO", "9b334076a54572562cddf3a447a57aef8e8308d989891f29e793bfbb68f58368391807b6cee48028eaa36cf7663844668871948f90ff5aa55a301ca46a435820" },
                { "oc", "cd1ea92e350a7315444c9f41b71b212d00f94b1b1679622efbcf51c2b16136a07ce3723207ffd6aebc6c657d47fefa7a399c5c550b61b20c113dd887c91c6a61" },
                { "pa-IN", "a24bdb650d8aee9f3b9f91a77e52fbb9fefd1e4866a9ff84b0fe938f55894bfa5d10e2b6c18546c57da3f5e7248ed0e08e12dbb941fbf61492d24eed362c2bc4" },
                { "pl", "add305af6369b181f4061a99603f5a779e7a89bdac455e0efddb63f88e34b02e7b3a852210a64e242276e022059bbe19f8059630aac18440ff09a40296893bff" },
                { "pt-BR", "d82519867728b076fc8e6f0bca7c5290fba8fff3e8a02fd8136a9ed7988ae1601e526c82eef69d18b02d8219c07bcf5329ca5f182527d5b7653de27ea593aa08" },
                { "pt-PT", "804cdf1f364559260697f7e312a9ed31e7b88fe2e6faf9848ad3a78d4c2ce15d37c8d124584127db3c8b37fe17fbc1d3b039d29f34792b8449e2799fdff65385" },
                { "rm", "c367b272ff47d42445e810ff8917c9a2dbb6ab3aaac19fa7eead2cf490ee4ee475cb206b58bb92f84cc78a9fb651aca57e9c3c2a543cec4987f46d2a286f5f5d" },
                { "ro", "d3a2ec49fcf831eede6341a26898ae813580f0995c4a577a5afe20fdbf69bfdfb16ea7e40f035b8d122f89ce46805f9f3dd5278e1f1aaa045f2ec4c689c9cad8" },
                { "ru", "538ff2da68e083e890bd39c0185dfc492d0d97cf13f356a96ec3d2f2bffd9029fe881a63cb4f3a2f7698dadb35f65dde8ef300c8a5e86f8a791a79c97ae42df8" },
                { "sc", "cdf7f3490097ba45d07768b64419282f77e3704fb241040c933e7c60d3e29579077405cec417a44a910cd7110670df1f11b347b63d1c952e3de61a98702a8bb0" },
                { "sco", "d9c3b53079386d36e9ee6c91af98089f0698bd1e09e65b41780de21247e7b18b14cd1f9ab412ca4eb235a34cf6ef6eb38bc3be477e90fdf6f6e3cd410c1477dd" },
                { "si", "0ea0bfcee32b480658adc89caa07dec86349b19281f523a7738884abd4c095f8ceda6c0332bb581004a0125e7f076f3c9e0dcee1e5690ed13620fc934ac77adf" },
                { "sk", "d63974c21b2a47de19df35dd5791dc2ccb28fd01c4d2ad147d911efd0c9bab9f5f3b3875ecdfa30467ebf6c56d68442ee673750bfd992606f99af6cf78b29399" },
                { "sl", "744bb905ca91c767be5ddac5ee65bae824edae4608c9141eea1015a27609fa8e8ba232d2a8dceaed3b2547d3d1401e5470397dc6c2180084e9ff7ef25153a766" },
                { "son", "6e5137b11fa7f6c8c6b0a7d4bab5e42ed720deb16834df3fb2affad93415fe074222684630d37086e7aa1b7996ca520c1e14e07c967ceacde6f76af7dd9b574d" },
                { "sq", "94923bc68593eea454447dc60db0ce76488257240a2889a8d7f19f2c0936c4f6c2151e7e7c2117d1865a790643aec68af6577ebde92379d57917349ab6565c46" },
                { "sr", "b2ab0206ffe2cf2961c247c220f4c4b1e5ad7b4d086c5c2913577afc151b8f4ceebd5ba97951a67591d970da31ef303c1c6cb12664cf5eb81cbdce6eacb36386" },
                { "sv-SE", "cf730871408bc6115405268241e93a948cc4ddef89780461cd74226bf55df0d71645e7df870630306341979ae6d5accd2faf77cbf7e65323ea5c8c47e8287770" },
                { "szl", "d1b76508eb7f0c0938f3a068abe40b00a0c354f23b0147772fe335fbb746f5daa0f11813fffe79f7040913ba909defd564d2151904c2e1554c6dba42a341516a" },
                { "ta", "583e86d875cfaaf6f9e4df7359fd04012bcdc9174170870af63a355b78db352194f2636bd579e8e9f1e4750eda519e844c37462c48b3d9c347f0d74a7a6b6f3b" },
                { "te", "a7f6a070eaaeb6a65a5f40fb7760c2c0715c340cf08e57aad382f634d260c9e735533ea426849a6a306b24d134e498e975f8a32adbf4bbdc2168867293a7ba54" },
                { "tg", "71fb8d74891a47851a247fad9fdadf2181353818883943fe13ee1b91b8d0300b10f002ea7b631f1c59b7b9142725846217ae51af18951f0ef5d61389bb44be34" },
                { "th", "30ae969a3cd0fe9d040228895e070552ccf5df90cf4f70912c2c5b2c3b21595749a3a72aa5ee7c07fdaae2858ad3b1b56cf6e3945a82be33d0314413491b8226" },
                { "tl", "b980e719a9c984905a0500e5200d9c37b8d5f9e1e55da219fe5965a917eaef23e20e1d9044d578aba816cbafd29e3279a821c132bff7786927ebebbe42c9a357" },
                { "tr", "9a9d827dbbf54f804074f3f77deb3dd3d47e12a280077b8f25559e6db5f8cab869ba64673d7f21a00a8f7ae73770ab2ecfd4c71c27843ca5da6a42ac9983a984" },
                { "trs", "e246f1b9d02b9c20920b8e106c2f44c470e9d155bae1ecb3761732e9f082056eab669d554ec2f6134a0e2b5df60f230ecf2de917fa483044df1d3a47a6a85bb0" },
                { "uk", "cf75f21a9872720820c73241cc03a831b0e82c0609bd67360978e17be7bb18986c4f401ffd429bd1f867054a7dbd157087fe36ce05b39bbec0f154d3f939a962" },
                { "ur", "bec0965cf254ed3a6a7e3c3fb222a836a7ec370114aad08b887e4529587cca57c609da40da9b0a15ae6ca1eaa8f554b493601bf38edf8929089f6a7a4dd2aed2" },
                { "uz", "4f6eed22c667a2b39a70d17907638f734fa7945648b7bfea093ac9197094638e5d6ff817f2c6edc386bb6493de8c2344624f620ceb6439adfb0d7e49a746557b" },
                { "vi", "5b279054e588a4e49a329936becd9e99e7ea8c955fe14fef4fecd41002d3a6a975955b06a5022d7658cb3a48cf84f4d876fefb9a9f3b3e4f69702f5c820c2297" },
                { "xh", "95e41c199eabacfe64f343bbe1a8146c259c172197a49eb4a408689569bbc6839b0d4d6037362ec23fe295a92f6f8611a33429343a4a496f3a571d572f6ac91c" },
                { "zh-CN", "124fee9685dbaaaa2c627220dd7f03db8844a1cf435c8b3325397a610797727a5c898746040da41ed5e05f7d646a8ad19c1347114640792950dd052a39e05b7a" },
                { "zh-TW", "2e735c6a4f76471af6588e3f004f3d89ab9565def375c7bdb777c01ae3dce9f898f028a8f5a0e551f39f650581cc86edfe5c7f595b71e49a83814ac834b94e57" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/117.0b7/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "b9a64b8c3ddf3378610c2fef134a5ba56ea425acb5fd076f16c7e13c720c614c89c3b6e2466c33cbe65cda5575b036b852f1673bcec14449fb64cfd560296f9c" },
                { "af", "9c3ae20a909bd0390c8088af128ed8c2cd01cc51655e754953dc6d62f8ee2d485bbbffc59c8119ad29a1d2ce5999d2a4aab2aa0e7df2a14444e291f2eb93486b" },
                { "an", "b56f807f4d418df775d70ed0f86c12b0778f77bf7fb60e843196661e85f771cae8aca272b58057a4364c9bdde7ec6272b15bf3badec2b566ab62409c59e22131" },
                { "ar", "5dcae32d1e5d4c1adf12b039510fef75272278dec413b80596c2c2622778875a9f5b1329b640831902d1e2fd241e549731ba35062af70d03800dde0de297a2f6" },
                { "ast", "d6baf54a02973ed7b00aee05dd49a535ea4f408832c28e37a4136e7d6b1e998adbc6b645a322706cb08ce46b1ebb4d8f7510334ba36ed4a1e9cb65efc8509700" },
                { "az", "b4d301e7c8f66cea9200733f8fbf9a203e807ed8cf786268d84b38352f35d63dd82895496098dae23e3fabe741fdf996a7dbedd74175844e3af91c18f159f1fe" },
                { "be", "daecf5e244363b77d7c0bd8b7340e9fb4cbed9bc476fdbdd6e54d27c00c65866d00ed3be4a82c3f26f6852767004b506f3f51ff3a277266e1d1a22968451ecaf" },
                { "bg", "488454fd64cd0c94f6f2a4367e3938037c4fdc3a5f1724e7ff5bd3fdec1aad2519847bc8511e9ae70e3860c58f800d513b5833610341d5ddb4e8c2e1911cc31a" },
                { "bn", "2bad19ef59cc7390bb68a4376c9798f7082005c9bfe676be90e3560117463c2b0f3ca6bfc5f5afe5eb2f553dd0170449d9020501115aee1e452ac4fdbfc69f29" },
                { "br", "7f4ada8a312a120cfb67888ea13e9a1af0daac30c982c8daa7584509567d184dbe82a248cb8e925c242844e6f4db08651e186782b76d4c9b25898400e0d7c622" },
                { "bs", "22ec402b718a9531a280dbe4ee6d615ab20ce142654c6b067c4d2df7542963334cc241bd4c69aa5b05b1077625cafd3bf37a9e10ef2383df0544381751a6f646" },
                { "ca", "d2887a09c18a2b90b0ccbbfecef4e75846c675642e98099e753c2945ce63f8b27be2bb11246d02a17d4bba9356b91b9802531434260485cf5c885dca790a8dc4" },
                { "cak", "98720407d13857b9673bf1d615cfb7bfeaae1aadb0949ea467dd28befe6c5b932242b77e2e5396d868111c2679e3fa36b088b05cfc493348761e2a485d123582" },
                { "cs", "0953263c01bb815e2398d6f4e4d32d8664e8946244a38ec52303fdbe4cb4af7dad29e94056943df6981bfb687856dfa90198ce1ce57f1dd73112fe9d10164982" },
                { "cy", "7534f51209e45850a36abe21d14b0db892d34a498268597c23609c3681aac6924da5290acbb3c9022b69b2ae0a25d277e88c49665215cd7d907eabed0c4746a3" },
                { "da", "b75181e76fec4674aa0603f19ec4072823aca6ce732745c582db655e6a49e008f299c45c7a49cc6aa8c0091e2a280cc88676a8b52b6038ec49a59d0085af66de" },
                { "de", "afa6cb9ccbfe31fd179bfa638ef191cc7821ece03bbb4afd81f1daf47c3159263cc40f41d332782bf5a73ea606b4aab361eddc55e9e00afaa2dc0e267cf91578" },
                { "dsb", "d1400c250beeee5f16b04b250b678ffdf5a976ff6418e8612afe08e73ba3bc24721d68877288b0012bda735098f9a434bbafa33ac03d2ad9766ea727ad25139a" },
                { "el", "5f92dfb7acd2f761dade5aa614f6616cdbad78ff907484931ab26a232ec8932b1e33a94ab8618655608494fa706589a44f4e5c2901a66edf9f5da05def676e4a" },
                { "en-CA", "e1bde14ed5b6595b164766b47f978f7b270199475c3b682c698f2ac201a7a75a9dff59b4c1674634f6abd3b5efe5fe022138119b9d151c8deb82325b26f8293d" },
                { "en-GB", "7f67f97e3571f659340c85ea8a91f238c2a4984b6040d86c3e90c3a7e180f7c932320cacc2428b8ac087a5d043d802209053e0d67729110b1287f739f6922ef1" },
                { "en-US", "418bbaee1feb0b34c8cd6eb64516d886487fa010e1f143025b76bc57cd3aae0cd9227006227be292e3e225eca1cb1ca5fa93eeb2890dc13f6a11a93dfb836983" },
                { "eo", "abafe776ba741ad5e6f201660b052fbd9a9022110584ee0e319a22841a9cd4197245274b87056823de0472a83196306cd85ed6ab3f206afb98ba29a00350640c" },
                { "es-AR", "89c9b437e705cc541c60df22ab43f53acfc7da7fe835565a2efe41e54717cf52135a8e9cfa3dafcc904232e874cce3316e98fb34f85b77c354907f29341a9a36" },
                { "es-CL", "9a23687337494e6deb9b19de9040974cb9ec113af6373159d1e1b97ac83e6d59358ea657e4841b62466f88be553f97b10d8cb2cb4382ad11a69361c448765253" },
                { "es-ES", "920ef846767965015cee64ae0314fbe5734226d1e1db51fdf76073437a250a672b4df69e7ec35166e048a7656d8726c1955f7358da0d02e4b40a0167395c197c" },
                { "es-MX", "165fbf442d276fab67264e10699013967f12a34994dc52c787b9af3e700364b1931e513db297cd2eaf17f3c372bcfd44275acab8ec0a7345f9645f575920497c" },
                { "et", "f2a9ff8db1e703d4d6cff7eed9636c3b3cf5ad6edcc0744ceb05d886d4286a90235b0fb684bf4a4341c9ac6fca24b01023be92e33e7fed0d4ae03f54fd0a3f1b" },
                { "eu", "e90f866b8f578a9302fc1bd322a674be159ef610b27db2192ef5ffdcbe948e967c8c13e0bca9c37e20c2a9c687275bf5524efc57d68af08123259c79088fdab5" },
                { "fa", "cb50cd39b6b208dea01b11c3ed35ed4ddfec8c0d51dd23c431f0c3b34431f1b67921ac5c49bd031baa5f8620f5a2b6f450c25e93a3832a51f0bc158f0ec5e328" },
                { "ff", "d87c972efba73c98bc73b28fc4b2e6d894237bc0ab0e338a25894888f8c18a6049c805754ef63d253f11191005ca25e00f9711d5c58db694414ec77db3dc73de" },
                { "fi", "076dc8c8a3ba60ca268f86eb3f071cfad43740658b9a715de488fef97dafb8128c1b6e70af46361cc5fd7d78031be426a5c4aaf53b5769f7367c8ab7eeada3db" },
                { "fr", "74332681ee73282aabe5895ddddf45d47682845cca91fd2ff283344ef59f474dbe5da6e7d38c14f9268931f15642936bac348abead1c5890d4c8a036a9bac311" },
                { "fur", "eeea1683326af0290e3c73f21b53104355017bad56219cd7ceadfb2e0d5e59f7ea790c60a2878fb4698773672166233e4a43fc66b7172a49028255caf987ed1b" },
                { "fy-NL", "31b7a09d4ce7abe698c735fed2964446ac265c2fcd92a6e55baa1a48cf3fe3cfe9396cdf8d43c982b97a780046b3da50fbdd5e01b4b636681525c780cb3585ed" },
                { "ga-IE", "6018f83c99dbbca5e416952a58636ff803ad20e60ff8f1ceca7dd239d6e006446e0cbbb524037935710c4ff25dc982213d83665627814e0d9744743d64f40a88" },
                { "gd", "002f515f739f41c0ef8aa2b7d481b1618376396cf9b2ca64488958c6caaf25bfdf6a0bd2bedcfe2605aabfdad5e2e93312412aa1bcd1da1ddba40b5f1b798eeb" },
                { "gl", "dfed4fe695c97430cd389c1fd80774d919fa6043a3c7e696398df8b011c85b53373ae8448002350f64f6620fb8fa66d90e8904ddd539a48a85b40e4b80b533e4" },
                { "gn", "96cc8d99f8d0920157c8a95034cd7ab4cc28c84725d66cf5e239d191d7554cb349e9a22a8a483b3002b85d6e571e5756adaeecfc510d36cd11cded04f4879cd6" },
                { "gu-IN", "b4f479f83413112941f9959bc650eeea3960b1f95d5376c9fd1b669031aad5267f4bf9eb303c3be9a2fca64483a6927f154cdea33a8b1a041f046258714720fc" },
                { "he", "719df036a6bcbfbd218ff4806762585bfcad56ec1468735d18dafeb91f46b76a967daf46ffdba69e90b43c40efabaee3687bd536f234f7e4ca9ad8f28ec3d8fa" },
                { "hi-IN", "fa6dcf36d7c0f7aae799252dadffcca39cc09557032c09e0fb86c63ff3bbebc050559904b1ccbf707603060ca4fff9da9982a8db29217f6052115e3324c786d6" },
                { "hr", "ba6fcebbf23dd9bcb5785e2d7526d4c14791c8822b74c756ba74d15174bf145454db6c1d6ca46454bab7fccb5a168b33691f022a32242df211af4495042f4e15" },
                { "hsb", "69f54d366c592d1d5eecc171f5024d21453f9ca6bb2a513fbd90b1966e8bfd4df0af718e9a1aabc407f362b5ca44ddf8c34948f755c1ff9aafd4dacb8cb0c102" },
                { "hu", "a3912ecddb57aa00247efed9c4c6eb19d39b1dd08d0f8af595997b7b65e4a55e003080beae04aa8fa8f22661ee283c5bcea4cb37c0348868284b01d20baedd95" },
                { "hy-AM", "2bb771b97a39e0e64b5c0417f2648dfc5392aabed23afe41170c06d94dcde37d455ab2d1d5c22654283a4008659079158d6393c358f1a4cf12c7df2dbb5571b5" },
                { "ia", "9ae821f9e7a74b7d4cc75067ee714ddcce39453e90fdb67d87a9af9779ec97e8e22e78430160582069f5b481526295d8627c755b49789982dc1a12eb9ff5248c" },
                { "id", "fdb793d6860a07a0973b56d3d1421a945efe44f76047f48e0cd8f1ebb821ab49fcfd229595a9f1372628d1bb9fbeb676f298fe3d466fa8bc56449ed0242e1cd8" },
                { "is", "4169147aaf182983856383f68397bf7a43cd04da0e2ae03cd062b7f1b98805abe316f06208fc5848162269c4a3bf4ac1e70182de766f833bca48acec123cdf94" },
                { "it", "290f8350a0167c6f223f073c8cab915fbe4272f9291ccc1d33b7b2a4884d5efc80185d50232eaecb5657a3d13ecca4640d0eb863b7bbf85cf3a5e0fc6edd0eba" },
                { "ja", "faedc0462ee4018e420f8aab502d4de466ed02fcf4a57ce7a065bd38133bdfa2759808e99182919546860d4f106d39ccf38589849f73fb9ef2d56255a282e720" },
                { "ka", "710464bb299dcc999ada260b94e507546542ffd7bb9738d2bb22bd93ea960ebcc3e3bd413c9ce1ff22b1345dfa22dda0ac2ff623d5669e36f7792b962f0163a3" },
                { "kab", "a01db13ea1f860dfd37385a8e17994ab344db8b08822f8198181e8616cf5f5c69c1978f7f327c9c4d220b46290d41a794ea58e24564c27eb58a3d4dad73daf85" },
                { "kk", "d05bee2e94a9be3a0efb6b3a32892d5df2dbd844de2fd84f91a98ee400f3795be67dba43302ec5fb8b7acce45ff58d9df4f5ce789cc599387949e9bec9a87b64" },
                { "km", "44e66359c762cd8dff71d55e1e1b203dd7d33539b4219e05236e2e86e33c4af3d203fbf5f5b9ea0ffc9548158d31120836b61c3f3b3ee26c436a5185b572d69b" },
                { "kn", "85e8ef1ff7bbe852236ae452b34bfc6acea83ec12d5e9a82183c186259341f6cac68692f7e0d1e95d668cd0406cf78f740cc41dd0dbb7fc4a02c21091af80c1f" },
                { "ko", "176e87877befbdb631db169781c325573258a647747c953062b7937ca65b20a358b7f5b806796994fac13c47ec5aceed2f94408f59f664d5e3a8c91e80b3453f" },
                { "lij", "1ec25ce99f3cb9b2da5b66e6559d6fdee97acb3b6e75cb0ba97697e40c4c35be55fecfa7fc8c2a0d82ec2713f48d821e0a6943f1ddb4880aa83432b77a121820" },
                { "lt", "ecf798df909bcdaaf788d027e26ed823b79d22c3f103ddedc130042578d90ca7caae63eed8b60d217932728b8a4520897c7a10d8df6d6534fa22b60532e22c90" },
                { "lv", "2956aef143c124784ab56279929618919b3de178497a9f03d3907d3fefb4a3793416ee8f802b3d50cf06682e38bfb098fb15ddfb5b7a59b31b05e6a9e82f58ba" },
                { "mk", "badbb616c09f0e44a0a15bb678b08d02c9c347f541d9b010944c6b28453fea6efc88b9e84c83924d2c6e11253760f5f20735169ccac973cad208c119a8bc7c8f" },
                { "mr", "9cf9a2bde5c9ab3ab1a0d3f5bc04d1952cf2339ce9bcba6ef1890215b5a347695bab6a82a78fd968ed4be65fba3307b25b7058a768276174baefea594bc861f2" },
                { "ms", "cf64e946934394ab7a80bdb1f7e2ba02886db6c48807cab24482ab40952186d6137b8c5ce818dbc4b83287181027a9ffc8087ae293f1cfcc110d6a5f45bd780f" },
                { "my", "9d5a251db9db3ec4cf586b37205c88e92e07e0ce6666f0c8d706fbfcfc979d3570b95271436568659c02e9cdfc370f9f50a83ffe1820ae419025b5ca76be5cdd" },
                { "nb-NO", "3edb8fc71dca218d76ad3ec6317da49cacf72aed95833efcd518fe67a25e6a1f20c3543d892fc491d275bda6c081a0ca81cd2960500df740bb01aed344dc14de" },
                { "ne-NP", "da23a03b2d15701d6871a9b0cb5c96fbe71eefcf773500aaa7adbaa538d34dfbf17b587c39646c3e8a7e9d64a685d7e663868dfbe715ea4f60bd54e027b852cc" },
                { "nl", "01a84c46a05106a97f3312cea311d15361eed6d038436050a6745a3821ce66029df06e641fc952cd10efed897757a48d0d6754e8d37e1425334d98bbdca214e5" },
                { "nn-NO", "8be0047a42fa149ffc11bec4c0564c2ffeabc5008f6826bb41e09b965f6ff3b224972a541ed25f54575f9277582811e29a8a0a714f960474fe15933242bca4a3" },
                { "oc", "2a6154f12d75d8f26b627e21fd6636583d8dd1f8f894282d3e037f761b9f28852c948430857c707faeefa4811a0eb2d0acd63926b1295d068a526a2fc59f2d19" },
                { "pa-IN", "00a6973311598ae3f0616c4fe38f670d5de957b96cd57893f73b029d0fb15e72c126832f27672e4985284dd5a1fdd3eb4b652a8e262a535cf0473505f66289ef" },
                { "pl", "78738a7863c5da1c749d74a9ebecb9cd50630fb00d6612d897ca88a0db3a815c21a6af43b7157b443d867c7a0b4ac5627ee6ca0b60760e0914d4130ae9e07d8d" },
                { "pt-BR", "148288f4ec156263fcbd06dd842a3583a5f01e518591e3430704ef93516e1f901f98082a8fa8da260feb90f971e63b333040b16646ac74e0050df556ae2565fb" },
                { "pt-PT", "4cebb347d4d8e872285a8a5c945f7083a6c17a13b9343a3950ed941f1551175f37457975e84e16a7b49ce783bb0f08f03d85e8285ea5cc5eaf5a90b0b526f6dd" },
                { "rm", "8c701df3bdf851cd8fcf1657a5158a181642cd2aa5835c645fbbb00b0ca499680d99c60078c613e69c51384c04eb918124e10e1fe38d5792b1c4a8eadb2e01e6" },
                { "ro", "70594047486f597c8927e74dfd2d42b1f6f29c339822aa15d70cf0dba47ff42e0b26029d3219b733ecdbc7eadc0cdc227c0660da4ff2ff6842c88e2e93b4b164" },
                { "ru", "f2eeb0d30ccd5a901cf62c8895cef1724d9cd67abbf1ed4ef62f54f2bc6f08c0d78501630b8693384bb2a99116b93bcad48e21e7b43481c8e2a2f71a3f695a8a" },
                { "sc", "0974e142cf50683cfcddc742d6f50da434f506fdef3eccfcac8e24ffa936db1f9be685b8ae37511c59220f7cf7ebf438b25189fe3f684302248c49b4bc8b75f9" },
                { "sco", "28ddd9a8f46f0e0858a569cbdc8b858b09fd9d1fa3e8a35369253f153431f41f753d2a760f49381543f125ad7b6fb45555054b9bd06c0b657af30d34ddbab132" },
                { "si", "151e028b0c05038e8885f67eeae3a4a62ac5332ba275e6373174ef34f06a735c90d00d9f0b9f319e3208ed6abcb969b570689b1644a4258ff0a88a728b2b6423" },
                { "sk", "f4e6ec1984b5829439446352ca1579b2c3e0f257524e3ddb2787081cf9cf359b4d7f5432fb6abef5a7b779a4f9d7ffc13cd5de361d6b5b4e472385a8dbd7a7bd" },
                { "sl", "9938a5194bd99f7e9026c5e74659a370b4dc72f261c66538ef32cd144c43ed722941d0b54bd49092de49f7a60cc97ee3c73cb766f8659870b30570313c49f6b9" },
                { "son", "b63849f878350a167093c1a2dfc0fc77dfb7a6173b903297609e20ea608b03cd18ed7f04a7a784083877aab91c7f33734c7f1e12d54bfbe59fcab72d351f5176" },
                { "sq", "7f6c9188036bcc3c79be7e1c927257d70a7d740fa046e80491da3c73fee23f239df3c2b1cf5f80ef5b62985908d12f1b97ab2b14bfa6bec23bded6610bf8f6f2" },
                { "sr", "1b09848502b7a05f87f718ec88012cdbe49ff4dc1cc6f1c78c19c8ea4a7fc474a113d145b930011da8df17eb51e53f40404b4c6eca33f70c7ffb3829414d023f" },
                { "sv-SE", "eb241f9773940d8536a85f44453f3d9989126d8ea08c96634af67aa8c686e362f34f4cb688402e46e96ae8de3a8c4e9c22d6991a64baee32b601a44af2833fbc" },
                { "szl", "f56428eb70a92a518462ac73a2aec8b5cb838f191c77ec50b401c46316b95abb4317128128527faea6c5f09d90d416a8fad655d4d694ba6f1764c9a508be4b59" },
                { "ta", "ff88136addcb82ab6c78027e6ce965f7d4745758834da1998f551c6c21427faaf0dd92a5d4a040af0e6e9a28e8a09666864cb255b0b5d08476af773ab89cbc12" },
                { "te", "c9df98d4cf33b263c492ff43382bc36d2626d8f2eece40de3d34a52a14b771770fff1c1cfe66eeb0bf695ce370cdbb17a7fc85bc0fc4291ef317f3f9558f56e6" },
                { "tg", "d846782c14966db550470bd9256eabcf4aaa0c039168e263471395c286d85c6d2092eee0f4bbed92ca856a8a9253f2417a81ed41c89e2c31c3cf75d93ba7baeb" },
                { "th", "4648c4623be8c6f19849f2a73589a78f44d7cab374ffa8afbf09952e498027f0feb87807179bb694a17d63bdc0997e3736e9fb7ed40af0758c1714d2767d3f9d" },
                { "tl", "9cd38a64dae544d38a72683653fccc96ec2bf39310127447eebb0857356e064a8b98a309f1b72b4955eb90258465b10793bf0d07b4601c478818565e94eaaccb" },
                { "tr", "b0ead7e6a07b5252e5cb6e487c5ca0da15a3bb328dcb7a7286e8dcf80d1393070ca0c534617eb6ae3db393842c8801320491132ef384bfb5aaef52b7942ebd4b" },
                { "trs", "dce80c209b4501fd492392ffd5889a5380b53765f65949148c5404e66fa1b4f257be81e801f2e5afbbf26a46466383e20327e57a33607537bbecf71edbca55a8" },
                { "uk", "0d4f39e0a2732e775c849123327670f5562b085938ec6f9bcc635e3c41dec299ce556e18ace0718cabae1e58e2c356d7b07b7e634bc7dd905ee9fd0b50252350" },
                { "ur", "b84f6f2acf6ddbd525d41e6e298f3faa1dc4c32f714ef8a7f4b384be770cc040e77e2ccf341437bb3d0cfd64d3bcd0089594524e0c13372da3c2a24d309c2ce3" },
                { "uz", "331252e657c241241885924260914aa07279a9d86497283bf3fa1cd031e9eeb2bd6a9593ac931307be26ab29ee50b374ecb3a91519d76a9a6fef685857ca9c40" },
                { "vi", "6b598aa858341cd7270f19553281ae280712d6007cf4df665f674678c306da00903215203d1a8140e416b5cdacc96c12f24655ea74a9d81242a3c2e273ec0fca" },
                { "xh", "4cbe2c27ca938cfaaa652a8fd50e1840836387e36ad65adbf655ff6f3bed9d32b996b6cddef671d716289e6b4a54d6a09942dd4f78a9bb1656dd1a67819a0aba" },
                { "zh-CN", "da806c2cd108319d891303c0dc69b2070e3eae49c64b53756e80a5fe6f6131798c6235790bf2d4d2b22a10c16f8978ff5d1f0889b87e51a3690e5af2697fae6b" },
                { "zh-TW", "35a176d99176781abc9a571df8c8ca9d17d3c9d0cad4f33f7459a0dda85a4743d9906bc88d4905142367669df04721ca75e7f3dd01a6ed505126177a7a6b2578" }
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
        public static string determineNewestVersion()
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
