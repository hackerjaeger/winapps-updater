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
        private const string currentVersion = "120.0b1";

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
            // https://ftp.mozilla.org/pub/devedition/releases/120.0b1/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "b9194d60b334431e6e89a4424fdaae5cb0ba954bea401086f29ef1aecaff29c70f7707772d102278177b0d3aaffb4d37a34c5d619cae680296cfcef839d850ae" },
                { "af", "6cf27ef664510c66a3967afe9e748932016d3207f32fbb15bf8f713a0e7ec19a3b99f8a5bf284c9e04d5b04b1f5dcf39524f0975c1f21831e0887ac8f7f7d7ed" },
                { "an", "ec9ac5885bc959326ca4e69089a16e200d48e1f8f5dc423efe997cacaecd7e4eceb5d3dc9176dcde199f515482af1e063993839cad42154f40e20b3706f0b3ef" },
                { "ar", "d6a7a788d274e4fcaa4fb0ba04ac654a378fed4814a430e15d4aa50e4591414114a728913d8f3bd3e25702291cc3c0a53736774527bd6d134f3c5def7ea51430" },
                { "ast", "d772cec6bfc103502cd840e8af543fab6757970ff5fce0bfd613781a9252c404fcdb98409cdba9d0a73bfa2409389b2edcca1b2a4ce77bc2eff3bb94586d1f96" },
                { "az", "0f99a6d62a4f14cc6d33a48b18702b931a102d5b2fa9c8383d872a2fd22d45632540dc72d561e7b8cb9a3245c4ea58f334d57d1d99495a371bc04bf5db91f9e9" },
                { "be", "3e7eec21075771282e4d301b4813ac9079d76acb5ae5c7c4115df5d41c285c5c1e616ad0d01a0353d6ad2fffee5523afdd38e9fd29ccf0b2ee29eab350d04716" },
                { "bg", "894908314703f15869e5bb7686ed26e5c4ff3f62b0a5ce0db290ff807c0834c1e9c593003b847b1bbe41be1adae604a57fac583ac2321320a55d9ca0460f2f39" },
                { "bn", "b4c64327f8762d02248dbf36c5a3fdf0be439f88d4c9cdc28897a3e2badb2b992c74bf48e66d2d93c372ac71ae653421983ad3b5e2b0e5a7feb2177c25826999" },
                { "br", "9958bc1fb98b10de98464387aa374a8702712a17799071755493ffa97b69f2b2ec5119864e4349ef1374f0d9ff3ad921d630782586bac34a926b01b9f575a0f5" },
                { "bs", "89fda3d291ea6e60448685bf386b66b457404273cdf7b8c4cb46d8951edc2ef195b93bb22c51a1ee79323475751802573337e1203bd4f930fed0df745df76bce" },
                { "ca", "32b0ae920dc886ed0adce6b8ff19de19fc3fc4764ddff71739374bde050c7ebef084884af72aa24a29b60e6076b41942c5e7d2f15c89098422acd3cfd673c5b4" },
                { "cak", "ae6d5eedaf76fa5b03c6a8ba2cf4a266ce03b74b1e4bf7ae99c120ffb0c9fa9bc5b190959fbc2874412e8eee9d45669f48f6c02a19f05da7bed2f424a4693dd5" },
                { "cs", "604d8d6eeb1896e38b6c70ec58676b800c5bc7c56119b3425aa79d3079e4bb18487d580ae1886d58c2f23f6eda79285a90cae2bd8bbecf0bf7835764c352ebce" },
                { "cy", "03988d17c13a5d6da31fce863db55305b6493300e216cbd72106483b62ed78a537e8d962e8683a3453aec76dcb0a54a565fdc554dc1f6339015ad9f8fa67c433" },
                { "da", "2bd44975a2773873468107b4d595adaaf1de054abbdc487317fa750ccdbd358f00ce67905ab6765940ab6ce28c447f8823c94baad613154b1831b18e1f1f1a79" },
                { "de", "b37418ef405988eccdc71979fd8bc02491fce9bbf144bc59e967fa2e4712ecc752b6a0de68a7209c3c2c22c9e6f0732254e6908810e3ed6a87e264324c88483d" },
                { "dsb", "1428fe0483e51a4ae2bf9ab8416bc7c0c8429bdaa2a4cb7154a046ea49c64759da05f4030aea3fe744f3a40d83d97c73702771c62eb4d981dcf45c4d8af5c455" },
                { "el", "786e405b5b645cdb364a410daa87b519c265b9f97a9169edb49d0c9fe5d879f9e7b50b6f6c2e68e5df183344b88f68075e298ab190f8d6e51fae63afc84abb3d" },
                { "en-CA", "6f19167f9e12c22275d17ce774aadfb47a4cc4632ac3b74c4b2d2a97dca80e6edcdacf0fd091601660855c455a9b6d7cc8d65786d0319be5fd7e3ba65e7648a0" },
                { "en-GB", "2f1a1fe790f13bea4635dd52d3c07d6eda2c9358751809199ee89e3a9bca6563d0cec90e38c8470e683852a70f66389f5a6921191dde63d1017f617d45aa3406" },
                { "en-US", "85b0576eafaff5af856b42526745b10af59c887504596db7cab3d12de93cab2496c5f0ceb4b96918fb516c5118b2976b7644095c4805f6ce5f213b9bf74e2652" },
                { "eo", "414ef12e05303c388e5d16db2ab0f96e82e18fb3c1b1b0489d58a6347f5c7baf05c6d903b1d7c17874da1cd720cec5eeb3efdd0a40bb0fdb457775f56f84389f" },
                { "es-AR", "edc691cc1d2863c154dab4677e14c87eff67e18d2344c57719acb8a04357c9e17ebfc929999553df2ad7fd2cd3c29316f71c18c0b5ffa99b5c45446c0a20afe8" },
                { "es-CL", "93b05d01ab4974f6fbd68ce8b6a469bf6a39cfa3509362d2aa052f2a28914c704edc097debc9281c3df743e57227093faf131775c010d28bb7ff22788d6e1712" },
                { "es-ES", "9d76e6a9e6936bc6c273ff5a4c772e6b8fe06b6519e983583b48ac9c7620a3b2692c9ec1ef72b806546e1a5c9e01f9ae4acb0db8c48054418b0bdcf34915176c" },
                { "es-MX", "a69eff71625962974fbcdb81a64205ef732c2fb9a5894093ea64d38120e3d1f67bf0e315b22498543f728d73fc961f2ebd2578af730b2be09d64e69abcdcf095" },
                { "et", "f210c38a2a70aa86ce2cbacb1a2b8c296e08f09c94c980abac8411091203c4f54c500d92adfaeed1e2f136f0b0ff50d611d5518c1106cecfebaa15fa9218fc61" },
                { "eu", "ed1b236985864a302d10d3d7c221930ec0e83c202151e48094d41d69876b2715220e4735b93334d6dd5798a88cfab2fda614ededceea8a641a91fe0bc0689313" },
                { "fa", "6d7478589e8b686220d187e8bab62d043f269aff0195a6d79e8abfc46255c6999f5537da07e5361e0a7dc7e860844c8b8eac6c63964a7a9f184c496725c70eb7" },
                { "ff", "0b1bc58097aa36dbff17a8e48dc0431612f574ca1ccfe859767c1c6901f1730b1b17b1cbc5075a6745c7faf87070e822bb32bd9a94305679b38a94a6c56a7e9c" },
                { "fi", "1d04394edd5f9f3f4f25207cea6ed000cc13368e05241e2f107dea1efb0dc41ab2d865d99a6460d02b3dbd5c2090832dac7ec3d71acff39b96d79a4e6dd0c3a8" },
                { "fr", "4866c24aec77777c60f54f22f275b560fca571c2d81b0c9d610493faa395983a0c9fe4dc28bc1a43c9311bcf485cf2214b0acce1bdda47df13a615af261aca08" },
                { "fur", "fa78fba7026df4ba1c43cb6f0327ed1912b9391fe91631127dfeef72dd73e46c2f6e05484ebb6cb568afc0fe37a5f2f9bd17b99b4f1379dc070db7a63a927959" },
                { "fy-NL", "64506ecd85e298f2823c07a39d7d11a83fcb3f806e73dcc7303e2a6d7c2e60ae0f6d870a2513861a1d9736399a4c845c998b308fe24aa2ecf2a5071124b249ee" },
                { "ga-IE", "ec23bd3efecdf94fc9e768ab8f0cc8364f7c367067e599038d47995531db0b0acee0a41187a8117f154117528e305ec59d60a6b0f5d9b66e6be4dea60c9b0cd0" },
                { "gd", "249deaec371dce15ea1bd83541c36ca677a8707370bd8e6b34825b1df8538d98de0346f9c8e9ee0340e8d795630e60746c23d6d5a0df3fcd3a86c31d88bff523" },
                { "gl", "81ab1a018772d1f31da4f0c745000d0abc9734bb1f670c1c260d7dd5aa98101ea9a84341b5b408076baf4d9ab20d045d2d7c3b9dd03368a5282bc4e6f9703a60" },
                { "gn", "e58b67347c0c6d1ae544432674f13efdc4511cf2ea11b48e031b15173aae0fe4b8b6401e93185d254052a634fdd66d91f935312eeb961a4fa1bfcca31b592fd3" },
                { "gu-IN", "077fceabc65c930b8a66d87ebd316868d33cdd4ca06f9530bceecbc161f3a7e8ab1cd4300a4b83df9c1a71de7e0215b825adbfbe01fe9ae0ef1f65f24ad7fcb6" },
                { "he", "a1f75df8fc2c63085bfaab18134716595e9fda76f0137bb6453598d100fd73af899594e591d1e5ed9f2019f7960a8448ef5549413cc4c107a66ac5a7df33b09c" },
                { "hi-IN", "26782101aa62934bb050a93dd6d12b5a62797ba9667b61eeba41953a56afa3206159ecb75102e3bb744549b3d0a990e44d1ec72b4ccd3892822cc34ebcf4ad00" },
                { "hr", "3939312b9eb3fdc7ef114f42ecc9cae9ab52ea6d64c2caa3f29c6cb2c89c4077c046dbae8c3f2643007008a0085bc32b1e197a4b10b90f6379ef6c5bc6810bfa" },
                { "hsb", "08b971d49c6ae7693b1f2272037de2a0daa42a76abfcb436f9e6cda83377896ffc5a53d364a5d258ebcd54be92f50f3d95713611c37bc79a644c3feb12712bc1" },
                { "hu", "5fcddae36c30df03ab379978296c7d712ec217ee5d8e73aae0ddff161b065153d524b950befea89ece34532aec4b004957e6cf061f6e7e4a07dd74a23106d8b7" },
                { "hy-AM", "25357715db44f3ec64c2e64df4d623b15a595047c674bb07379b7e8fb2a55fbacecd547fdcd411d179d09e60b7616f1b7045bae9b3435685bef5057ae5a338a2" },
                { "ia", "730521b67958ce83a82e1764e4d6c587e630a363310da1ae333714ecc46ba4d88cadb41ace09a56315517b6032749a4e605f7192315fd099ccd8257c98fda1ee" },
                { "id", "ee6092071fd405ca01b42ea96bb2666e19508c3691c6b6f38ba21ec91d2c03a9388add0b5fbdf70d79153e38aefaaccc46dda61a55559086d0025af23d8a6037" },
                { "is", "94bd9f3578bbc610f443212453e70b00990f084ed0ef5af0e6b727f8a20fb799ce2c575af6e5fb7b513824165c610d18a63fc2e7ab4055bb76047b2711eb683d" },
                { "it", "87061c0e05de9f6a0163c41f604812a4d4f3748b2f227336c7b516df2c860c1bfde03c6a32783ed0f63b2ae2b50b2c1b6c773a1f19fce11dd3d30403c0d4023a" },
                { "ja", "c9a8c04143f02119b41dfbc470c0d609212a76d0000da7497c5b568354fcdbc642801a85a5616cfd582e0823c775daeea507a82f044dfbbb06af351bfd6d2647" },
                { "ka", "8a1e66610b3732626da78e8baa5133b582bb801c46f424606976e9d1659d240ddf1a57da47e95563a0000c4844012704de42febb6bddbe26fbe3306b5b685be3" },
                { "kab", "5cc524cc9c76d38a293ab829175a9d1312167f7668c87094a65b3880f145dd3dc2b24ae6eae4110c8892193dbeced36a73fa045502e9ff097cbfea51dd70d7ac" },
                { "kk", "82aec9dbfffd2efd8cd6bcf80502e1b99bcb8631665c294d6cca83df9e22c1309124996b4dd774981b0256c01b867117922136443d73389eb811108dd18db07f" },
                { "km", "16d77eecdfec8b2c059181e4a84f8ce71007db17cb1b52ec3c6e5a88244019ce4997179861ebafec3fb43547a618ee23f32b1890d9428645158431de1d35a4f1" },
                { "kn", "aa07fe91fee6de5c6a2341eb5bb9fd72bb6dc6ab7d86725f49e20dad0d503c0a35c5f7583a3f0702356e66410653721710299377f30cb5b4f1c94b135caad1ab" },
                { "ko", "d7c4f150c60a609d8d3949903edb7607ab4fc6113f14be2b370ca34fac9f13b66638ff752f098ad9f917732925f198902da5fa5532e3e8708cb309506e474865" },
                { "lij", "528084c1645994a12c2198d9e58fa4027ce3c8dba372b622287d97e885a673686158f03a1944cb66212531638d22ddfb36490c840f067d93cf8fb4f8fe2eb6a5" },
                { "lt", "8e3d09d9d2dbe4dd2df923f39aa0c65c0696cc41c20e27f62c99510c7e4c223965710bac2e3d38d73c0bd2befb0bc423fffa8fe29c6ba7d84d2edefa6effd888" },
                { "lv", "0637e3feeef3d14c719a4b49b6e509553aaf1918b9228fa4d2abd25e98682cc0ecd41336e71bee67887714505ea19b12478b1a1b53ee2522db4421a70526ea45" },
                { "mk", "5967269ef1403e73bd29a317256b9da0f0487f6115b93c31d9ff5a4e034b11b841b61dae0df57cb8bb02fb33304b33142ccc2e808c673576b43522132852566d" },
                { "mr", "3f7d3caa5ed77681a9c2ac062a445ca4013a3be2200185a2709d7c930bb4d72e91370c28eabbd44667d1d416e022e57b7655d2ac1335e28d10bd623f6bcc2022" },
                { "ms", "1b9a026c8e907bf4defe36ee3d1bbd05464039f8a5565c9b8d2346333c2095a2d8013177a804851308add8f428a665c8b6700be7e7580ba8d13a8806aeaa50dc" },
                { "my", "3fd3b580ea50d9ea4abf9a36a1ae83566ea7973ba15579fc2daa336deb132f1ca7d035cccff2dfd77e8940b5f88e7940d6f1996be8343ce13f640a35ef426858" },
                { "nb-NO", "1335428cc983fb20140bb06e61c7d612344984c708c1dfc03e83d57b4ca93b483425a6e1585060f49e0613ab04a2906b1d330d089637e525c879aa594ed2fa24" },
                { "ne-NP", "8e56ce2f700ebaf1880d0330bb624e96c1bdbb84c248f2bc9dc2375c831a6f3c9ca341f1d9668d30bab53aad2603bff45fda97cbbb7f8bd5a7ba3589193996d5" },
                { "nl", "c849d5bde0d7d32f764a65571a81379bb4aabaa64e197a954daedbdb524dff91c2ba664f59d023bd349ce60f12a37256da2c196785505e248026089a0f475b0e" },
                { "nn-NO", "e2aa27b46a843606a4ec7e884d3a0d6c8b0fe3d472b2ce0dfdfff1584c81c68eb6e314362ed9cc50e5866578007169018b954af0f66d0659e93f769b995ab900" },
                { "oc", "253f705f2cc46e4984e70c178e59a170c422cbb0d0d6340228119bb3492b8fa21b52490af9be028b91fb461c60147eda8987706ad84b596f1e3ef3fcc7241680" },
                { "pa-IN", "7e063371640b5fd3714b442e6e140c3e89cbda1b4dd10215d384e6699495e1761659eb8056f809f04bed9a4dfac4c0a11f20f01a63733525643539641d6b0241" },
                { "pl", "04cef855c04481838cdfad9093cad85f39a4e94997b76f5f5f2843edf4ccfc099fe3ee10b30373f7ff4123f3f2524e0b368e07a49b53131bc4303abb686d7520" },
                { "pt-BR", "29b43af14ee2bd3035135e29c433a1d344b787467ae09a5af1aca5b1a22ab9608d6b6960764f72c22ede2c4de9adb1fa335c36c539d661ae36b93948879d3464" },
                { "pt-PT", "50f15a07a5fdad5c031de97b918d37a295020a0268fcfe9811a74d16e9f29d9c9e69b09259592a7c8c10b5611c571143dfe26a221a390c87c6b76c74425eba7b" },
                { "rm", "c4b9ee14932cc79234c2a982d12559f168e9d81ba56c9aa0874104fb00f1b720b2603717c3284d857d5ebfb5a9e2b04a3f1e9cee18bcc63093c5fba0e2e610d8" },
                { "ro", "63cad017bf7d4ba005a13656320f3f7d465fd9ad7afa5ce98f37d95c5da96d3d6825a4bae2430be950dd8b564b5f89068f9f26c4e6188cab6041be1893d0cdc4" },
                { "ru", "f603b308a89559135b7b70bef4ebfa57d26f55af3af54fa5e0cbe0922bebb597b43debf1f38de1322358b71e076296f4ae60b5616f9dbd305811f670f9903d13" },
                { "sat", "caec936f5d65d298c5b302ee28f03cdd0cb3771eee2f77190d92e637b974eec6441a6a5d22a3670455cc25115cf83218f991bad2596f8bb3891b0783cfa67129" },
                { "sc", "cb4ad434dc735348316de789fc66e1e78ec7f6a98d8579ac4db851d93ad51d620fe0d305d9fd889972997e15fdee6a6eeba13a5629ead41601d7ed7022e80405" },
                { "sco", "dee57de13e65d405674079045312bae8d2999fc7c08a89fc6abdacf521216ad3ef2ea622723d7a4bc805341fa0ee48e6a2e45ebfdd6111f521e62df9c7529235" },
                { "si", "4675c9e092d58be99a6d04ffebd62f61ae1e781d2b232500a1d7b30d7f3f235d89fac8bf60b61ddd4f2f9f190687c56a306bffb823ccca1985346861a90df2d9" },
                { "sk", "2bb4c085bc858971db5064da67f87827f133055a4205e6e537aa0d5ec22e2205176207a11d7faa8292c38cfb3d590b52cd9a091af0f2ffce340861a15d9a454a" },
                { "sl", "594535121e4c80c57a79b42f2161da6bed621851f91050c853dfc3f887d03b23e0abfbec07bcc2470c7bc16e1390677c86660ef878c9b4f2988ce89db350a663" },
                { "son", "e67b229bdc1adc1b1adc1bef1ddafdb264e532af55cd4699b208707133b1d49c074b7af7c65e06f7bd2570f76ec6f5e0178c61f97f835e3e4aadceb216ff071a" },
                { "sq", "e37152f669e30b7afdc4f572ab68537dd4cfedf9c9b354d64bd483b93722ff0c7df2eb5c5e0bdc93a3714e6b3d837c208b758c6095d97185bd291a7ee2b8df90" },
                { "sr", "af37e39fbc497fbbf92a44260fb022871c0564be88d54148063e3504435fc9c3e49af3015799b994bdca9447226bfdbf8f18b727947b580ae81db3c91ea8c629" },
                { "sv-SE", "94b0b4514531725a1e497af659ecef11f5fbdf82c7ba316645a4d9d59b58659f8ad1d370a46ddf4c4efdb9befa9c7866ac8224a21d5e70f7ef32dfad3d1c402e" },
                { "szl", "27fa7057faeccae7d23ce88a5035eaf5a36e51fe902e955fdc88301b814089c2b44cb72a40685741bddf9452ccaf0bb2d22a76bb651a10b5ada5358363e10f59" },
                { "ta", "d00269854d6f16e6556b410196b132f9d9f2f63ed3ab3a93ebf0f8df1c23391cdf10d8eb408d7c6690cfd9e7c0a7cb02b21057b2baf0e219ff2df23ed9742f9e" },
                { "te", "92e30818c504512d3843622a22b9c8e89e6dcdfe7f4748ead16b497807e9e899a7052582e26fa01a1651451eb8e23e49853a75d98294e380f0b7e9def052235d" },
                { "tg", "dee89172a9e70705893ee7ed9952b9bcf9180dc21f71d1972c5d917657b7578161f73d2007c948977f74c0e1bb2b8c047e350153036e9f4a4c6422d2163bb9cc" },
                { "th", "d6ac00f42bdac3200fc5105c370f737f9fed5df6fb6ba79e4b97c6d826b0e31d05f6d885b7da79079d07f2b437acac18e37e3df28fd29fdac149db2313914398" },
                { "tl", "2da9a44191c40bab0b1b3547be27cf0a24d611da523c56a10f4acf72aecbbb1421787898c621c5dec67bc599cefd548e94882a38a6fcb76a1c4c2cb25a739b1d" },
                { "tr", "9bd9fcdc9a21267b501676adbff224f58da0997082e67f4b92e9512c7cb14bcd3f8c9ad0b7822f7b539a5547ac5e28c59f5130a9be4e3810c032e07a970bbf64" },
                { "trs", "0e691888c13349e3eeba3c8c514e1b6de6b760cc9c78604e2bd7c93cf9a2cea9eeeea173edf933ce9b624243f13ce2102c1cb6f39e9b6b669b95715210331eb0" },
                { "uk", "8e411edecc9b7cc83b71caa83eb4da9135a4f9f0d280749602f92c3e62c69ba05069f46fc8910fbca8328ee28faec49de4773617afa482329ae09faf46ee724b" },
                { "ur", "510d6083abab14af1f5cc34ed52bd89b54ade473db6fe2a614988608f69714cba9bf0b8f2126f9a7a49691f61bd57b8bbd2328e37e645ee2120a795d5594f682" },
                { "uz", "0975e77483712c9ec4f47ed3fe1617b3bd168bff55a8d9bb7bc78de7daa4889b0bbacc29ef30278b01a83c98f57bb44d32ddd99960c4f3b830cdac10d22d5301" },
                { "vi", "65b86a6c3e142b8796c963d9084d2b3cf3a5a8b56c381643c3753b24708baf8c3522cde9b2cda760e46fd200153f0d1daf8d7fe924e1d2cace072c6ddfaed514" },
                { "xh", "58c6ade169809d5f9cbf39a6b14158a37e532291ce507463f0b7a090fd5615ef205bd632671bad32ccd42c8b8d105fcb1fe2d948bd504d9a57f6e86552977a3a" },
                { "zh-CN", "1e10d0d0f59ebc4b4bbb10c8abb7d56fdc1404a08ab3ed6ec22115c6d8ede4dec4e68a98f4b955c3aee25dc906613933ef095f3a0b1d49f54dfd35fa37fe4bd7" },
                { "zh-TW", "71dfca7959e403c6826b07ffab215137e7d3dbf4dcedb5ebcd433a555037156366e0901ef540cb6e711ba34ccb38366533d8bb73eb686067d34c52d15ee32b55" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/120.0b1/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "2455e7a2ae3f65bf742b24bd5df52f1b4bb265bac0d7d7f610d7d26f82a5bf12e5fadb31b6917961e688094bdc41a695ea2c4b133dcd266a28b8822b68f4c862" },
                { "af", "004d442bc6c9ac08e82a2c419d8e805092d254a63d98e4c93eda39699f941fcc709308272b8f47ee3075d1c7db91d0f5a4ce94d45445a232d3deb18a9baf343a" },
                { "an", "1f6ca37721fd89048d34a08cca8b11b2503c2d760d12ab1707b1c386d6205ac08274a761f95dc87395ae8df9eb042e35914f70e66c82230cd8dd8515269474df" },
                { "ar", "4777ded2b0e305bbd7163b93d14ac1890d73070e5e9b3b274949878f73630c2f3e4389e5a8e2e52c85fb68c0bba56e20b64885900a83e18a36933f4530495c14" },
                { "ast", "e6c7f0f46ac13f8c7bada429d5c5484ba2bb44273b708de459eb8b33666714695d0e79f0164c7bd8bd9683e626a75a1457fa9f31dc73324236c8bcbe7b90ad6d" },
                { "az", "aa37b6c41eec1eb05c7b61146e06c6afdeee600216e3beb02d2b64df6ad3967bf5c66f631541763a3803953cb693de8ea9ef5f84999c54ac7ed92aac46dad2a9" },
                { "be", "ff91e8e0e685c4fe65fdc0423c076db04bde0eeb58fcaf2de0c14f30b8ef3454134fca0efc11a5bef38fcc3ca241f358fb2e666442753d62213971ec128775f5" },
                { "bg", "e04af8dabb58ea5e2b74480c22a9d00b338bf5076562c117a1e6be5db966b6f156a855448ea0c0cb48e588999028475da70c63b9ca8290d61790e25de78e5e31" },
                { "bn", "68ff72101373caa70fcc3db8114052546a9398de655c20648e8a6ad9d73e1c3f337aa168b733ae7ba384c7857d156b11700bbb3fbeb25af48c6abebde3451e4f" },
                { "br", "4cba889c8426f5ff02bfe6acff8922bbf7856b8e4f15fbb832d1729b92fd4a876bdc8e8ff3ea4c97d18f2d7f8491c85d1e2b31ee5c14cbd5a6eca5f19eeefe01" },
                { "bs", "2cd57f8b8aa7584278b1783ef37a619cc85c91e259ddb1f8057027ff8ab14f6fbcc4694460af14dbc38ab9eab37fecb941e8be308c7512848a77e6afbcef34a7" },
                { "ca", "17a72795d7be48699b19f9f8970823e8cfdaf3ee13b39cd2145602c7d710c505199618ea723eab3a124ff22c71cc4e9ec98bab2ebb763eab9d01d9147abe2230" },
                { "cak", "14ff7c24974de6b1bcc7d522d7c810c6fccfb77cd6c9023dcff7f72574c5a65aafbc099e0d753747a13220f4836b21bcd17529856d1e850b141deef066442861" },
                { "cs", "15f44bc852dad6fade0cc37f3eb4ea18d1cee6dda8cb3dad4ca7b341342b9df1ecee0012a8c9e66ddcf4883ec2257ec964191c1a316a7bae49de35c75aab3919" },
                { "cy", "4b185387896c431b483522b10b886f43c7580d0a22c08955abc97043eb5f20ab6e4728b33cc1933a8868dc1a6bc0a02bc2b35c19967b3e664d0ad74f5324ef92" },
                { "da", "e2100575d44ed83a04d403f6f66efc5a08967402bb0258951d001fa0c06aaf439009a96b8e68e0d53b34b8f4a8ef5aabb3f462a40ce2b639b483a80e35bf7636" },
                { "de", "27ccacd23d8ea9cc34cf41faf4487e39359868b9f4e42d680d7822268fefe33fdd89deeeaa0bed4920b49146beaa60bc21511290adda7b44e520f6c35d8e1799" },
                { "dsb", "63b983bad9c23762ada3544bf7372bf0e28ce5732e40b0ca4bbf3aafae50d1bc2c3710670003d0c96d662b657f544f3ec942bfc22f5d33dafa4b1f4feee3613a" },
                { "el", "0e3f41a309c704ea188d6cd7bd80a89167fde90476b8a5e74093c48eac391e082e98c7dbd4373d570fd1b6e8e1fc20b8b8d0e76ce6fbe611e13b73a2489ffaf5" },
                { "en-CA", "bac555e5b829afb8d45e07919fd98552c694ce330f9c5c635a555ebdee0c141b7c4d752a2ce35fcf2d59022f34075909d896787593b9d05ed88d7a8c1439606d" },
                { "en-GB", "97249f9c82879fd6acda09ae8a25f016957cf389127c39b835535f07b964e0b551da5b715ee2e7426cb5e9a48ae60232f1438d0bd6b8981113d2653bafea8f6b" },
                { "en-US", "46288f42978d9c487a19b852fb3478f5ece19d8b5929373dd865bf4e37e1fbf077a480bd783b3db8252ff825a5db9f9bcf665d25e3536c2905fda7e9777142d3" },
                { "eo", "ae1f1e3e35b417c4541c5f180beaeadfe16d69ac7c73d50b7a290fbacb9f59239ee9a2d860da913a4ef26c0de0d23f39ef8a38bcc9a1de925ade8a6f80f1c910" },
                { "es-AR", "de419563a47db864ea07318ea33932bd9031d84e9a0e357dc07d680694190231232b51a10b00791320d951e86f444bd3ac20d73f03375231eafcc7519b896d7a" },
                { "es-CL", "3065d10213eaa4ee8b0dca1d3b2c169ae775600fe80ff2ab962031a4e196033d56cec60fd6be44e3301bae0709ab7e3191aa3c4e4b3c414f20e4dd0efe2b2eb5" },
                { "es-ES", "5305c8bde7242df01820c4bf3f4b5db83cb6ecae68eeb3f69e654ff2376e1f6126fa5c9dad31d57c39cb1d9491884a0fad0c56c8103529a305a8e35e0b397eb2" },
                { "es-MX", "8cf901bee4201b7956d8a2f8c69c88e971741c5ea4cfe0d3c10010b45279b5d1169063490a8257610bd6940794c52a57200faa5245552c10c9fa1993d95f4d89" },
                { "et", "571fd4d7ee414faf365ce33b83c6cf4fa06c802b4fcc415d58deb34a0a1d105b2ce0a0e8c21909d70941423bb85df9c4c17e80df8708370b52a19daa47321e51" },
                { "eu", "490f408f0b17bb46896892c63cfd276f3a5ec2bba680a94cfcd53e64fa8e37bcee76292acf1bcae7a7a4bcd80e964722de251afabdca57a684897f684690465b" },
                { "fa", "ffcefb09bc2abba2e635bb5a6d5088436df70ffe5bac4973972ef4fb6dcf87b45802dd9d8e3991be4e604877fad07622451012ed324489bb18c79698b45ce907" },
                { "ff", "f423d1425b865079cb04916e37d0878aa7a364ce83b73cb12d779f838029354ed0f8cb56aa96d118ede6af56b0e02997bd4cf59956bdeb5caa28677dd78bd2c4" },
                { "fi", "4b8d37ffa169124ed48ec3173535e1b26deceda16ab28bd0adda854bf71f7d43668a70c6cea5b75d59e33be84c5c15d70e11b3557b1e94a2f99b561b58d5e4b0" },
                { "fr", "28e59b6f216d50c10fbc1a62e34a29e0d1848ead40a3f82b992b2726b904b57ed38cf2b175b008b846bdf00dd5f265eda515e13cb6fbbe6e9540ce9bfa3feebf" },
                { "fur", "d02e22c5564fe3163695aaa2f14a3766111042409ff9d8b5464d131e51bee490983543fd809589d93af4cd320d5bca4ee1e98bb848f997ea169fae35c8f1f986" },
                { "fy-NL", "b1f606826245b5a40b689917451ea8d99188f76bdaf489f82bfbf088da80833981ce9120b5635587c4528909f9912675492fcb630549d2c9fbf4db8a67795444" },
                { "ga-IE", "6a816a39517a789d4778b0f498a31a2b4014939eb25de06518f89ea161275b380dd8b874c320be1a07cd479020cd6978cecba55cc197f0ffc384b3f84619fd6d" },
                { "gd", "a2e225a63fcebe40a041f5f77c634de4a35d6352ecaebed3de62bab5808f31641e4edfa16b5411489355c3f3d975d380c9fcc713b0f9e79fbc7b318d97070bd4" },
                { "gl", "e36951605ad658d95d01a4f631f9fc382df822267e4ff99310f079cf774f1b69a4c5e02f72c267ef843df29461cc48bf222f93664e0de3256984c1252b65d644" },
                { "gn", "d9a6438b2ef2b65412826560ea8a47de152e772f4324574292272d953d2102849746120421ce0cf2cbd5169a1034ccab6e2b0f5437592dfefffaccfa41a65f5c" },
                { "gu-IN", "d1441b4be54f1be42d3defc06093b658faa23cab6b655d999ab634f901bbeebe8329260a5d89d55867089ddcddefdf263f7c3848424b4a5b03a5fec1bb381921" },
                { "he", "41a51d800bc480cf55170bbb6e6449e2368e2c63b1b20fd5d4c6deb0b4680ed7ba7cffcf61d174cc8c51bbeceaaba7c702131430db078bd40ed72b99122012f7" },
                { "hi-IN", "01673cd0d9d1c0fa59cf1323b2217fc77ee63d1b3ea647f943330dd6c9e4f02c4efdd3d07aa1e17098bb72da1e4dd4274b90165e1869e2d2be0514a3f5417b75" },
                { "hr", "31cf4a94aaf769a94114d2a3bb65cddfe1aaa627885a65f04cf20bd56c56e18570d7f3b7d9804fc74f5f70267e76e1bc89ab9b766da8fd3630ca60a8973cd43f" },
                { "hsb", "4d5e0b08450f960525f250473b36228a75540684421f5ebe3a0fde8bcb6ae8dc3932d9662602b14b623ccd06422d97dabe002063c5caa35db7c8851d5bd60b96" },
                { "hu", "9f0f518fc6827aefd8ec1a0a06d123d803906a8fc9dda1c97d2c0697d36e3db4cacc07dfd4a5487bb461c65a3576b9b946c2fd212bcb23ae5d61849c99544bab" },
                { "hy-AM", "2dd48fdd0a53bc66bb1287600ce439b36ecf304ea753fa07b4d2cee1b228aacc213747f34c0a6393e465c056284409b00bc4de2d720175c73edacede05abea8b" },
                { "ia", "99ba0c91920260e8087d04613b8ded9a7346b282c432b9ba00912316bb10ce9f33ce22234b91930e00d3edda2cc95be3db8a4a2071b6af7b17461e155accf7f2" },
                { "id", "0ecec1fb2b2659f7d4ee60d0e7cf06301ff4b1c2b4a6540f3287c600acc8ee1ecf3558819e8fb7219c2a0432d10bb84aeb644f97f0f44283096ec9e0123348ea" },
                { "is", "d3a74c69db4481dc64cda90e9dbb256e34e0b4aa243e615e70d595e738e45e9cdb3a70c895e5198702e534d96be69bdee6c56b2ab83f19d0330a5bf03ab21672" },
                { "it", "a24dfa43e2d8f6ec3bfe22437803f66cd7fbac21b11a7d4e38c344b2aaa20706af3de6f0079ee9d2d60a5a868d380bb247b3639a66aa18631fb5298e9ac2770e" },
                { "ja", "312e94cd5a6eac2b672ba6caadac69649969c08cfa001ab728aa8a4f71be2991709157f0763de24640ab4f3fe07fe3e6ef0f92df251c175c5d2816987649d07a" },
                { "ka", "ba88abb8ec41d3d6606899dcd46e2cf8762309c6dd61eb9dd48a3bfc6ab4fda0829004d793768223fa415d9bb93a3ff6b5a551ca2a2b42ac8f95f17b6c5eb339" },
                { "kab", "cb58b8ec5efb1f0eb5ad97df3b4c0149de65765f3d258025489f5e5b7687db8ac486b2877859af0aaf81aa188903fea0cbe36e6a8055560812d98a61aad5f935" },
                { "kk", "06a1129e558d4adedac2f24db31722fbc40fac48a49b1b1725dc37f7b31fa1b3faa4d42e50848a6490584d926b043fbd44bbb85c0fbbb335735953804087d301" },
                { "km", "a0c32cbcf2764cf7652136e0343bd9123a0ec0b96850bba5de2d1b7aea03e17c51c05ffb112b9a9dbe2a10bc4403edd736a2f0cadb9e48d6c20e427aa4e92004" },
                { "kn", "c106e40ee9aae21995c01279ecd72a85636d13704fce1fdc082e76c6e89fff5d4b4274751cfeda151b5470c5ad61c3c7f11f1ccb49019c2348f807624982c93e" },
                { "ko", "bb266a16bf1184464ebc3a6686045c338aca27ee523813f200cbb323c6688ebc0ad8f8ebb6daee7453cdc7fb2bdc3e4e214e486227dc2788d773d18907e300b0" },
                { "lij", "8d6797fc838bdd929b644dddaf304ca52f178db2641b98616585a4b6e6f59d41846fbf067488359e647af68ce7130ebc3df67ccf17131b6d2773cf45bc120030" },
                { "lt", "52a8c41c8b257435c5fe8c14cf75cebd6c8c9353524299d9408f0c8c59914f23d5dcbe19a170df14a4cd3eec8815fe8cbcdbbdd46f19f1ecf0886cf2a27766bc" },
                { "lv", "337d920a30634616b71c950ef911d27fd3d07a18a50471ae2bd8fbdf41ee3e27635b2565f5037ad79b17391d87a7d97aafd632bb27842ecdb982ed1b5a9421a3" },
                { "mk", "8a7cfe900db9ae61bbae39ff38ec59919128e993d20d494b22f638147da9659a6c789d7de91de8fcbe9c027d6f0646386549d3363596f7f5efab283dbad08b77" },
                { "mr", "4d951d53f173bae6fd14828b0501c828090cbd90f22a577074f69ab59fe13704edf9f5ff996f22341077d0a4534e06167ad2176565f6ab8f240d89b773549d00" },
                { "ms", "43e6ac89e510a8a1bd28470eb30a08ade029ac2a06413a93347fbecea730fe6e3b3ab7eca8e2392bb737cbe56893a5b41e082bb103bf286764184bf3c7cf4256" },
                { "my", "de03ffe34bed668b348747c95753a7f5f7a885f08b479b2282879f2a4bf090d697212f55564ed58701e9a90b3bdb133f16c1c47133f11235c244eed3d2e8d125" },
                { "nb-NO", "a16962483ae0055de5b9e540ef042e536980a3e6fec98fe8df43bcb5ace7d29075c028158ab339b68b1e4e0bab7b6b71f5539c87ea726977702ecff196694408" },
                { "ne-NP", "f706925c7d1c2e29a77693eddff4f499bc929b587a0dfafda024966a184e035850d0ab6178d51f28d759b80547c771f7841183e925afb0b1a7ea6dc099454628" },
                { "nl", "a5a815fdbe91d728d8181eab342d4fd91dd7cdd606d123c5180a9305d1b7ae1769e187a103357db013bd134c2ec98a7539d317ae18e0412bac6bd7c08dc6729c" },
                { "nn-NO", "9665ce5f840b71783dbc4a7a87e7dfb3daf4f1a7cefc6342b929f9d7dee0ebf4414679c2c45481e3f87321e0680d6aeed62b27d6d07def2ca69e322d2718c37c" },
                { "oc", "47d9208e91a7f8f2cdd68a04758b4ec1288489a9bdf2e4e65b9aac74b9c21217f7a94be47792e9b964583be5b636b624cc1307ad6fef860617d52acb3a3e484b" },
                { "pa-IN", "56c3125d1edc309243f42ef77a1e5661768b4c42db0756c0586160c2c2fd0d268c433a459d3bc0b60d4b85987c406602251eb469072a39f1f8de06933a20ea98" },
                { "pl", "884c3705fb9a518f3702b9cca588838bcfe1d83cea9cec7d5ba93366ca6fd50822fae5ffcda052d54e112f0eba5b0d9cd9c24411d276d94cc8a2ac6016b151a4" },
                { "pt-BR", "29182931625941aee9c03ea97789677a26071fa8bed9e9cd84a9d6fa9338ce1954b150e779955d3ad98ed9942e5f3115d294ab85d1870eaf1bf497826b38497b" },
                { "pt-PT", "92e3dc0dcc7e9b2c8bfaffc8742201056a27027a3e87310eae3667c6084084ebe48908b11e0f2b31d9fa4b0c65eaa4c9706044581c69ca352add324c7b671933" },
                { "rm", "6c57386953d34c6f6a967cda5099b9823d6fb51585b723d8297fdf1da222cde8aa80f03dceb19dee2f5f3fabd52d324063fed17acc94ecf327a56bff31f5e09a" },
                { "ro", "647983daa0d2d27b62744e34a98369cda7cf7a5fde906219a86005a5e254f6aa02615f8c1264083b61c471605d2d07b8f9e74f38c513dfd4127335eb097a11b1" },
                { "ru", "dfe8313437f259123eef98172a5aa28cfd2212012adbaf7f3ae0d31a45abc95acfed9950458100151d27dbcd025fabc0a379ffab0a1b2623b599cd9a15850168" },
                { "sat", "6e272dc0733136b21c9edec1bd36bfa0fbe1643c965a69637c26aee512b8206fe3829d2827a9f498dfb77c706814ff838aeb621ba5134b83b24dd225d6d0a171" },
                { "sc", "a8583800899bc13216ef6e5d13296e93e562cd2617a274a292e1c49214b2daa422b54b5dffea0217ac896a1e18295ed64e2ffe3799c264b3af22dda41d199b6b" },
                { "sco", "4957b0553b26c4ddc84b5845cc6f6c02250a76b625b3d0532dcce903488d6b155990f83e9cb34b2d53ab61825110ff2feb0b2443c708f078473ba654d291d9e2" },
                { "si", "bfcbf48b9ba1076a9110db76fc7f95561c198c6dea72152f3a43557a25d5a8f161b6b61d592af927b8a81129c6a51d912542820f5813c239d1b8e8cb1e7bcb5f" },
                { "sk", "ed10b16dcf90594d4ae59910b98809156ea325c1062e46d98d012b06cbb55a47c596137c553d8edccf1f5a45880db24c50215dfddb0c9034b609f1756433d3a6" },
                { "sl", "e4a8d0a95d2810eee106bb2bd84dde67d59c036558e87e045303e3c05eecb82b20008e65041c82d49d6ed9e45e045eddc8b0c6362628aef4469e8f8100f18140" },
                { "son", "40cc377f1e3c1a599772c392a1bccd867c42b76997a74a6629999627104333c101e5bac0a8d97281a001e44f155cfe231fdfd51b08b4b948c2db2ced928918c2" },
                { "sq", "2a99a1797a0cdc3ab026a1a13e86bc6050d70a53b47b984fc0b30dbdb9ca56287783a54a139e38d30cbe51ca0ee1165def1461e16d5a772d72cb3354eef9b8c4" },
                { "sr", "1cd082f0e33190f80131417c9268dd30a18366e49f0a2ad1a1663c0bedf757fc001be59d999e794d1e9862c1e4728774dcb6383ea106e2dbbc60e45958e40073" },
                { "sv-SE", "c51374739740d4aa7c820e922602f8ee445dad86e9483dd0c68e56070ea541a2f9f678823360b7cd5f7780d9515036d6bf36f00a8209bafbca1116d18adb885c" },
                { "szl", "3eacb256b10b451a7e7d4fea48c833cb0eb2738efe1c62d8e879dafd252763719b71175de0e716d12c07a3412c69f7c86324458083037bc19f2d1a065b6fe463" },
                { "ta", "25415208935cc9d4e5de75bdd75102ab6f0b2b7787d3b778152ef9c78c68768c6d67a361fdc1b7357585fbc811c8eb01a9a3125b7f58114f8e58a522c46b2108" },
                { "te", "c12d2c003bd6a41904362b40f8d1a4a362a0bc866ce3db37fe2a5a8b61aecf78823b079b20c72ed10114710917d0be855c978bbe39d35336c630771e87dc9af5" },
                { "tg", "22927ef5b4881a1f2fcf5e6693951817baa2bfe849de7121359defacac3e9e504e69201b953399994d40bc11359beaadfdb264bec27e27eb2980a7bfbd794340" },
                { "th", "d5cf4331c2dd0b374f7b37a7a83c4e9980640b5e0a5c9a039853927166b3ae12d37a7e1b7698206a18f5b1d76d9ed7c28698d390d34069ba5437797da28dedea" },
                { "tl", "840f2f05af044ffcf063200220468de70d530e7d1d9663c6a101da91a5d4c110f0354bd9e482de8846dd2182a25a22fa11e9c0edfa7d9ba0a97198bdcf139d06" },
                { "tr", "7a80064ba0fd09b690bb2b7fb1863e8ecdf33fe19fb746ca4a0cd37a1d0e3e37edefc5021b225bb600a4075b43e396485482d52cdffc71d66303208ddbee4cd5" },
                { "trs", "1c90d15ce7f895374b426992f3fa98e020d32654b3164d9c853faefa588c4867d29437ece90767276bbe18966aef150bb51ef296064b83ee1436eb62de54f95e" },
                { "uk", "7d6435cd9295c5e3bc2f43ed9e09bef086ac0798e9e14533ff9d93cf799536dc7e11fa20b5590eedd51b1ee769c132a329b8e53506d6e45aaa0141779a22b029" },
                { "ur", "8080db82309fb5dceffd0d916447abd1948cdc05b3177c6af2e4bedb727d663e259c1ed3af33d0ebdf8877e627d4806c72e495dab45b48655c2be86b65b5f7a7" },
                { "uz", "ea09eb58de39facee61b1325ba3c3937bc8ce1f7c79763d3f5d77177a7cd2eeed40dfa924f0c7db2269f27cefe0f1ddcebcbed338ca2a936910d212e51aa6f03" },
                { "vi", "b2e6f877f75bd108cb5e9abe7ef07945537ee4fdea18982a6f732de1380985ff5d9e0db8e78e49fafadcc5fcc0e027c5f06bc204c82d591a428479f69fd6273f" },
                { "xh", "489523b1cf4c54ae4f8c47a9f439541832f63ba21eb5494668d07c045906293458da419effec26ee966f59bacbdb2697a42e98da7ed905bf7d6043dc85a120c1" },
                { "zh-CN", "53dd631bafccdb2ab0ec129845d13f1a3b30c081c1478983d36e8edfe6841adbef58299a61d2f898c683e6ce27b386a6605dc70da408987a73162a9800d19baf" },
                { "zh-TW", "4e62afffa788c359513e648c6ddab4c86ec8b10c39ceb0e0e71881e61a0b16a91d739b935263312ff47b3a9295bdc5ffe8310a88f819d64c4d0fed14b3804568" }
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
