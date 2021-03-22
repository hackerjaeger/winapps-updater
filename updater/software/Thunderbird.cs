﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2019, 2020, 2021  Dirk Stolle

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
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using updater.data;

namespace updater.software
{
    /// <summary>
    /// Manages updates for Thunderbird.
    /// </summary>
    public class Thunderbird : AbstractSoftware
    {
        /// <summary>
        /// NLog.Logger for Thunderbird class
        /// </summary>
        private static readonly NLog.Logger logger = NLog.LogManager.GetLogger(typeof(Thunderbird).FullName);

        
        /// <summary>
        /// publisher of the signed binaries
        /// </summary>
        private const string publisherX509 = "E=\"release+certificates@mozilla.com\", CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


        /// <summary>
        /// certificate expiration date
        /// </summary>
        private static readonly DateTime certificateExpiration = new DateTime(2021, 5, 12, 12, 0, 0, DateTimeKind.Utc);


        /// <summary>
        /// constructor with language code
        /// </summary>
        /// <param name="langCode">the language code for the Thunderbird software,
        /// e.g. "de" for German,  "en-GB" for British English, "fr" for French, etc.</param>
        /// <param name="autoGetNewer">whether to automatically get
        /// newer information about the software when calling the info() method</param>
        public Thunderbird(string langCode, bool autoGetNewer)
            : base(autoGetNewer)
        {
            if (string.IsNullOrWhiteSpace(langCode))
            {
                logger.Error("The language code must not be null, empty or whitespace!");
                throw new ArgumentNullException("langCode", "The language code must not be null, empty or whitespace!");
            }
            languageCode = langCode.Trim();
            var d32 = knownChecksums32Bit();
            var d64 = knownChecksums64Bit();
            if (!d32.ContainsKey(languageCode) || !d64.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException("langCode", "The string '" + langCode + "' does not represent a valid language code!");
            }
            checksum32Bit = d32[languageCode];
            checksum64Bit = d64[languageCode];
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the 32 bit installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32 bit installers from
            // https://ftp.mozilla.org/pub/thunderbird/releases/78.8.1/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "331dea1ffc6442856a39771cae9f908f7bb185f7e71b51ce7a71a69d0154a788c6a3bcf128bc6ea7b3f5090c30184e33af645d78cba841f01bbf7d7b33f1b794" },
                { "ar", "3ad08cd0bde5e1aab7eb968b8606364dc4fd7d1fc5d9ecda5e74ba0c2fe3fce7875f48539934dca202084d678253b85487a29c2b0066fd50ee7f1d8be67f8e81" },
                { "ast", "bf43294d491851ce6448925ddea0ba161ee53076f5abcacd43f85ed284f8dc05f1595dae71ab2391a8630941c129db4c4f627687ddd3bb029120c0dd2eaabced" },
                { "be", "779dc2d6cc76f7c434c8c56d55e208c4accef27c5d6435b616802d4146093f68ff8f71e25ede6145938b6651bf6d4dfda19f0d0dcd0cfe437c30521cfba9b7ac" },
                { "bg", "2d04c93e759ce91ba5fe5fcae872ddc7d61aae05264ace0d5c02d70495b0337df099f33ab8f01d24b1da40e5e98388691b59b899151560d7efbdc1a18cce164a" },
                { "br", "b5f4720d4229a69fc905f0503a529bcdaf752c8145bcf52c5fc6afc1b9221cd257c7d6da78dc65c4a603bcf43ffdb2bec990bda0bd601427236e569d7194b335" },
                { "ca", "2677e7dff14f547789be8c7b187bff1c1f542f01402aaba25d89b70416d15c2ccc46f3586f697527341acc81c3c3e93848f684e36aaf75621239c6fc7a7278f7" },
                { "cak", "30d525f34031a292ed14e01cad18afc0d30e83d1a93ec66c0b04af9bc34d2c2dbfd950d112ccef941c1c8a769f6e10f059c1273ca92235f0250c1fbdf5f571d1" },
                { "cs", "5dfb13cf201330444f8ddcfd6803534732a46f52f15f580e0e1337bdc9e7dd8bbf23e642e7f67a551ccc1c79d0c27d75c20dec6e76a783481a560b43606e45c6" },
                { "cy", "fa92e900999172bedf26a0c8e49325e406b76311c8542263530faa85c4ce94c83b94be843296566afbd448c7ef009f2d89cafccd59dca29255932ca52a5c7cdf" },
                { "da", "d7df5f176641a4be9004210add03b61c89ea9fde9846355c8a00290db8a0e70c29c5c4bc53ecbec737f73ba13956faf183460e6a7247a7f1c51043a08e5d8f62" },
                { "de", "5ade848ee2c0cfcb25e39a8340050f8bb261d35f31c0283d9fff62f0d0b49adc55643099c556179cba7c247e9d3de4914ca4b57d1ab4de35d0766553b22722ca" },
                { "dsb", "0be1d58a50a8615927a46a2b1258220affdf9beef003ae6bd07610391363a9f5715b7c69f95d5e220149f64e087604fe619a1684ed2341eed99e79dc7c87aad6" },
                { "el", "36d78815810472a3623648304e1f7c4dfb78abd079d64b12949874b3dcec0531fc869f1b6b43485f0807f3068030d7e4ac3bc301ba3610a820ac471d4db34c8c" },
                { "en-CA", "e6149abc29dedcac14b039bbb545c1bc4d8082a6b34dc0d726707a37369fbf77fcecb0d66b61d1dd0de14454a34b0c9399573f4717d8b3aa6b98213c42d56af1" },
                { "en-GB", "a6ac78f11e4840f7a2e715c45474c92f481b76b830545b4bda38581da4cc361fc864db8231c45791c30fa07b6634492959adc1168c18f9e35eadbe52e8692c01" },
                { "en-US", "6605991a2837d71adfe9e38ae5c60dc42c4e677cfea7a5436c1db323b717fd46690a5b13e1834d1f46a06e9027402797731d9bff9eec0daf743686d3075b93f1" },
                { "es-AR", "ab9bafb161b7640a2bbee61a5ff729611bdf975d2883211cb57f71e95942e82ab417ab5565c84a953974f62290d138696e3daee1ce50b874406c62d8bfb8bb5a" },
                { "es-ES", "ffe663b82cf42251a1009ea9a08ccccb628794c28e3213d56fc59b172984563dde8a79489cb8f2ead8b11008a58e8d0a8a01beee1132fe022295acf483a10cce" },
                { "et", "33123cc2bfa0b8b3e4e6d81196adf3ec604fd417c898e77c48694bc0b2a6f76a09f5d4838d8b29a18e20b2475ceefb7a9c0b06f99b1c7e3f1904784863ef7d65" },
                { "eu", "c88394f2f75106867f023fea5501e4887b9f4665e492510c3d461560b6289b13a5683434219b4ac3f70df14da8ce186d42d1abaa2446a3a700b7c0504a4ed3d0" },
                { "fa", "a7e50453367e20241fe05e7d9da752f2faf66463a0989ff6a34e91f091a011ba9a40adb686b001ad1113d1ee593b7c617839b6583bad32b674a7b7d1e5e05617" },
                { "fi", "bc9ee055d04b18ba1c4f203a291d5f1a2d0624fdf07cafe2c29d4e91de6411c323eae13b1d5f54783b7bad01934b0a06c981828c5d8fe1d9f43cf704a91b2b29" },
                { "fr", "d04224f2b2cf461ebf6f3352ce839578f4fdcc2c830ee58d6badffb22c6f701b98707bf49288a947d6a687a82c05bb39560aefd1a6f370e51afbb1387d7a4053" },
                { "fy-NL", "a019cd6ff86c9d397bea0985587e96b6b7449971155c419886fb7bfe8f09056007db1c544d094deb66e3573bf928a9d81576787a2e64058a1a8d9195ee975218" },
                { "ga-IE", "3f041d6ed37023162b1f5acafa5a9981874b8dbbe3fd454bd8751468bb14619713eb9e12adaf4bbc90d851387af2316936d15989edca1df7a028f1b4458ce7c8" },
                { "gd", "303bbb077f086ab02dd3e53ed9e199c7e68549c92bd0906c4f5eb10c39c5ff4c6aeb049304ff8bb1c3773df28e691a6dba1310e4ee859bf19f120b614ae511a9" },
                { "gl", "9301457ba949dc9ffe6bf3d00d6c39fb7bb29f6adfa47fe8ad17b60fedb23eb3f5a90d6dcf73e756debd4a3ce954887970a6e8f47130a9a1c4b6f11ed320c8c0" },
                { "he", "37cb6f90344fc710428e8a9043349d298521471cc8fe59a8ab0db7eae2214fe40a9c793dc783fe9cf59b5314ac21fd09cda5ec5f2d5db42e47f416748b57e5e3" },
                { "hr", "51bfab40e8b5d80158f1346f52480650dcb4ba0f89a6d77500f926a85dc27ab4ce5798511878b138b1f06102d7c07ff5fd4ee0d6be94da6128d6f2ba98ca580e" },
                { "hsb", "9887b7d690ecda2812707cbe563128a49bd8ae5b6514f44f6c1151b74111d5c1beeeec6400d9080e568508698e8c016d22f798a271e99f4430e4bac11f4732b9" },
                { "hu", "4c5c5f06504d13268407d8ba4384def18573d1d6efcbd293cae4e7fcc1039458a60eebb8e39eeb97d77809676548c7fbb17cb09ec7c431d3c32b503c3278cee0" },
                { "hy-AM", "24e0b1e2c51b202e5d2266766ff81a6c275bbbc21ba41c60a5d9da372c60fa08f3d8b9b44f365d5e477b7e580ec4fc9a4a22198d1415da180e052a914dc3eff6" },
                { "id", "14b8ef28e23c600340ccf90dea193a8d6073bb57504e077987fe0e8f433b65165d1820a0bb1165e1f1929647f569f87075b54fea03159a198058565338dbe19c" },
                { "is", "61fe352346e3a9734a82701363e2e2aed021151133d79bcd64390157e1eae08ed3a1338cee889e9dad0b77ce6dc2a539411979e5191b5f6bb135f948a515a29b" },
                { "it", "ddb5b11bb9dc9dd75d8c186e2bb9261ae6ef476b3a7efd502d2bb93cbbf920d0d879273e3eb314bce1d7be33facead25ecc28119157a845cd5947e6638325635" },
                { "ja", "5f9b5e2483a29e9007cf1443907f70955b03cc9afaddaf40a8a2c47e3f24079fcb351ddbeac00c4522604d9a10525eba9cdfe8c831d5a7d9399d24399824d373" },
                { "ka", "6c0fd6c924249897774e66b545dc18cb172b370b5e395e15d7d14655f67b5037927ea52c48a97a5dffe7988e170d63015e58fa7740c20b56b5cfbfa858ec8e19" },
                { "kab", "978301af420172a92aa353ee3190c2e0b916dd3f978a6f55f1c83aa282925cd5c2ac2bf1f745b63c162cba1cdb2f24af4c4055e3008c55206d1c04d260b9b571" },
                { "kk", "50f0b2e5566e4bc34bc8e236e5214b05a4a4b457bd7a75d165d15880d0e9f46931048be4b613edae29fb706ba282c54f5e9cae8839781a4591cb8db69b862fed" },
                { "ko", "d7362212620ce670ccedbd39f29316e6a980503fbc3a5d10484782bcca2bcd1d4b35ceb5c09c5d2d2c76225ddb316337c572870e22fb7c0424a0fb4c786b298b" },
                { "lt", "da9cf40c193e410b5b4e9efcfc7becd45d684cb6667b9c7e09e4f20ec2f029531986c493cd225207475f0d7133564e9dd7d10d068c4436f2d3ac0afe461bb942" },
                { "ms", "2ae9fa138ec54eede17a97e93aaf021517ada7904f1946836d3463c1d95f77f83d4d76eadc5908895975adfb0713cfd261cc1d25b6542f48469d586ef0973aad" },
                { "nb-NO", "ad62186c3fb1c69a3fc0538a38f9b4a8270a3c1ef43f2076e86be67d344d756a95635cca534309b5f0871f121abcc5a07ea568298240b1bba2d4fee48730cfcc" },
                { "nl", "a17e687ad3d631eb37ab9da11ca438dd46b3e55719c2281eeadf45e4c281497021bfea90bbde7faabf2e29bfc3e846956226147b92ba86da4d8d0cd5a3ae1435" },
                { "nn-NO", "6a9ff3ed8c69c1ea6cfb1ead38982b7460e1d702b433e617e3ee82c825a31d02bcdb55c2078e800cfc9c8bc53d4fb177b5b3f2eed57505880b1c06cf383171a3" },
                { "pa-IN", "8b253e69f60cfdd4f8f9feb9a9cad15429a2ff148a1d0f0caa3ae37a943cb84ecbbb5e8e308ea4b6360a8ea36627672d1daf125250d6d644139e870f17bf8f53" },
                { "pl", "51fbfc36755acb575aa368f9d38699b04d25b8f32fc6184840a0e5c4543316fee9f151aeb40ae5eaa02a807a1bf458602a8416c06a3e37be0fa38d4cf3b87a43" },
                { "pt-BR", "bdec1ae1b4c1b4baa3f2932d6d0781ea7c6a28e5af8aaeca39a35d41a0b2ec76d5ec489c67256f561c7b6616834069d59c988eb1a75a370115f9dcea2aa569e3" },
                { "pt-PT", "6d7f83794f5fd7039d78b87261232746d8af5541daba3c2a0925ed2344c29a21bc17e046976ade577c5f3fef71e7fb4a24cfa6805e2ae13d9c5ec2a1b099a9d7" },
                { "rm", "8cb4d189831402ec56a9730c895327160cb5f9af6a41aff66575a848f90fa83c3c7b98fc0714e00cbc03239dcd8ed7e24663272e88658fd4934fbbe81a123395" },
                { "ro", "5e8a790d03353d17838b24af4c2494d66c91f0dad050b230ef5983e7da2cf14538b3bf9b2eec41a5ada8fbc1f57d7e5d706727f76244ddbf680a833b04436429" },
                { "ru", "5e71337ebd9142b5cb228268795612ba1d6d15e94dffdde405dc0d939b72c9cea514b26fe8710fad644085ed2c031d4fcf130f6ffbd8178e3bb5cd90ea146502" },
                { "si", "cc5b73faded47840d2139886e744f2bb4de9e1a1681f5e64ea4d64eb3a18f8b84d8b3838d1e9aa02adca5bb6c3e877534829e978ce80f7ffd8a7bb03ea146d92" },
                { "sk", "bbdabe230bcae3c45200706c8020851e3ea1a3ebc63bf0b6cccfd2f7e6d3e5d8425835ba4d59f64ebe61ddd2a69e70a51635ea4a57d135fa2f1962888099f79b" },
                { "sl", "7f19e3bbdb94cb1664f408d4b22a08fefe81eb20fe5f9343aac1a20c02d9dd86ce38a5c1326ff60d3dc407bd7f8801f3cd47b3bf8d6e940bf599c5f0dab9143e" },
                { "sq", "0583f4105664a9a130444a28a57b7f6c5f6c68f96e9612303c179fa24c8b65ee5292f73774eaf8b063b700c5f037d322a498f3b517d2e1d8315df25a98e8d3c8" },
                { "sr", "142d4bede3232f0743e1f4416863a6bfca06091a6c0f6d79b202f72dff124de1960f86be614e871547d5f383c7466cec21415263af59c107e82ef4517b74a545" },
                { "sv-SE", "750356f08e93ce06d22904884fc5dc8a82b867a15a53e4481fb957d00d1c0e4e018710ed9b6f040131815601835af17ea46a312780513822f47d5a70a91093b2" },
                { "th", "b2535b5e464ded488cc95ed0b6f082a69842062b31765747bcf485b4f0b32ad9d684222375cd1719cb9a952dad6152b0a18e873c5d689d288f04c6a5dd49f33a" },
                { "tr", "e2f3d9d8cc29ecd38e549116eef769f1df4c7faa9e6e3e4cfe57acf0f022564e01d902f794eaafadd45d8de86ab1de8d435819837472b5da0c90053221065303" },
                { "uk", "798757f5d4ba3144799b9c3678042a9c9e6cb7977e35b388a82d645a573603d5e92c3b6ec41ea6c7eae7eacbce5e66c6670e2660ac8cafc023c12b89d8862165" },
                { "uz", "391c7e039c8e283b5c5d5aad8582badea2e30465ba3a80ce3975a59b2e40f51fb369a6a08e845121eb310d455e1439a0d4ac2844b11e9ff4f8ceec8368cb8373" },
                { "vi", "9827f7e1c5d7ca90557902c2425db18765f16fd22ff48d704f5fc1fc19efeaaa0b7e75213c4be8d3f0b6bfbeba27cf7f5715fc73c28dea5af19a649f3a7d607d" },
                { "zh-CN", "6e533b4f489f7c1bc16f24ec0f1797be6da2ea37ec8dc2ac9e48e6024c68eefa0b9e2bdff098219a4ab85e729ad8bd24f88760f322595f7dfcda35e719737824" },
                { "zh-TW", "13171ee2153c6c2d01cc07767d67a62c633c5b9eb52a5315e491b52811b7323044f9716d19337cab5b8fc0d382ffc680f854ab99ec01d129434049be15f82d77" }
            };            
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the 64 bit installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/thunderbird/releases/78.8.1/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "90804d949912157d40049baf1d8defba41b42ac0725635548c4ead902c3716583578e43103f3dbbd5dfbd1f6809565b8a548d8d838c3fac93ad6010c8c23582b" },
                { "ar", "c41d14e046c5996d7c554815b488bfd2bbed44d5c7f47461bef8a1bf9bf69386e0cd18b8cfccd2f00b71ec14720271b715b40071d9b02b3f9875a67466d76056" },
                { "ast", "67eb89794e90a69c8ace811c34a1352e5fc29199b530f86ec839cf7dc959bc8daa82cb6acf0b4059972f3096005b87ef9ca4aa8c381374906c753bc15fc572a1" },
                { "be", "258e9531971e5da5f160d80bd2e7228f7cb6f07461a8069a4bc105dbc0e5137d1d1266e70e3213aed568b22730b6c643bea8b5e9ce2dcfbde0dd3a5cdaaf57d6" },
                { "bg", "73d95d937136b03461c9a2fe8f763c0c4559094a2d5652c8914bdfecdc0a977c5ac28a13e7af13611889d101064e54a81d473ddc0ac4c1f1bfb8864bf6f0c65b" },
                { "br", "8161eb431d2e8dbafa7b8be2f56ba9a719d37115c0f4b8dd70bf1a702e89cb912119b41640171d1ee295d6f459974fac93cbb79486e9f7aab4558119f2110e70" },
                { "ca", "a6f8cd29160819ab21c0ccaf2413928335fd2650456b73f3f6aee3feeb8a5d543648c3dda224aadda30c1860856ec15d16d5188932d12439135360ebda4eb679" },
                { "cak", "492cac68cc423f1a81a4e13be2eaea4fd63ad88aadde201842e4bb80b24ca246315135880c289cf8dcd9792f1e8f13049c03b6eda8ada948404510191545c32e" },
                { "cs", "49dc1a09ca85b00e49ab441e932cba2e2c7b6c8211ce363adf1c5e63929a3600c7389c95a51b8c68ad707da10ff509079947786a1c142c069f3b899acda5cba6" },
                { "cy", "402787e3e1dee27a2d3b9de4851f781ed039d66e844744d99dee0b7f34a6aa66596e74acb3944f6ad9b02db0de61291aeb4cfc5c451aca2acb73d7b7b722c3a4" },
                { "da", "76e74fe8ce5e7000d8669419d43f8e5145a3955c9ca571036a899edc28a8545c7978be5dbac30886733c204c22c635abde3937d305d3736162137e2483c89191" },
                { "de", "e6c80930cb79b59458e83293bffdde20d0f7719a78f715931a101cbd77d0c00698a87646a6b5d93025b65d358f852d3fac6efb28d00db571290a83af3d96af8f" },
                { "dsb", "f5d077857cc811fcb379be365d96da1dc84541245fe67cf35f5466d088ea25eff77c916ff9aab2a2c99696a93b839f0529a639d99298375b345a500c2a463a99" },
                { "el", "ce2bc79bc5d83e6c892731875edddad2f1bd9098505c384419c918391ec9e7be3d5c9fe1474eee55120efb6ceb73a56beec6ca1cd0fc2895ee22a5888c0ad10d" },
                { "en-CA", "3ccb813bea763d916099839229b6d514d46a295d46a74c545762eb9a2ed0d5829cc2382b0081e557ff3951c18912e2be9b0ec63cfcdbe234558960cb1971557a" },
                { "en-GB", "62049916738ad0f5e3d3533be90c4f1c4376b5e7f184fa0d13d61000fde01158bda29589634d270260b5209cfb5cb9e9d95feddc56c7151b415e84166b1e1d41" },
                { "en-US", "dc1e3ff3eaec205ae43f2b09975198eb8e707eb04b2fbbb930003743e6434707d2c4c4a0b3826cfd9561288ec7a4972866463e8bb431963a6377afa23fa741e2" },
                { "es-AR", "afefd3f08c012a4613779083786572925c578631de0f5bc01f55207fb806958bfe19f42306d36d4dd1e0cbc7ca28c43823524a720c4241914653e27c362f8da8" },
                { "es-ES", "e80c998f4119c12fe1689459ad14e76a433eeb6110893c63a6e24ebef2dab735ce00f7a3f51b86322dbb5a3f49a5132fe6985107ed1f40ddca80957a307deefe" },
                { "et", "501f980d649b02b8dbaca982a41ddc80e8ba510a4a90a56cc4c37621ba1b67684a82d1c2fca60f0b8bf5768382594f6203d143353f3156e71622001ad328ba93" },
                { "eu", "0dca114d834d9be9a3d30f3851d89700d651febc2c2a8a6794670c0bfa46851da4d0bd2da5ee74fb16e23e798d6fe38c8365a720e65e174dd6488d97eac86be0" },
                { "fa", "5cc0ba4c86b5ff26c96eb41ef06496b446f81f48f0634e1b8e830c8f551cd7f9b454517d880557dce891b8b476084bbfb2e1b3785492f76ed525b8333a17604a" },
                { "fi", "b87634199e7dd95a4bd6506be67d2b0f630767718d5f98981ea197d402be65df3f1e79f5a00823d693fd40caf7b0c4759aa65b4ab5e8e4ae12b040c6ac212548" },
                { "fr", "0a4cc6a2e58cabee9c340c58d037eb2a2cdccf5d76cd2de448feeadd58fa2d90e1db6d01d36ed60e8ddf9ea6d7e4d36739c7869845840dd9774fe244cda2ee52" },
                { "fy-NL", "99e0747eaa907c5f08eadeb42fca1b1f89c482f28453c0980b722a2372ca4a489d87e945824e1581929d294111eff36cd2b083ad54428bc81448631c2f8a78e5" },
                { "ga-IE", "17d76afb196630f9adbf8cf11c15ce809cdb64125b1916c67068d91127aef9a4269bbe6776b134b3320ed71790e65bc9c31fa13b4af0acbdc8a07f98975f9247" },
                { "gd", "f45e398a877dce05f400636c17a9f9be57e64e1eed8f43bfe954552d6ad09a30705267674109e60baa3a5438fc19616de3a2c75838197531b1d9dce9a27dd606" },
                { "gl", "d1e3f80e22b20f16700843d8436421c020609e0bc437fae4a2ca2080f1e185a53d55d4c5ca5e574bee9bc63285eedf4999c14131827e343b59623bd9ffd3eefe" },
                { "he", "9b1ef9efe409e3ce9cff65b02891c4fda4c7fbdc8a520581b2cce27d29270e4910d6e37136300fd8a7995507a28065974cc046c6edc1adf35a205fc30ca61d45" },
                { "hr", "21df3b0e12959f0d58ffeb63e986d33282f100c7676a5336d582c40d83d1138e75cac87d453e3e3e5aa9ddc2c3de546089f40c72124f2d98607a03a041459199" },
                { "hsb", "e43227f96678084943038508dce166ab9e13a34a24aee33b2ba9bf69d09f466b209f6582143954f104e1f564af990ee7259827af240cef30bbd9f98e1b633441" },
                { "hu", "2091dc252cd9f959f23f7a32f9975f2fbe90c3e7947d0c363eb976958d0de1a274b65c283a6ee084f5d0204f60dd050a72160b820a9d54b04471d37b386d1b74" },
                { "hy-AM", "723cb59cac64add3b859fc5b4460b9aed274b5862747b370a5f6e36735d6b183b1ea280e02dc3303e4074b5447c82ff2f8ae0c6e8aa5c295e7c84f485243d1bd" },
                { "id", "5b1bee126c84e6a4e4da693ccce033b7c95a648e699497d7139d72d0599d55afb9690e39473d5cc909e393491e938716706435fff7f059153e2635ed16cbc381" },
                { "is", "0fe7ddc24badd7614a934988511ff4363e27ceb6b8c7b578dde9e57565edb48a6e59b1523e440890c38a8ffe07dd1d89fb9f1577423997b38917eca5f7c3357c" },
                { "it", "1b34291ab451957e449f6c1387deddfcd05959f7f12bcc09003f3e20fc841345de53ea59e619378526c11605b2f82c53e53d0fba7f5cb729aafc2bce1c9c8389" },
                { "ja", "d232a3c55d46e4e69d9262cc99915a2fe2db1304f3cb3beb494b6ae2f6f7b94f7a6f4b24213c63edc21c086da9284b15609b8ee008129729369b5dba41d89ed7" },
                { "ka", "f466f9979f0f14ac1e25e98579e5e0c02f7a80256608b82cb4310a3d800d63570a855839e4cdfface0c76b2b68723af9f5c06f940b2ca93c09d7250be52b1cb5" },
                { "kab", "bb6edac02326c05dfcbba6f74a04827caa68f28e2d8f993fa12991125c7d3ceeb0787022acc79740e70c4d501728a97679acaee6ff3c2988c5829d42d29892d9" },
                { "kk", "aaba9d289e98f1c5ec543bc1f7263a5296e54dfa0d75b50e6578a6e6ba927f150f83eb611a5e7e49863a26c5f5203fc0e6f3829348c90ebb281c682fd7c9e139" },
                { "ko", "f4e8f468b888d5ffa11e9fa4dac512ec3ad6f615b2853bab57bab9717aa3228d7894888c10dcb6b1e91b640c15ee007aeb4e78f581c1ccf44dd69b3a177f6220" },
                { "lt", "592ad56a68d0688b9b979dbfd796ab796413a19e394116135165726e0a5e2bcb3a190601e1ea5bcd24e09f6c62ef497ec200e4df67fce08bdc5f777d79870836" },
                { "ms", "735e701462cdafd4f1bc20facc10957043c2bf9e6a682b6f86bd9b84506adcc4affe0d208b504a353b7d914bcce7fd5579819a9e6b9895b1284560605ebb9a4d" },
                { "nb-NO", "9c435b5138b118192c10bc873e75f5e0ac09829d9851b859f69a906d92556b46dc7a271267e11a439f06255cc08234d7b04aee071c8bfe064b7fa3815c1d1b3c" },
                { "nl", "0a8978b621bfd53556ab7b9a5906bf288a82f4c655cbab7304cfbc884914fa037284d2f727b1b53e911a8f76a04780b4d9f7fa1b80d5837dd4e04c02c954534e" },
                { "nn-NO", "c86e9e84bb8bd155a80fc7f3600a170c408851f9c2f02b8cb4aa43b2c869f019c1d17d637c6124898652e82bf91a8e182a2b76997ed71df24e28661f6b8ab049" },
                { "pa-IN", "e7f195511983c372c8adb9d233209d91677c27531b4fcb3d8b8f61443c399c2677d9e566f73a78c05c8970e4bdc925fcaf92d45a8cf1cfe6669f6eaa13b9efc9" },
                { "pl", "6b60985e0d6dc85495dabb6a9f698bf05d6f9743c19336056953772e2478838f3c89374e559a40ce6c309c711b9ac50413180f2a91498055e16f34a08225d4b9" },
                { "pt-BR", "dd08c497335ae14871bb86cb98c3a9187874100e013b4a01934f4b024d6e41e6fefb5dd2f5921590bce29908ca95cbce82816e02e8ce99ea52e8f2412e68fbea" },
                { "pt-PT", "be068a94880bba69146755330515819b358c7dc474d943d81e9909adbd34b544f24748df73ce479e640ed6a05ec69cf73f81f9c5c3a15080c8e0d689f0eaba85" },
                { "rm", "516bf80c2ba1780e0c408db4ecfbb88f40a7de972916fce2b9d8f019e6ff10bd2a89b424b7eaa10aab114621a386ca648a78cccdb29a799679daedba7892a743" },
                { "ro", "09589830c3db9d26d22343e11deaeb3a4ed9a179ecd723fb5bb85764067af3a27afc4ddb5326d4e89ef33470515aee5763d9903df57cd564e616f1a00064a550" },
                { "ru", "fee8c6f4b8d88b72af1c268f35c5141f37326565c872453f1ca7ff358a36493c29e4a69c386b2ac6d4d38d6776309c0f3d4271acf1ef2b3a15dd856677b6d1ea" },
                { "si", "8f609a0984e29aade12273b620b2c7b017156a3405eb5267497e124d337f32c51fc2bdfe77edbb337a952a8676fe41ba42ee857bc42f53947ba14bf28f3f29ba" },
                { "sk", "7f8e4e477348340d90187422ed140f11597e776730f97dabe4cd50c538e0fab95fe1b789fe138a05c35c82d0ec8058fe4eea142b54d2843ab50d6d729e48fc42" },
                { "sl", "aed45082727665dba70bdccab0c3d569b47ef779f6128a3db4c837ae4bf1b246b16b6166466531119c7509a98e9ccaee314d7bc5ccb5acf74dd8871ef072e799" },
                { "sq", "d55acfb6de577bc7fde67b8a79531cf34a922fc9208e10bcf82157616d4f15fc0d7483ffe0b59e4574c872a5519d495c41f273904a7007213b4e23e932d44313" },
                { "sr", "5326916091dbf224d8de91d893b93d7ed035875378b2a73eb4f45031e7a133a6ac29fb79df74a603db85b0ffc35095029b302e232d13b427ed426565db5a6c6c" },
                { "sv-SE", "8d54c7ead7e68fa2217dd0bbdd7db6094fb8dffac2f6faf55585c49ad14609caccf6544e77483823739b73f44413d40e4fe5538f58f31d7de3036defd5b3ca7d" },
                { "th", "1f7c586e3a691391a87f0db0bc184823ef5abe71d9170a4cabfc7d3f294074f7785bf377c22da5e3c0c6a71c9dec7d66856447b314eb74f2270b785369650d40" },
                { "tr", "9279cb815beec91abd98020552db38e3bacc2ef1911ee2bdc2e2c0442bc821ea396c33e98043864f44231c0816da7e4f21f3a5cd13c6df9456bc6276d84d63d7" },
                { "uk", "503ae70c106c887c0cb2de34d6ab4b857d18f75565ffd1e7671c74d4e71e962840839811ce1e3164358e3e51911ca0c0c34bcd03d4c4411f9176d8d490459f00" },
                { "uz", "4cd31bb7b114470408f2349f64cb738d275a5f6bb9d180eacf443a88af22f2c5814531af95d0c5145d1bad894cd07f5f1c02fcc31de7715de09b5e16bb94d6b6" },
                { "vi", "97c66bc6fb3f51c4aa87ebb1b789de676271d4c9f8ed8b1c33b68c069633af02c00d294d98971d04d933ad9392681ee2e54ef94984c91d91f4accbc2d7e062f1" },
                { "zh-CN", "d37100cbab49469134183521daec1050ce3ae8b02e659e22d09c7e5597bce96e7390b172ba88793e4473c162226de3ad17674084a59ad2ca4d78006a0cb59a05" },
                { "zh-TW", "ab278759756687a7830cceee3dff868af79545a90dee2c7cf4c365edbf2e8f68998b4c1e974b76badcafc96c8f3c0ea3d0e5150a9d140030a3f206330809328e" }
            };
        }


        /// <summary>
        /// Gets an enumerable collection of valid language codes.
        /// </summary>
        /// <returns>Returns an enumerable collection of valid language codes.</returns>
        public static IEnumerable<string> validLanguageCodes()
        {
            var d = knownChecksums32Bit();
            return d.Keys;
        }


        /// <summary>
        /// Gets the currently known information about the software.
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the known
        /// details about the software.</returns>
        public override AvailableSoftware knownInfo()
        {
            var signature = new Signature(publisherX509, certificateExpiration);
            const string version = "78.8.1";
            return new AvailableSoftware("Mozilla Thunderbird (" + languageCode + ")",
                version,
                "^Mozilla Thunderbird [0-9]+\\.[0-9]+(\\.[0-9]+)? \\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Mozilla Thunderbird [0-9]+\\.[0-9]+(\\.[0-9]+)? \\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/thunderbird/releases/" + version + "/win32/" + languageCode + "/Thunderbird%20Setup%20" + version + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/thunderbird/releases/" + version + "/win64/" + languageCode + "/Thunderbird%20Setup%20" + version + ".exe",
                    HashAlgorithm.SHA512,
                    checksum64Bit,
                    signature,
                    "-ms -ma"));
        }


        /// <summary>
        /// Gets a list of IDs to identify the software.
        /// </summary>
        /// <returns>Returns a non-empty array of IDs, where at least one entry is unique to the software.</returns>
        public override string[] id()
        {
            return new string[] { "thunderbird-" + languageCode.ToLower(), "thunderbird" };
        }


        /// <summary>
        /// Tries to find the newest version number of Thunderbird.
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        public string determineNewestVersion()
        {
            string url = "https://download.mozilla.org/?product=thunderbird-latest&os=win&lang=" + languageCode;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = WebRequestMethods.Http.Head;
            request.AllowAutoRedirect = false;
            request.Timeout = 30000; // 30_000 ms / 30 seconds
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode != HttpStatusCode.Found)
                    return null;
                string newLocation = response.Headers[HttpResponseHeader.Location];
                request = null;
                response = null;
                Regex reVersion = new Regex("[0-9]+\\.[0-9]+(\\.[0-9]+)?");
                Match matchVersion = reVersion.Match(newLocation);
                if (!matchVersion.Success)
                    return null;
                string currentVersion = matchVersion.Value;
                
                return currentVersion;
            }
            catch (Exception ex)
            {
                logger.Warn("Error while looking for newer Thunderbird version: " + ex.Message);
                return null;
            }
        }


        /// <summary>
        /// Tries to get the checksum of the newer version.
        /// </summary>
        /// <returns>Returns a string containing the checksum, if successfull.
        /// Returns null, if an error occurred.</returns>
        private string[] determineNewestChecksums(string newerVersion)
        {
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            /* Checksums are found in a file like
             * https://ftp.mozilla.org/pub/thunderbird/releases/78.7.1/SHA512SUMS
             * Common lines look like
             * "69d11924...7eff  win32/en-GB/Thunderbird Setup 45.7.1.exe"
             * for the 32 bit installer, and like
             * "1428e70c...fb3c  win64/en-GB/Thunderbird Setup 78.7.1.exe"
             * for the 64 bit installer.
             */

            string url = "https://ftp.mozilla.org/pub/thunderbird/releases/" + newerVersion + "/SHA512SUMS";
            string sha512SumsContent = null;
            using (var client = new WebClient())
            {
                try
                {
                    sha512SumsContent = client.DownloadString(url);
                }
                catch (Exception ex)
                {
                    logger.Warn("Exception occurred while checking for newer version of Thunderbird: " + ex.Message);
                    return null;
                }
                client.Dispose();
            } // using
            // look for line with the correct language code and version
            Regex reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/" + languageCode.Replace("-", "\\-")
                + "/Thunderbird Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum32Bit = reChecksum32Bit.Match(sha512SumsContent);
            if (!matchChecksum32Bit.Success)
                return null;
            // look for line with the correct language code and version for 64 bit
            Regex reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/" + languageCode.Replace("-", "\\-")
                + "/Thunderbird Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum64Bit = reChecksum64Bit.Match(sha512SumsContent);
            if (!matchChecksum64Bit.Success)
                return null;
            // Checksums are the first 128 characters of each match.
            return new string[2] {
                matchChecksum32Bit.Value.Substring(0, 128),
                matchChecksum64Bit.Value.Substring(0, 128)
            };
        }


        /// <summary>
        /// Indicates whether or not the method searchForNewer() is implemented.
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
            logger.Debug("Searching for newer version of Thunderbird (" + languageCode + ")...");
            string newerVersion = determineNewestVersion();
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            var currentInfo = knownInfo();
            var newTriple = new versions.Triple(newerVersion);
            var currentTriple = new versions.Triple(currentInfo.newestVersion);
            if (newerVersion == currentInfo.newestVersion || newTriple < currentTriple)
                // fallback to known information
                return currentInfo;
            string[] newerChecksums = determineNewestChecksums(newerVersion);
            if (null == newerChecksums || newerChecksums.Length != 2
                || string.IsNullOrWhiteSpace(newerChecksums[0])
                || string.IsNullOrWhiteSpace(newerChecksums[1]))
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
            return new List<string>(1)
            {
                "thunderbird"
            };
        }


        /// <summary>
        /// Determines whether or not a separate process must be run before the update.
        /// </summary>
        /// <param name="detected">currently installed / detected software version</param>
        /// <returns>Returns true, if a separate process returned by
        /// preUpdateProcess() needs to run in preparation of the update.
        /// Returns false, if not. Calling preUpdateProcess() may throw an
        /// exception in the later case.</returns>
        public override bool needsPreUpdateProcess(DetectedSoftware detected)
        {
            return true;
        }


        /// <summary>
        /// Returns a process that must be run before the update.
        /// </summary>
        /// <param name="detected">currently installed / detected software version</param>
        /// <returns>Returns a Process ready to start that should be run before
        /// the update. May return null or may throw, if needsPreUpdateProcess()
        /// returned false.</returns>
        public override List<Process> preUpdateProcess(DetectedSoftware detected)
        {
            if (string.IsNullOrWhiteSpace(detected.installPath))
                return null;
            var processes = new List<Process>();
            // Uninstall previous version to avoid having two Thunderbird entries in control panel.
            var proc = new Process();
            proc.StartInfo.FileName = Path.Combine(detected.installPath, "uninstall", "helper.exe");
            proc.StartInfo.Arguments = "/SILENT";
            processes.Add(proc);
            return processes;
        }


        /// <summary>
        /// language code for the Thunderbird version
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

    } // class
} // namespace
