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
        private const string currentVersion = "115.0b3";

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
            // https://ftp.mozilla.org/pub/devedition/releases/115.0b3/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "9a88161e6585806e0d7460284c32c7dfe714debb9d39bb61916070a4840c14d517787c94ab5e31eb4bb919fe4b16e474e2a8787837754e816d9105947c9d3269" },
                { "af", "9a1cf8f7d6b5af2719afdb938d95d7dd0cb87dc461b7b5651957eee94b46aa3cb2adb33c988238a3be3aea653bad053d9ead103322268383520bd36f4cf6a88a" },
                { "an", "dc037ca3fecc950a7912430de096d4c6807dd9a1c06e0b20fb3b88a77e8847cef6f2965ae035bb3314bcb50e19b1a0121ab8caad217f633d1bdfdac2ce24dd0e" },
                { "ar", "648da77bc56ac935c791f85ddc26a1f217286fc2d552671d8a34c7664e976d30585d2312a4e27a0971f98de2e02a3290b271672b3a6553add964a5408b88bb70" },
                { "ast", "0fe582949077cea19411c5cf5ca9a2f7c1f2f95c990dc32e4e90397006bc8b0f3f68c62a737c54fa8f4723f6166a87c292b682fa1f075d5d5d0461cf7239ea61" },
                { "az", "3f24a45e999a1a68455b24637cf59eeea48ace7ea0a59cb9567b97eab456396352ea98ab3a6d302c6f3ae235bb0aa728794f41cefbb5fc4b5937190a714bd4cd" },
                { "be", "1d3d3db7dfd51707fac692cad178d2e96fca1c11f1c4bff13f97f25ea0b00166bc92e6b24cbc836a1a4d00b4de1c8697265ffe987ebd1b537acb6d82ff64e3b7" },
                { "bg", "fe618cb02dc8e84eba0dd9d1ea1fb2d4716839f8221691e2b5f02e8063391b8ce2599f14b4c37dd7366fced205b3e74a420085d71becdbd0a9b03b294efc33c1" },
                { "bn", "4f2d0a4969aa4eb340670b7917243c7a2d2b507e9ed87c5871b782b9f15b3363ba75f6263f1577fef0edfc2cc23c915236a504d13f9d1b5636d0539bf98c2e5c" },
                { "br", "9f4954ebc262e82ecee174cf128033ba5546dfdd8b7e3d4a4bad0c9df5812291c087399b4b9ae7d0bc8c5d1f1b87d3ef3a1438734417b1fa92e82b9a8c8d19a0" },
                { "bs", "debab5d011b2dc4d6f7d8b5d1ee71eaaa6748fb9a016aab613b68fc660a8af2cb67ae7b33373a48d48c0399777fa8df06fbcff8ef3d83c5c5f153b33573bcb85" },
                { "ca", "5ac0f83e0db91a73bb960b07e4d2e2ff5435cf8463d08bf736c2e626e537ed66cb66a88b3aa35b4e449ab2a79e598b28f5bd71ac861435726cfa3dad7fae3f5e" },
                { "cak", "a94ccceb2a5f936adf1d45579ec221c8cd731544cc45501ef8d33ff837b60e873332b593413cb1bfdfcbb024422165e01d92a2c8194d40d3f01c4e5a1623c130" },
                { "cs", "ea796914f3d0beba5a66e176767835e209a4931796c2514c95e2ad73d6c25d1e2a64fba041af05fcf583a6b499cc24e8f4b4e40f9277c688e6d078f23e204069" },
                { "cy", "194cc88070eec458a5159dd3792fc10c16374a346ffa3c0510af34a4de254bd8e2bf63d80ce3896bf000508171b5f9307f650c27af2dcb24f5cbde719053cade" },
                { "da", "67326cd16900c859fb37418f2a3387aacefe7b817633f4589457e7daef376d08ba4b40528e5d7b1f060214ad4cf1d3a77feb8f6f97f9aa255d7a39d9d34dcabb" },
                { "de", "50b4636c14924c3908278a4ccd6d7a84ba4fee852fbba65a1e0c526b16a2b411244d060eb4a9c5ed97b443aea460d6770f290ae6766d6983a31fb63b68096bbb" },
                { "dsb", "bb49ae4d2295bdd4aea6ce891370586ad51651ee8e0f73d985ac6e33fe124b5b8b24b04704c1d594bd5ed88c4d0acdb79fb9797ae11aba799e92b1693bef7b1a" },
                { "el", "fee3bc8d2855453c9ad79b393aeaa5b07b41388605c4491113905156961ca92fc20e876439fd7f64d216632e8c68cdc2fdf1443ceb9e0d2a6136b2fed50b8d21" },
                { "en-CA", "e912e1f7981939ee4621189015de5ace631870c432c5bc3b9c4ede79b01e2cd93e582b90a3865ce482cbd66eb73f97b5f7f23a99089eff69f340e07231585673" },
                { "en-GB", "c6a4646e88ffdfd073e432565fbf3b61e0c49d0ef8c83664d71eb38b795e0d466754139c80654e5355498897a1cddb456c2d254f75a3edcbc3933ba3f1819ede" },
                { "en-US", "8f6cb84845b18d171677d5d235720f980aa8e87138da5b2ced89d89ec47e9d89fb20c5e5ec6dc2419eeb1051372b9e1fe282a802d8bf4c291a4adfb4b8a5b6d8" },
                { "eo", "dce907192ce97330dde979c72a804adff375d1247a39506e8e34be01b10b3f5ad38100cdeaab1def8526d7d853a444699576d40a7f6700fe7f4ef1a641470554" },
                { "es-AR", "447f2e973903fc2630a2273c1e8ba25a85037edd1b9d3289820c2adf8a31714aea10adec9a31047510676519389bc130f10b333eea1bb0c44724141300e359ec" },
                { "es-CL", "31f2bf280dc6f8ba21d722b04db0b9928639f7f5671c186c97df370ffa5e5b35c54702ce901b0015363353cdefa97ab561c7f3e1b51134261f1bf7d51d9d14ae" },
                { "es-ES", "82acc03bb8dcbaa573f94235c59a93e1b9c75fb4694ddea9df642d0345f9324f0140f80bcbc64c6044acd8599abfcf10561f68379874623d25bb511f43dd590f" },
                { "es-MX", "a89e5488478e002df3e2687abfa2d1c71d02a81d6b5e0905f01b978ef251c2d64e99b527580918ed4c8beba8a905e68df659c0fbad85fcc2a7638893d25588b3" },
                { "et", "709752c85503dea87b3c30c300784c8c861e3ac7a301a98fea13710793f008d5fa46a745d7b44685c1defd23dd7c754e9b3847feed512b6233f8f3ddd6a88660" },
                { "eu", "618afd258da5d0a6c3f9a2bc6b9101e00e8d3db889dbdfe8567503b6485d1261fd10ee2fe09cd57925e059f07ea7decdd48a8b980c09613cb25c76984033a8ae" },
                { "fa", "d90083723278d9871a6352eefe268081c5f0f23683d38311fddd2b52f63370ec9314fa494e33a8eee39ee3457c41df673da33cffe494aac66e3a13ade318a286" },
                { "ff", "0d7592c238e692ba4d1b9719f31b426d4b3bae080f24c81cde5ae62179d9708ec594e739983689e465330f5db9db256b2b9196e9fb0d78c0668a5779d9c4bbb8" },
                { "fi", "ff1ca669c2f3df704306885d8bba85405df311f77e64901c143a45cef9dd623515bade47d415bd712ea7af83ea23efaa4d3b59e1764bd02a3c00e24f985caae5" },
                { "fr", "9fa60c53c8924b0563f6cabb3eb9b5f54798a0f6e90ad3af069fccac5b9dd06029e08a3963df03faaf6e9fbfbef59bc4b76a5390ded529425a12437c65d8d135" },
                { "fur", "4a0a45ab112a40c9198e5ed1cdf0757bbd6fd3cde369e161a09a54487af310262885b084e2c8efac2d74571080e95a9c8cbfcf9df8869396851baa441fb6e192" },
                { "fy-NL", "a0f11bfce45047615b096286c14228616fd3bc6d7ba46a61be7b4429181d499f4a63485ef709ba19e5c802e6bcdf6583b359886381648ce53fe1bd6573356137" },
                { "ga-IE", "59cb05028516cadf22735167d4cde9e6d764c6640913cdeb67d571dd420dfbf5db2a3dd5b08dcafa45cb40b3556e4ade307730e17b412249fe4014e02301ca0a" },
                { "gd", "1297036d773bd66a47f11de12557833ce94fc8ae4db8aa25a2d5ba19e7facbf456f19b31f38c7f450901062b919c3fe4b3e8bd7a55e57dc5916f2aec510c6a58" },
                { "gl", "f4eb35efd198b6ad563a284ec38229de4e5b7643238b491aa800694565f303cbf969d9f2eb4aa0070e75798a360b07ab4680d77add36825c42999abd86c82253" },
                { "gn", "8d94ce405bc6245eefd4ead2a8cbf6aa544944e0454132465bbc7f31f6b8cce1e59048371e10b87fcab7373fb089c028050f3b06622248212bf03b93db12688f" },
                { "gu-IN", "b5b8943e193fef2235053f867708d2c511f27be8c396d18f7a47fc6090feaa3f5b54ec591e72de67304b833d3e7b50b2f9c4083e926ebfa56acd2ad05b5208ad" },
                { "he", "7eb28583369a2d22468f8e8c8dc8c1b9b465d3f15a78f625d7f570b59233842185481436ea4e99ee2a84c53aace6be83f3cc8f907ed0d8e4978dd72fee34d095" },
                { "hi-IN", "a8dbb76deda7dcfc27171072e36a1d78a5b94285c04bf27530bbb5f416bca3d9da9b3c9704cf965540e492243f5912dd904c5ed6621773db7aa1227014e00d61" },
                { "hr", "eb8dd3ef025c7dde9c9eae1a8f208e4c45b273ed248c10801b6903af167ed13b46666e519bff29ac290305864c3e14869981f5940c10add055a5262fbd0b5dcd" },
                { "hsb", "922851cf953e591f7078b40fe16c3c14129930730b3747acb4b140d0e49962ecf22554bd3ac05581303e294bd60c043a86ee7a3e687cc70af739d1bd88e01c2c" },
                { "hu", "ef35e29aa384a305bc11bcb1dadabc4224297401bbc492ddecd772e7b8d4fa860f72fa9e7487dfeeae6b9cbb830940950a6649ddc79f62089c2f6ff83691f726" },
                { "hy-AM", "4dd43b17416822ac7819a3381281d8fd5a0ef39b535128ae487cc6d4a409bf728cf00afa7ee002f65b444f30d039262c089ac20d54fb5933d0f9beb60c71e735" },
                { "ia", "6973be438aaaa9d1f9b8b48abef0a5b20b19d4721c442d4353dab85a77245f954c1d099d1fbde012357ba27e99969ef14d3d66ea6c691118098c6c7dd60cc256" },
                { "id", "0ec0219fc4aba92ae9985e4823eeaa1993c06aeb68b258977b8195124f8bc0bc0b6cb73259dfc93633888b2c74c3c5d21c7191b8199faaed7875186a23609fa9" },
                { "is", "a2757dea8d5d32ad0a5124512d5aa81bb6e461bbce5bdb8e8d9d4a0ba4d4a014e1151856188fdfffd1daabca19352f6b0c4553f7aaf0447f12f8bdf19dafe118" },
                { "it", "1ab52f3e03e0c007373c2de7cf6e8c8b9e515095489320a10d8837927a45715d9eccfea87210da9765937500a23bd5342c0e6b439e1d4d26d0934c7ad5e872cb" },
                { "ja", "0ab480e63420c76bdb3c0565cc3205a93e62122d5566d8b5682c1de142e202a3f5fc0cea2bb467af4302a15f3c5cbfaa88cc4ef6824c64d94b4dd9fcbafb088d" },
                { "ka", "71d8031095159d77594f3f85dc2b6a49793151ca206a3a5bb41c95919ebeaa51eb4dd202a24d0ade38125ae559cd367fdb2e70ea53aa1c27c5f9f32479eb06da" },
                { "kab", "448bd047098f42e01fd61e23609824ac665dcd55ae1d26621f2b50caa1447e5716a305e1294ff691c8b2c207d2e9e22dcdcfda8ba33608ea4f9e558668b32b2d" },
                { "kk", "574358976a8bee0d73ab0401b3ef376f043a2e70dcc890d3ce2b33e7f916c79b222e00ab02f10bd5c432127b500a213e479a79db7b9d55130d45e3372cce21ad" },
                { "km", "447374c283e6de786f6f6552e46ac500a963e4a777e130100c7d22305405ea017a02bd365f2efa115a7c6c6e52eb5d8b39d0da91a9be9a6f9637e8064b5822b5" },
                { "kn", "fa7d59f66cc0c840fc5e94309ff26c4be6ad0a2b552baa788eb1e8865834bf968a3b2c9ac8091c2e3b3e26ec8a330a7891870ea6911ca540155a7bb5663e4252" },
                { "ko", "8584a85266b3d4de9655e963d31c5742745a4a1df70fc07afaf41bad1dc8846e4119a1e0867c43f4dc7caf6a30f631d5cbbf9f2a44114f22382131b58151ba64" },
                { "lij", "9db1c49a743bc8c8c7f36e6163de3e9cd01a4331be41248f043c3cc56f9a0f75683082b227a488d9281592da61dc1c24c5e7ea0266cf03fceecf1a02c6b86f07" },
                { "lt", "c3e08e99a168a50e4f08c8118b4974885aa91061a320a1802cd5e4dfbff47a77eed0d00e7ed4c12d5b9d4e9891c7174fd26da7c85c284659f7f616df7ef93261" },
                { "lv", "51069af82a2947cdf0f250c3120f2a7edde801a0093cfdf3ee3b1563c7b347750e439a4d682576aaa54d886a55e7e9493fcd7f8484e4f1940400b8c6bd2ba32d" },
                { "mk", "da80558343c7278a7b46f01a4376e03bbf1c386dfc96d922f8b75e8bf72698273bb088859b1e0aa8ec8d0026294767127d6dd093aa434df850303a99e99f4959" },
                { "mr", "4853b63be73e1078f8eea348a3e1f7433c222ef34aa0fbb3ad11d68805ab6aa5018192e17b09df2196be9fe894f9b8fef43cc2e35bf15df708afe9318fc6848c" },
                { "ms", "36ecb36f6f5f6a2e0de15d4e03d2c444655d56979e1b1b4d48d4f4b46dfd53864805dc91c09f286c524c6aaffa82b0aeac1ac7fba34820b0003043af191883d9" },
                { "my", "4608110e8e8e5ec410126fa77e685cd43ae6c79ae800dd9bd566f2f11bb25707cd27b92030b923b4b5a41d25f3220a02f6065e9c90e01a529082d1a3d6728c9a" },
                { "nb-NO", "17a4685e2832e7e78ead6d3148fec4b1e963ca76066c633d06fe54bfcc90d6c27ddd9420be506b8e6a09cdb686de866668e1877062acaad75693fc77b8a7e94c" },
                { "ne-NP", "01a22ed02fb6fce04f0094fb59f5f4fd57658a6cc0a49def001f4ed30d92539ed8ec2f94c26d9bb0f957aba218a0af2e43a634bb0dcbf42585c4e27a8a305b84" },
                { "nl", "df8d08fc3408eaad4d1731fa9a9d40e1a4fb8c500df3d9982faf3e19494f9624e2b021297c48c64b6b7bab225fac77b889997f2fcc96c8ad3bbd837547751153" },
                { "nn-NO", "7c4ecf0a84f4f291d3a074fad9caa510f09eff81c896200ce46a56cbfdb004363f0da3716f86b7ef57178c004acf65b1f5f27e0f6f7a5c718c3ed1e3a34b2967" },
                { "oc", "54b72af2b0f4a907ee15aafae1647d781e03cafa17af59ed0846510a93596e26f533597d2a143786ea969361ca590b3dc29f1e05ec8422cc18fe76304e388a84" },
                { "pa-IN", "8dbe0c79be46f54c6cb3a53d49981e3b385d2fad089482316b420386c48957cbf9e5d3f86318d0048d390a50d7948d568223e1c23e3feabf179d41cfbd5f3e76" },
                { "pl", "9df3457378287534a2ce977a67b91fe0dd7776a43a653daac7e9e4ba1b74d067c8a8e941a16e41e0d2890452abd507b2e085b2a7ce81ea6a439e93cae406d7e7" },
                { "pt-BR", "b6b35d0e41834f2e5a8dc905b6b1d1db4118fe630bf845e1c45656667b7e2aec00475e174f017f47068bad3cdbbb15576c498dbe8d2cc5773cf2e859d4e680b2" },
                { "pt-PT", "daba0b33622166a3cfa8b0adc7a213d91452414664ea5313463bdd10f412f688f2255f1685874f025395941ba052eb9f7ff69ea64ae3557b9e367aed4854a561" },
                { "rm", "4c2054f2201aa517ed7de1a8095285d756672ed00825821f7a38e23b6f4c1b8a90dfeab48f24994fb777a93925c8c3c9eb419bf390240a32612b6d6235d5f29d" },
                { "ro", "340e4d8b9edfd7b333b94b34dd369221cfa671dafa4bd5049d27fad1e2fb9415758c91f566a3ce700d61e6886aae87a4d4ed1d220169de1c1d6fc8fba2393b4a" },
                { "ru", "41ccda1418efba794d8485f1070cfc92dddf7277c3fed1c16aa47b1df3937138f44997bf583aa00907cdea39b99d829b86f81eff46506abb9d070f0c5eaee1d1" },
                { "sc", "1e3ca32ee9d113fdca06959bbf8a7cac98ee960109dcaa8b58d839b4a20900a33b5403053ea5a638888654b84dc706edc4358025fa909334320d59bfab1f767e" },
                { "sco", "8db21fc72e8a48ff0aa92ce3dfdad1ada3963b99d562723a52c59590c136dab163f343adf831d84ec3c85a8ed0d02a51441febb71d46b7f4e8d9363920ee33af" },
                { "si", "46b2820a7371815d84e6225230c18987de6e395d76a9b9f21e2b48bb202659da2b031ac41f6cf8aedbbd8db3ae859f84eb37ddd566d9c71ebbd0ae798fae904c" },
                { "sk", "737fe594a302e308a7729f06d8c63be1ef449baf19923621c644659bde500ce4b5af5f48c126a6dd1a3d3372e75cf3ae4a61272152b1e3c5f36020cd8633e148" },
                { "sl", "08e5ad1fc3d11da60a395cace6ee62fe173ebd1b76cba49edc6b4e4e341d65fd748e823f766ff7899887de80c8399c79f7086bd9629aad7b52d89d1eb05f7add" },
                { "son", "8c10bc25801829c37a5515c217db72555bb5d3488b9a5146439538443b7aab3e31aea669ac14dc68b75b3ad2fb5f5be1182ed02d800d935e905fc3ffff950803" },
                { "sq", "7c9bc31eea8eb4ad34190deb0ff90215a32982a962121aaf42a3184446116fcd02880900b54ff50e76da7f38b5853b34d819ad98217b3413cfd01c386fb5570f" },
                { "sr", "a2d44026ace8f1437f52b71972e229c75c9b64551af6129e63eaffbac661036af7204d9db9c0820d68b34533e3a960c27a03b7a2490faf8d0070ff4668014039" },
                { "sv-SE", "28de77b47af2ba694adfe381496372584a39765f911314c9a720f6783b51613a460081d5a5300010e70b4484eb885742236f9111461138e69182d4060057ea28" },
                { "szl", "6de7b79de971d2d8f6cc678de41aff6a2893b83615951a8c98f2885bd4e1877bfcfa2f069708b8516845a1f756600871445e96ab72a72d25ec2d9b6cdf7b7f75" },
                { "ta", "80e02245918b0188c261343734ff19ab22a6ba85506c270e154f8431c6727e32e82276798f1659ccf4a6a4822bc3d7eee057b4f1ebf8e19f8132a19868b9f66e" },
                { "te", "9a0cb2c3b100f75e2600f6587f34d70bd66bbbf8c1dc1ce3978341e47c44a7fa1e90f6b20dae2be7f2777c89831b1104228f343d679821504e924099e49a53fd" },
                { "tg", "8dfd8504d8ac3872b54ec4f04b60662b0d79a36ee781211fbdc6c8f9b5acb692ffbd5c006a6b52d079739beac4ad1a2b257396a4e5f7e9092bd1d147553b003b" },
                { "th", "df17f120581b9d1398777e900136b45b5cd547170f99745aedc0e0cc545ed15b329083549485d86eead1ad1b87c5d2c4035fc11ad77012ffe46df66159639b6c" },
                { "tl", "9918599613fccc19c37b94e8070c15627bf1aac5d40bf20331ef84fbc1221e0f80fbae4e7b29bab6ab89226c937a90d8920f71880b72c6ab4c7dbe787dce02c9" },
                { "tr", "b4958ad59491370c7864b8e302ff2763b60d05fafe29463efab32c09aa490d7befb7bd2ace7f3c1356d9ec3fa75622b374493b4dae8c020d2b482341e5812c48" },
                { "trs", "ef74f1724a620375981e5f64d613ed1b227d0fb965b1faad3e690f70ab424212d515fa34c9a1bf975faf1b0ce09a1e0b8afba8c90c28410cde92a76aab04e8cd" },
                { "uk", "f036b09b98b4ec41082ab319484d0230495e35989a0f31b5625a3cde1f2a7069047512a8ccbe0b35d700e9664d24b30ca19590110a765eb69b654705362cb620" },
                { "ur", "0bb25b6b1785c2170369d906336cd0122d99b4dea86b1beea3d17a2e81626173b16d3728617c0137c96f5c22f790af1cfcd45d5b0d9669be6e7dcff730398a29" },
                { "uz", "2da8f4469c6b21cbcc3910c01d049fd6ac873cb5158dcb1a62d7812188eff51d11efc6c7935280fb8c00b2c40e03419a6cc0d1d4e0e01e9c3531d444aae376ee" },
                { "vi", "6b86849b559c868624411c152d33f895f32ef9310ec0b62ddd661cc3b29f5ff633a351ae7829a4133ad508c193cee86a74ca81d79fe41c43378b055317824b79" },
                { "xh", "9bf14ef3f6c207de1aadf047dd06bba2fc6e35b5dee669aab9dd0465bb187ad9c9f744ae4cf3305c26c3fa3e43bf96a9aafbd65a5fa22ecc0662fc8657a5ab50" },
                { "zh-CN", "9f5fa5def5bda986761af562ec65c22aafa138af14952fddcd1ba46ea4f972fd31f44dcd93b0663d73af20ccfa640a61192ab641db7f6b5e782eff6a6a5f2853" },
                { "zh-TW", "87e7bdace96f2d069f539885a4e89085939075b8dbe4bfe863acaa0ae5dac1627774ca323b3a151a667e359da4f720c423dceeb8a880988731c3fedeaabe9967" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/115.0b3/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "d618ad3e44bd465e16a9b183585579bfe24ed87554879de8ab73f78e406df8f3e71bd360a0653b350a133650ae5d53cb68d893a2d55da8a786e57bf3f8905791" },
                { "af", "d0619833ff9a4c64ddb575567b8745e87669da3136f3e041ddd12bb0554b48a684214938eec6259f275334737198371bcc46174fed112a961bbcb54defa5e654" },
                { "an", "f81672cdf2b21e8b9e4d27483ebf06a6820a41bc9d07ffb1fcc8674e17343ca374c1ab00ca5deb55661d3f6116c303ebb20f203c7c28d9a010190fa9109bc736" },
                { "ar", "c3cead535c8a904f1c8d75a3647a8901650a666391d3dff4963f811f98321d267aa22afdaa7ba465b8b2ecc7b017d69105186abb5911424e1f4c86626f70ffcf" },
                { "ast", "a28e2d6dd0ca5fd9a921278208e51314dc09dc3bae08148b6dbb39d65a8bd6c1eb8f7f9bff9bf0502075ab734082d075e822f8458c9afe407deee33cc6d1ca2b" },
                { "az", "9d2055338b186dd2568ada1591518f408737e524c20415a7ae5c26cb13352d564c4a4ed86bc14e7dfab260c62a39993ddd21e211b2b7f52ed1c8c86a1c46d358" },
                { "be", "5a169995589b77f3871084c2e77d375bd213856987291908b39e58b4a327f74bc3a060867fdfa3c6c3627f6cc8b6d6ef6a32806a80966ea1c3fbb62dff8bf47b" },
                { "bg", "b5fe4dcb242c2d63282a9d37c49f7f7b66631afbeb8c4d3d468a9b1ca323cd1e9a75b966a8edf540576fa0c47c9cbe4455cc8771a572bce58e82b75280873f65" },
                { "bn", "738edb475544459266e7edfb03464abe9b80af945566c7f80a305fa374f1d88d19e4f286597da80022d41d74e1f92f60a82a5d4f4a2da8c1042390b02c964669" },
                { "br", "3814827b2b0f143aa01ccf7e609f2f481e888f0fa6b2f4cb0f36eb10238d7b54ddae4b078cde2b815f2e03c0f5afc3000cacc6226ba2fcf8acc759034fdfb6d4" },
                { "bs", "084bd63048681bd6a27355b2f1cee2cdfb74ecf00818bb1b1a4f99a0ce8662c100d21529bad391b85af9092e409bf00989f5159b44db164edbc1e5549dced73a" },
                { "ca", "e6b30c0619ba8fc181efd0c66606fd885ef464702b61057959de8ea4e45e2b1ca19536dd8de834fd12e8a7bde476b9cccd813219ec1aec5065b28bc901637a0a" },
                { "cak", "40d3b085a4ad22ee9bd05dc62d146daac7c00ba138301152fad671403cf2cc46a6583855beecca1074bc043dcf8d4dd240d708c6f1b91693ea2b0f13ae7a7618" },
                { "cs", "ed2f8cb90d2ca569b30573b59f4d380647cb99a842a0aada4737d3fc836cf0a80de9d0935516a5cfeb146b667b55f1807073f79dc19048933fc6567764dd38fd" },
                { "cy", "5d43256abc5b8dd0d3eb4290e8db5c5b6d441637ff965fcc91ef7b1bb40af5dde0a7fe6415cf5714ec590857882acbe68dbecf909f725cf5daf85e1181f4572d" },
                { "da", "576339591fa388f0240c022f4f601c3ec70198a0ba252659f1815f8d54d985bbbf762ac2ad4d4517360a79cd9ef36ed9fc632763f831f2b9c5f7c754289256fb" },
                { "de", "1dd1e305889d57d9cfebbe65dd804d0f4bde86abb99e439f613d02841181f0bc80baa930a681cbd213df1da11ac2b39b578fac2aa80b975049a0eb2a637e77ca" },
                { "dsb", "24d379b0636dc719fb057b404e00376395f4e9eaa7e0c39af6cbe772ce2a3b3231cd6385dd8faa3183f2c8cfa2fa43e388b654f6291ee4c548af1c96efbb5a8b" },
                { "el", "566d5de2ad0b934f98de78971b66352a38d5ac26ec6046b9d36d8a8b2678eff0af8e1e7043f0a26503622b3fd16b55a9405327b5681f5fa35b0345b281edd670" },
                { "en-CA", "09de7bcb2968fc6fca8f7cc0cdbeb75dca64901a20b3d2869d4dc3916700d59fc859fb3ca717aa9e3c534c45cba37b426fd9753c53e1457db5c8f775a96ee51c" },
                { "en-GB", "cacedfef11b4d8a77682e65c33cf6ed8b90c028bba9231e089f52b6dcb2d513436b3947ee4cb4bad248deece5384319bfbccac8b11060eb97b5e745589ce817d" },
                { "en-US", "0d6649d39cf95cfa56b572b3cdacb80467285c21f9f9d29cc1875be771e2c99dbf4db91e07146438c7a593b3b5ca9b8af71e60a71bd1041b90610e49801e30f3" },
                { "eo", "eff7c655e00e8d325ed8984983bf70318fcc03990edab766448b64f06180db1e1cc8042f666ec8d46f706dacecdfc85a30a302a53732989bbc7e0738f71c5157" },
                { "es-AR", "06ed8c269c677a8fede12fca7695f4215b5d989e0cce9eeea13d53708c3b29c69f2a715278442c80b849ef4900ecfe7cc43b0f738816ced65f0784ff686aec93" },
                { "es-CL", "cdd0849903c8d6099d504d6f6aeb10532f0960ac537993d6eb0e5c61d5c5fc47b93968c946f9ef1133ec8b286821f8d13e2e86530f3d0363d4351d19098b72f8" },
                { "es-ES", "b5546e009b6818eb7450db8c07d4a211507f9562f440952e2075239d11b6d86c55278a04273c70dcd4e2812fa706e940e2282a0fb20d4dc19d6f43a40df989e0" },
                { "es-MX", "9ae2bec55816eadf6fa655ef7b7105c0afdafda2a12598ecaad3cceea351f330dd34fea0fdf60c1e24a47fca6461b1e0bad83655c20ed90648d25b58787d03d9" },
                { "et", "c990548070d34b5946dd053441d791e8edc080b0978266ee987f5968ce3aef45ec72a242dff8a6fb399af014fc4d1a493f01e028183f89a773b4410a49b9a86b" },
                { "eu", "1e1ead093f08272a30f4eb4bd8ab87d6eebb2e0fcfb14b3778ac4d666c9cb3a6593231b54d64981c9b450508554a936b038ee2e1ea8d7b2bc22a04f4de4198de" },
                { "fa", "1ecea5c3382abbb6867fe33ee904499d150981ecf9d712860e5cd99ca4be413450f6e1e6f8c1679ae098e46b707363bd0653b57d6de9bd3dcbc225c0aba13531" },
                { "ff", "5169c17360c607ac60ab6357473a49bf6e97db7161b585f791b492fa0a7a4b6d9d486c17ce2854a5c3c796e821f1473107a6479e392c2d9632afa8aed9ee4514" },
                { "fi", "0e70930ddd0b60976ef05e30eab22d08d04309ccbd943b36cb05e23daee2b40b5a337f59a99914f8101b5f440dbffb1c4ba1fc18d6d0163c5e6471e1208bd71a" },
                { "fr", "963b585a77d2e76061be923ec77dd4415bca3f8e8dff907249f0cedfa73097d0129450288f56e7cfb7282fc8390a6589fb281aecf5d0590b350350d495642d7d" },
                { "fur", "2e60c2172c7aec5699b0ecb077862787cd89f45102404da85e9f91cc1d507e44c47506db9733c63fe99c396b910d96e1bb2681443bd556f71a3ce5b7dc8d787a" },
                { "fy-NL", "6b58495fd2b42408cb3bcc453679875c0577b5f26108839f82b4b09d6161a4568133e8b1b80eef8419be3570797da623464926dab7b0e9aa65eea99807e50d5b" },
                { "ga-IE", "970191fe184e2eca4f2718e931db274f61c3a8d82d93db055b7057f11f7e9e7938fb534356d594a356bca7a9044cc8a0e5dd7cd39712e4d93628c3d8a0013af6" },
                { "gd", "e8701e3cc5d089512b336fd73ebd3e54eea58046f3ebf1829e3c1b2e1591792b029a6cd609fb1a9da4118d1057b85b640c087095ab2d3ff06faedcaa7cf6b727" },
                { "gl", "f95dbdf7a186db0782ae59fa0e8b3dae2fd2b4293f526c2958ad8af5f0e20bca70c3cce66a41b368617bb7e2699232cc1561615585b03d93d400906d4141de45" },
                { "gn", "fff5aa95306ce645e15ed00628e3d829493eccbfcfcd697bd7dd89ecdfb14e1c932211670170d9d11191932e4cdd2cd5c260022bdbd54adbf5f6d4b71bce6ce8" },
                { "gu-IN", "854cff83e13d1e903f92cacb77a9f9af40f0c636de8a1f9eaf99a4168af9a6b19ab578ac186848bc57d45c29ef0c0ea1ef6c0ffe3161531779df54edef42b40f" },
                { "he", "01b22694c035eb54ce1b3e696961decf52ce086993f825855b77188620e3c8690269cff03d049365f935717fea956458ff69975a8ab2e88438d2dfcf87260bc8" },
                { "hi-IN", "ed4029976009b7a24621e9079a99b2c9a4f8cd855c7ecbeda448e04746b8350e5f72e0598fe0faa096e7995a5344cb45a8617f49d9f0868e5fe5aab2d013933f" },
                { "hr", "d04c0ab785ad914257bb54664e336a1fe2f7b117280835673d4647787b56d59658c45dd9ce21d5c4cbfc22decfd2e30a6d3759864e2c015edab4b5212e3b8b52" },
                { "hsb", "cd7af6205cec8484261d4bd273be8ee3c75b4738b5d8fede38e10be6e8be9b64a49c2384e7382aef1c4731bb8a1b77e841566919a761bbb7685c3165a6d242db" },
                { "hu", "f41b059a0cac7fa3bf4734ad1b59d33a943a5046d8c375ea65ec9b723545e950442d7fbc67e9b6b271262be3e8221d1a6be97141a9bbc63f5c3fc68435845d63" },
                { "hy-AM", "3f4c60735f9f252ea96e33a3332db214770c94edf6d6f62129b9adcb9d0cda746d9a1ae5bb9386d707dd18c492fc450d6c8585c4c0bf71b769a7581bffeafc03" },
                { "ia", "1a152bdcf41c5a33920d8acb73f82cfb0d71666c156b131c773b7852362d955539e2f72e8319d7480bb426045197f45beaace1252f074403bd1951b4ff837bc5" },
                { "id", "2c145964a0978d8c8c6fab6ba883392a2c760a377c3999d5320f7536c818a81351bfadb171a2fd9cf6a0eaa46d97f610f5cf97651893a63bb7fef51c6fb231fe" },
                { "is", "16437e8f7341b2d5aae66e7dbd5e3e7c57aa485eef0939f2b72246c075abdc159e97560a6027bb7bc69fb6120d2e8cc17cbbc69f52048e0a1d049b40db421879" },
                { "it", "4f178ff8fa449c43f2198b33f0c346027d96c3021bdf0af76b5e9a40b7cde81c51606651e2d8e505cdccf795953ef5c3120ce57a50c7a451a948183a511bb9c3" },
                { "ja", "a946c307c3e6ba4472c782e7ff0e99a91c357cd45537519d62de8c7e1d5a267b5b0697d01244932a4b45b9d677979aec91fa2f767f0565649b954ddf3412ec82" },
                { "ka", "87cd332786264315d93df26242c3c1320a3ede9b29beecdd8afea54a49843e9fc52fb3a8dd176602e8284ffba4f1f8b4b26ec011ba23dc4badb491d8f0177bc3" },
                { "kab", "3eedb8c812d0e3b3050e7060a5a5e2465d79515c9e87f921f44f1ab369b371f165d097dd0f793e017c0400570709baf555accf16f06df29364f4129b1845d741" },
                { "kk", "175f4f1729ad6ec8fa45168118fc7b256dcce3c4fb7de3b1672c632c1ded72b608cbc2c5e0f436a20c3be8df1aa6227d4e00794fb5df7d144e054a2b46d2f32d" },
                { "km", "70aae12374f8af68dad832b1478d387ad46906cac4d5d1d8508eb6072380565ea9798245bf0c04ecafe69f9c43fd39c746ca86e6b5c9d5b4749b56f1a08f0ca6" },
                { "kn", "f335800ac7d5de089336d1b288a291ec34d216143b653523f246df4adb918a2dd24737f6721b526e8fa852d2bf8b0f9e84eca8740587ba0704bc779157fb944e" },
                { "ko", "3fdd72fa0bd10fbe582c596a375c3310eb4034015f2cd279f8760661d9da2fd7e82b0f67562a6aa2671fe60412cf3dcdfcc556e75387d197883ed86655b57757" },
                { "lij", "7d69eb41ab2a4420013caac75d1caaa29645d938a1ce8b2b56e99b0bf7439a29925b2c05568f6fc56203ed74ebbc29003b6b3d3ad849efd003f3c90f0d079dbe" },
                { "lt", "03294fb65edf7a5fe76cb9b0d9ff1b3db2bf3c5e3d88c2d3022891960b3ea112ff3c50ccb618bba01f277e5372d1fc54755137122ebbf36ea4255c1ecbe1604d" },
                { "lv", "4b9b2d999e9555076d4ec0e9968c4ce9a42394ebaad145fda2aa33192b6e5330e461e0a977606bbf4b5326797654b0f0fa7f6d375ef33a5778bc34864635679e" },
                { "mk", "ba3c7b76b1800882f91177deb73f201d2c15c6b47757d05005cf9b721264219a2818c968c4c061b037382bc740109943805668ac9546ba61e92855cdb92774da" },
                { "mr", "f434b8735a5e36cefbd7cb7f88f0cb535b3f04c0d3f25336c983548880dbb0d188f3146d7cb1b5d4b7784938c580194118c605af968541031267eadbc9a8a550" },
                { "ms", "321ef3642f69215335a2478146808c3ac98519d3d79e30f6c8b82c1705a4f03e421aa6eae2ac26739cd83aec92594b747249d19f6d92ed2fad2c83186effab51" },
                { "my", "cab1a5308b84a58c456e1d9c08f68923e724ad65c31119201f5cdc15cd888342ed20a489fe78c36bfb4aa8984aa8a23ccab2c436c39790510699ef60f0fdf2b9" },
                { "nb-NO", "1fab76d0c0aa064521598560b9cd6b4527d94f63ef9943b0db592d0cc0db6274ff20f369199febea80f743f92c735e31da8e8a33637994f700d514cfdcf6026c" },
                { "ne-NP", "de1e7900e03b0ff5f3b1d88d6c21e7aace9d992759d2aa6c237d3bc0845446e3e04341f9fa8da69dd0eab4ec141e9f3e756dea3cce6b22d868e3fae985645d93" },
                { "nl", "3b3087f3d86c0871e8d0af8ec1ac566cba1eaccbabe1939a78bbc695bf99a524a18574950d21e72c7c80be4605e267451235ae36c3a43008bd184a33c82952e2" },
                { "nn-NO", "0d228ed3d3a723b74232eed1c3daa3301d2844b21247a1ea10d37511d781f2a5bd24e44cefbe048b967f26e4c2f7c160e87279d10178775b3ef5d786966696fd" },
                { "oc", "44c6e76c6958d547e17c5701e51864866448b29efc4dcd7ce249194f0bcb6bc323fac23c98998dc55189c6058e9d0ae23ea9c4bedb2309019900c13620134a4d" },
                { "pa-IN", "2428a36a44dce337b30b200d2ffc77434d405245b06dbf3440cb5b7f480af6e41fb07a8c75ce3941e36b27612c9f918db34c950e2ed3e683878a2f7c84112bcb" },
                { "pl", "a20277b8a97ca1abc2dfcc06a1c2a0175a077afd6c519a54a4da3edd935a25fe62917276e55afaadccd33e0811314ed3baf2c07f5c5aba011c937979f608cfc5" },
                { "pt-BR", "ceed55e4739ffcb3aae8bd6a3c55657411736a1c9b56120eb72e0cfd10deabf865488cd026aca8f58abeccc22ce84f806e29114774cc3a9b384c3d33c059753a" },
                { "pt-PT", "3df665bee0a87b95941a15f48e4d187b7b326dcbf0385fe658c7d2410877fafe71caea46af1101ea93eed5ec395a842f1447cc0e71f23b34e77b509e0ba9ffa2" },
                { "rm", "87fd839515ef3d063bf039c2c5c4edfb5d68e4bd8db82dbbc9d3e5ef0f1874055805c6f827eacd08249a08fb061fe9032ffc8127f7c73bcce8f766f69b18af71" },
                { "ro", "dc3126b989aa5b4a177bc458265f35e85d2f5f1a93f4d4f8424152e267e057d11387b52b45b82bf2bbab63d942d765590331018661fc56d11874b881a5c34067" },
                { "ru", "208c15fc53276786777fbf679f08622ad3215648022bcee73713f188f5e5c668f4e0844636b9ccbb9acf02665b5d7418d4a60d26044e4712cc3dbd35958242a3" },
                { "sc", "b8a4dc32a3cc4738f7897fe45fb7bf247bdd3ae467137505a97e7195a57091ecc6f9fa76e07315c37d3b105b5de1425b806302939ebb8d4cf8b035413aff38e7" },
                { "sco", "154923d35fd1791f51494777a2e5a74ba0cf278d68c75c1953e0ae36453874c4aa7c235e16092de61b254eff8e6481de243dbfdff2bd485dc079930f11d6b16a" },
                { "si", "a293a7a9522955e22c2e186fcbe6523974d3a83a5ec1c3697bc26f886e8de65beec860313450bd25311eb316d5e2347ec3cbc424cca3d7c6ae79892846cd7cf3" },
                { "sk", "f01ee528d5cca9fee8beff6d1423768ec4f794748b1a4c992c8c8b5c753bab72d1784f7ba72e269bf878a4c526ec295e1fafe3942c24e18c163dcf2c5f3f72e9" },
                { "sl", "1330020dc97275e824f0ca91fda3be20bf730271e74d03ff4787cc199eb581111e71ba6df11e1fd4580ba23d2e7ac448c853250b6501051c9303847d3af3a7da" },
                { "son", "fbff120b33c9aa777e8dc6f7947c4288f2e77e3c45bccf05d2d3d846674329067ad18af4728f4832f641bb088c4451ae44bfe86c1189c7a04ba82f3cefa5df09" },
                { "sq", "8a463c6ced7592cdde60a981996e6458a92025d63a8012f90d2ad7f32cebad8ad31d7d99b145eeaa45d91b2d80d98913c83c4a84c134af5a756191cb69381391" },
                { "sr", "17f7da5e1c7d5e46059a40b92cde446864bd47edd0c68d11abc5aad44c0c1bf1a81cb3678645e76c7ca665eb13afcd7f3aba854b7b1485056c88d29bb2d5a7da" },
                { "sv-SE", "5ff9afa2f93399b62eb3e954e3ebd123bf196450c65e9e02ec8a1db43d7e083d296c62c13fe40836eb6c70532a09fd86d46099e40de346fb62114c471d1d25ec" },
                { "szl", "cb2c35e5963b958857a47188aaa6ee5dc234b68130e2466e3c00bb6d8f1af84783b94769f64a1221c8c44c07cf904ea9e925c8657f5ea6cdb371018099c2691a" },
                { "ta", "40a80f5aa351e58673ac7e78e717342c23705140bf708e69aa2074b8f0f2e830bb789ebc928a76f16123c97c4f0251699186b6814662701eaaa03931a1dd6022" },
                { "te", "4e7a9c89cbe6a40466f716f9c42466e1eeb30d0710a3b0fa000ad8aee9209ead7947a9bb56275e4653d549931842ace62d07bd9a8f921abb467c658297b3c123" },
                { "tg", "61a5bc8a625704a04d07129ff0132837868272471d212f30a20a320896743d2d4e74bcad014bfc47a5e0a596825cddc417480f4d793aa6013eb39ff29d381284" },
                { "th", "56ca4bcec9c3c74e598cd76e8730f8d8f35355eb71248ec201286e393f0303a5c68661355d290f3d5c8d26956252df0d7d085e2ee1aaba493e7e45f3d8840033" },
                { "tl", "ae7b937e434bfda70a6a6d7ce15db0619c3bff24009fb6fe26ab481e6369c8664fef1f564d3285c5c72882a59541abfaba0c04f30426d940875b8e629dde592b" },
                { "tr", "5f7a1534b4ea4d70e47747bc36a21bef9a780638046e43e22186dd9a9b6d5a90b2eb20807444c367ff57e68e8badb306ef7be79d2520934fc8ce8b5b65e4141c" },
                { "trs", "98457d5f0f006079f7620f9d835f572f3c5b137ac1f253d26eda17e8ae19a5eaffd16a5ed5dde2242fc3f766ad590c279d833235bb1dd45b79ee7d3a777c7ff9" },
                { "uk", "a9ef5dbd5a01a8b6e5086d85723cbd5840a0af7d6de9dcdebbcc5cc8f5710070be125f99b73c2c0efec13bf5dae51078fed29aeeb0041005fd9a29b857cd2ea8" },
                { "ur", "7681ec0667a130bd8df3331895b6251b0bf9e06f2af15e4f7f5716e4fb9e0076aa61318c8764a68569ac390a5b6b90166c823cedb42c7852a1181ee0edb35b14" },
                { "uz", "b89a23da859f99e86f03883e110126cb54d71965453f881201ed00e78c8e798f5150b3d2a7847b323ce0f6aa36db58d199239da0b9ec8475b09eff8202b23fc6" },
                { "vi", "5eb8be087746c1ce8196c291e3427f672facc174a0186d970cddce1c036230eca026d304e769b16d287bfa455bd5d62d5bcadef0a2c9648985900f75bd088e31" },
                { "xh", "31cc8bfcfe7c787613f455694edd7492449e87f7f520ba2e1665abb541140032e410654cc5d1d7c99b898db34a0f084f9d49d246fdd1bbf9f3f6e6fbcb683e76" },
                { "zh-CN", "2bd3997cc380ac7761622c4692cab701617687b4000c68d5338c2cf5ef8f23f923e3b28b6744590d85294395e00667bb1f4ed8659dbeff8cc5a2d724d1b2f12b" },
                { "zh-TW", "59e9d59edfdeaf03516642120298a2db7bde931206de82f677def2f2a4236613b5dd5795d96f8443709b08a04c3913151ba1763d209fb30c779a25fffd28511c" }
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
