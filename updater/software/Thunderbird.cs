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
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
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
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


        /// <summary>
        /// certificate expiration date
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2024, 6, 20, 0, 0, 0, DateTimeKind.Utc);


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
                throw new ArgumentNullException(nameof(langCode), "The language code must not be null, empty or whitespace!");
            }
            languageCode = langCode.Trim();
            var d32 = knownChecksums32Bit();
            var d64 = knownChecksums64Bit();
            if (!d32.ContainsKey(languageCode) || !d64.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
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
            // https://ftp.mozilla.org/pub/thunderbird/releases/115.5.1/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "2e5721d062c59e57dd675ea4bc78a7c8ff7ca4a0d1e1fee50e4461fb39ae4bf3363b995aed78f8d1ce01d8c2ec1a01cfbc4232cc435b2708c872efbcefb19217" },
                { "ar", "c2edac62ea2050329ec0e66f7e8df34f1f3a46b89269ef0c899aa5a4a2362f114891af1b714bfa203cf88b3301231cd9327cbdb9d647960866492308ca23e307" },
                { "ast", "c6832c9c7a3673b30e556564e2bc8a78995fec38d994460f5d169bfb823aab644e399c1882a034a35adcd83a648cf4ddf8cfdcd89286e0c4925ce937fffddd10" },
                { "be", "59288a327f719fd9e823b69c8b405f57c23eb97d74ba3472f112d1993734155db704625f34842def156332dd562f5e4ebf687f96a8d03a470c72d25f7866e98f" },
                { "bg", "a886474d5c1d9ca954c82306307b2bad0eefe858155e8c2ec970202c29cd12686280b2712670b9850bc0548fdfb73341394f33af8c4d3fd15b00fd87579073ac" },
                { "br", "fa45d370129584c80748885ca1d7c1ccce4fcb46df93b6f5a6ceabddd70cc276b3d5050f60e512434872f3e041eaf01c2035b31dab80141e2788cb7f142ea950" },
                { "ca", "63d45a149862668b66427e343ce858e5e59f8fe08631a374a4c2b3a53f8ceb9487aa26f3549228cdc3e0065270eec6c7404f5486b86b176be1c9bfccf79e5e99" },
                { "cak", "77df7a7aa1b400295080d1f2887e47062e2c57b3ce61de687b48726ee5c16b534cbc54bd83ddde111aee72061a298f6f0941bed8297619e3942816096dde495c" },
                { "cs", "7dc08263d85e4733f91a8dbd809e8cec888eb352bf994f549968f3be2f094cfa3355e9a37d0bd2acc0f17d163017eba7ad3f7c506d747cee237516f1a963de79" },
                { "cy", "2650d33b626fd22fca356136c06ecf11b21ccb0adc5f65d33832154b851b01f58f72c7d6377c0ed5c9e4c0aab7d469fd82a00a9c778f66dc3ccca6163205ec02" },
                { "da", "8d217c6b91cc9d30b8b890ea06f6d8aa526242860bf5a2a843db698c55d6eb23ae0d2e7d81f53d98670c961b30a4a809399f58c6bb413a8dabe82e915ec6b536" },
                { "de", "6da355e69a97cf7cf47b941c064587515e9e5dc6cf8c5b65a83f610984dc2dd6ed417ce85674bb83d16db4f994e33eda719d7c1a00fcf8b9a04b4b892e7451c2" },
                { "dsb", "09ed1567c0b97cb818453bbfb1493900a23deb2f5dd1724df12613a212d348f04397b091c6b295a2c1f9b2f6abb5076f4a0503fb7de8ef472a52c07c6881eff2" },
                { "el", "5631a57447d6060b51e370c4e70d6afe0df337dd7977bf595756742295e688daa12dc888ad2dcc00f9de00f6d0c99ffbb50c357d3bb05653afde7a38b9dc479c" },
                { "en-CA", "27177fb59d2358ec85863313de15f9009640b2f849be66d1c235fa14b0356ed2a3610ebad1434c54635a5bb3aaf59c6275af81b36dacf1cf10d85c7817e181d2" },
                { "en-GB", "d4ae4052dc0bd354875dde8e359c43eb00378b772ee21e33d6c7a8dc31f5599a81790ede4de68c63ffa39b5b03467c5f6d2ec07dd3f331b0ba0a28a4f4309549" },
                { "en-US", "1a867c8cdc3bca92e480f84d02c9e16f735cba5bc2ab10bf1b1e1ce0e559bb82154365a08a4b2875c06bba17fb9b6d1133d41cf55313fe0922a7e918ec5bc9e9" },
                { "es-AR", "d97213360535e147d4f3b4b172cbfde8047aa246fad339a44813695c0e7fe27f4a2e42364229c2997a41a1ab95fa4619b714c95cb9aed3b6739caf53e1c1a368" },
                { "es-ES", "47d680c45e8deea86441f3a4f15bf25f7054938abf7d4e1498ab14f6a420187d640a0a7430aec0b51df931c0f94ff4db741cfee6167becc6c549b797c6686072" },
                { "es-MX", "da8dfa846a3c24bfe8500d4b4a501ae58cfa94074f57893a4ab9ec352d52a1bdf42e6d8d3c382c7458f1831335544b457bff51f26246ae9719342ab5c5fe6eff" },
                { "et", "50223d72678cf3f1690e0cce0a315529380ea29db7043ded699c5b2b6dd9c6c32ddeed82a47e031c09bd057a2286b51d9671bb1bf3971cc61cefa7abeb6c1fdb" },
                { "eu", "fda7570ab8f5ef570713539fbf0becac546e05390cb5b0fdf2a476b0331decc0f45193da0837c5821f121da96841a320fa3297becea540a134d9e931841ac0be" },
                { "fi", "d71f3568fc397c1ff834535b917c6055802a9be680bc73b71f176b08cbcbddaf4c8c39db5e7026bed0646ad47dad038942e73a15ac196b807cc2bc58aecf7e01" },
                { "fr", "5764f215f5273e32442e44199f6ba6b77c57a1a5477f8ed001942421a2d0573c58b2a41fcfb785bc331e8f503f5de37b5ed011cad97ab3bd3b90243217f27083" },
                { "fy-NL", "464fe3aab13dde3f57a75b09b25ad3a63b4765d20b0791714f031640a95faa5fbc7264d0fcadb85232a3c721fdc97e5321fa551276fb29627924094e5fa106f4" },
                { "ga-IE", "67a982441ce87202359b2fd4c64207bd6337e95b44f0033891940902c0460d67986324ae8bc8baba4e473a336962c35299a1b7c6d891acbd1aa35c480b743014" },
                { "gd", "799a904d342371b62eaeba57bc86f098ceedfdb0c9671eeeda7d52379c94fc19791e71deaf71cbfb98b9e06e14398f34b6b733f0f34d19fc929be279b26ff703" },
                { "gl", "01ea1978628b4f2bb38754d3ed56a733299a6ac1a209388221c86bc3c817f4f585969164f0743db7f1b46559fea479bc51c11be257013b0cfa61fd1d58572696" },
                { "he", "01ef5e1791b61111df09ac4d9e05168f11bd87df8b15e2fbefcd0ad46fd92f1921cf7d51c393d27d269b825c433652cb4c5ba6dd7baa6ee42a2f9144b8a93e7e" },
                { "hr", "64b284144ade2ac9dbb47903c6c525042f81f67327aa6aa73af0115ed00dbfad9d56575cf9b07fea8f319b2d4088b61e914cf7232501770c7470c8b94784f83e" },
                { "hsb", "40b46bd86916528c613ebb4a5582461d17a5ab4dd919a43ac206378b0fa703afe7cc9401dcd9bd1c3c11aeb2a640bf588bacc33298afb2c1c9ac3aaf0956b70c" },
                { "hu", "154b247170c4ad5e39f3e7a9a7bf23f2b51d1d14d80649198d92361b37dedd2307a47a3ec23905a3bea901e9ef37fdb9e55ea3960b4d88bacd8ccb3ddacd4ef2" },
                { "hy-AM", "ab13cf88d7e85331ba4bfc77c9a994c4a92bfd7b3759c730cc40d1de9a5bcaaf17799ce50976d1eab4649161eb44ee0c738cb06f9ae04b368ebbec8726c28c17" },
                { "id", "839341dc32b063dbdb6b17d8997ca3cddb9277f446f159169a3cad92544e844be299d0c59011eaac8938dd7f301e659d71f84042ed1bf2a4ea0caf804310c1d8" },
                { "is", "17bd70806bd3751db572572a34bdc7dc57534293ff02a9fc3cd74eb203edbc9166fa8da25629b5cb574c1ac2215288bbc3ca4795f3cf60ca438550e1f694eead" },
                { "it", "dc163e2e5514d92f7f09f72c3b677f7a1c399729320de89d84af9f4ad78c512800c8e34328cf544b1e642eb401d10682241cbb62940c70ff4a689550411ef3bc" },
                { "ja", "52657f0d79767691ee93532cca52563cdd8b5734ac4f43cd98b8975ab48f65fde24528b70b72ccfa7a6ae54d3f02c2e3281bfe4d396cac4136c71b6024935b57" },
                { "ka", "fc995923fb16f99fa6c5be2f904855b96f1c7196a8ce2c449183b5461a4ed14e6ca9fbbbe3dc12d330ad4d6e48be9ea7350834eb70cf6ac23d6a643ef8831544" },
                { "kab", "955d18f9d8cfb9e3e8667b21fe61143d146c0d7b142bf99808e538ab18776b1cd289bc7c341f54fd4222dcaf1d29d37951a0ac6cbb07012455433879e95fe665" },
                { "kk", "b97079321b247e56b841b7ca597dd30d070e3124d8ab31a8bdbd2023c7ebab245c8a3a4217957b279784fec12f4dcf9ea84c93ce7d735cefab43639747d1ed4a" },
                { "ko", "52c6090008324f090ef616a087a67173ddbaa241f0d1a727626a0225bd0e77304603a2de7ce763c8abf759ed712fcd2e8c010324d719ad6ec17875fba620e899" },
                { "lt", "0a4223723ea1cfee8aa0e3bb1483c75ef1c4f81d8f1c0d8453c8b737b49e8308af898ffe530a7a12a6c09d9b2908b83c8dce44bf34d085f81bcd793e53bb4b19" },
                { "lv", "077bea5b7c84c2bc9ec475c0b292c24180ea030fcbf0e85b948ac284d22242577499d2b286fe7c378ff18192dc6aae2f114893a60f3450f77a003423c329dfe8" },
                { "ms", "7ad1b0dcaa785a85e00797da3bb9e6a61b182ad87cf095a934b8345db90b0156486042855ba858696edd73e91ec49aaa00a89c76336647c14135b26571785f26" },
                { "nb-NO", "d4ed64c8a21ff5e711f40856490b55cecd251c947af9a7197645e7b20dccf86101e179ac9c098ffd6b512f803c4be147eb9a2f8f403bd71ccc8373feb0a1f067" },
                { "nl", "769a4ecdd8e2ff8840234989117c10dd2e5a72de291903c7e64cbe9b480ea814c97381b6668039d9de2a881074f18ce1f2dba0ea09898fc42b856c287c647e62" },
                { "nn-NO", "7c78f2fc7ab38ef89f865895783f6c149f5816efe6fff349dbf3e1146f165434637694abd168e3da55f9f7c3bb1927acd10c00c1a4987a4f87b1aa623b0b6b28" },
                { "pa-IN", "95d228f4dfd4be840c6b3b3d478c1d029b1841cf36835b814476df770db60969437fd2833c22e8d23b1b6722d321ef4d01a968d4b9e107b7841014fa72c66029" },
                { "pl", "227712719f2c27efe86e837d77531aca4a036cc570d91de173a97de4b1b54d37696696d255ae696d4ad15909966ab1464d22dd7106d3a1f46468635d21139547" },
                { "pt-BR", "440bdaf1c39d1c59dc4141a35d10f7ee46c09231fcc8b158a2f222ce0e7dfa8cda23fd6e523ed3d0609f929774efeb70b6dcc6d5d4bba240020ce1beb31fd514" },
                { "pt-PT", "bd3ba7fc9817f519e4debde5bc127fb345210607d9e716dcf1cc33db49ac9cd69aa20e0318d96161f2721ceaab4e4e8c5cb7a02f78e6aa52bd11b914e15909c1" },
                { "rm", "057e044608df4bb47ba1bab96ab5a82cbefdb8fbf0a5c60f66fa27ce39a27be0483d477e8b3ef14810ffe29edb6ca1cf1d88c1c379fae5d277eac06d661b8bc1" },
                { "ro", "8be4e99a6968917fd1ac1975122827fddbb9390c8f051b7dfeaeb7768bf99aef8259d971f7d2ed3a3a2b1944c126048284e07c328fb3ab6fde2b732dfdaeccaa" },
                { "ru", "01dac284c190119014e043c28f4119fb5e5a183e7fb41f37b67f443ec3e8136b5cd9be7983a45f570e6e92be88c2121266b8945acea700ae5c2c77e490ca1b98" },
                { "sk", "ffc82493e8d1d370724d66b3c41135e745c2a4c15545c6b67244ce3d00e645019e5fb91de38a6051e0858e5075b8e96c14ae33f351a2b0e6166318ad077d257b" },
                { "sl", "582c14cc0e319e97626edd1d2d8a79c550a0a7ed09a4df77ceb9f6f7174b2000253f31f352a63aec9b93d485d1932204f61c22ae3ffa34d3a3a21ff6f3a12f2a" },
                { "sq", "1fc0547380c9102150a04097401608f7ad38f6fdebbb0a61f85ee737ac290f0f5114a888e6913567de9a92300190d02f47c786a4fb270bc7a1ba814b818495c1" },
                { "sr", "2da0b6cd76ced61e857a3e170f6debe0bb3c8eeed682d4c42579ca0e29c5195c300f0ca868ec0441b7032adc4d7294104ebe5704ce9bfa0fe2a9e6c75f3d8a44" },
                { "sv-SE", "97c5ef7de2687daa96127bdd260f3a3209082b899db6a0cdee2b0c15c71b278929e6a688a2bf10db6f5f40d019e19a643364656342fc512621c56f6b4c0a161b" },
                { "th", "3185cfcf254fdb8d98a1105f7ce31303420ed04a0338a90a8cd3d09800d0997aba75d54ded17eb1c0e1bfd985afbf6bcaa7a719a89aee27fb6112d404a9b3ca4" },
                { "tr", "a8f62a50a829bce832a43cb64369ed4ffc6a2c511af6b938099779696ab727828e512bcf703b97d21e157c9890c70d0d6f926b69c23494d7d94d421a5f0eb30e" },
                { "uk", "f1d2899dd9e5be2b9e9aaf45e05e0ddee86b7ecd9504a2430064e62a8ececd09c7e3a8687e94819f826cee0f46d9911be52d165e5447a92eb8f7c25ebbf93dd4" },
                { "uz", "7db081aa5c4793c9c22df4e73f9b42b5ee8ea93f8a21f0eaeafc31b2f1a043880ed6ef3bc62800fcea78729e26434f2c5c4ca270a57a91ec5051a05655780def" },
                { "vi", "09124a8d00717ea06d5b54ad7178194620b5a87263c5cdf4ea628ec2c33cee77259d1f2b63dd37390e5d6a6829c81d9f18486df3cbc603efd2765bad54c0748f" },
                { "zh-CN", "0d74319c3007b5aabe597c1d6d86d3af40f773ac6053dce3df663490f2870c38648cd5649390b68ff1aaaa29f1213321d4881864112817f56a8bdd1246c4f64c" },
                { "zh-TW", "766de6a91dfade4c579a56571199ff09fa06952e73e5c217f0d02160f74de431c5a2ca08323654c94438af37529bab336152b60b5f1ce8d91d697ed449d04112" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the 64 bit installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/thunderbird/releases/115.5.1/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "24072d214c601ba65ad77dc9e886f2ebd620ee6865ad5fece06f0e9fb7ecacc3eefc50cd7c1b43383e4c212ef66d65f210dc73b7e279034385773bb23974828a" },
                { "ar", "3c14b8f5bca5c8a9b9f01ee24cb49517263b2dd4b258764180e8d5ef612d7f3b177eb4584d9951fdb5ac14fc416e87f2a6183bd254a0624ca47d98c58f5a31eb" },
                { "ast", "2416ac48946421913c9f1c9e1610ab1dda68f05e6104581ba18c3c5b1d6cb117fd0a3ae2dfeeced2b4a2eb77878b6efe6b6b94179f041c5efcd1e634a68eb016" },
                { "be", "49414dc5dd3232f833f2ca43c7783169e391d9c09365c0bcfd84748cf0c95e3dffff80b47a0372f177c21b31c6d8a2aab52bd6c35e235ba4e7f8e3f0d6b1f54e" },
                { "bg", "3ef004cac68bb82d68748874ee95f4e71602e99c3d2feddf8aa480b04710b77bffbfd26d7fbae675f16e147cd42f0e65d6d6951f1654e2108579a67988fcb148" },
                { "br", "bfab38fdae00fc5c7b443cae42b3a734546541fa319408712e9afe01a16af1d6a413b252669bea5df84201d5b91f422b903755b41a627c078705e7da54a5429c" },
                { "ca", "401db0786958607892fd64354b2cfdfed870301ab33082fe5a7e9b76bae637d90d9c2332ced049b21a04454b81809d66ba5b85f0955d86cbe5e9f6825e388fba" },
                { "cak", "17ef6a23012e26a840df17457a3fd88460bfc0164df57b7f37cfde137d4d13c3122ff47406fcaf90e69bd098be7eef83eebcf5b5b15e3e7ece4e1baad30550b2" },
                { "cs", "a37da02d23a883a2faaa6e84cb168a9cbbcd99bb8cff40295d7ac4aa1cee68b102409f9539e6f5366bc37d51a1fc563b9f8323970d7e5c44b0eca13ae487f9ee" },
                { "cy", "f46f3e3ae820c37a4189768cc0aaef4768fe059be32f46f955c72d468837658687c7ec6e8575d192f8cf7c0522697fdc7ef93df8e442826259afec192955ef5d" },
                { "da", "07f312ff7fe56957dd254713a656ed765743f8df4af2353a8b26da4512e9696271e95a9f16a390a619ba49e7de5630e846e7c91abcef3f5b5a580a89dcdef4b2" },
                { "de", "ecde9eb1e5c5f9019a1a4f00ad6a15c075ac44b33d3023356fb54e19cf1ee915d10b9cd034c5cfc806c4908b5acc77f98f5322da3f1e433eb3c2e18c40081b26" },
                { "dsb", "ae311aa74ef714bae2fc480760a6c452a635111644771a9105293afe235e6566c626a09f39f3b47df5881c46b03ebc6bebf3d3101358bbdecc8fafc183b71b5b" },
                { "el", "c8bc0e654f9f8be22d5e2184517787b5e3289c609c77c98008f1ebd07e6c742ea99363f522f688a9114e7df143923eb4acbccce480884fd9cecb5e10839a4ca7" },
                { "en-CA", "57a7ab7f713a991c52a64408651a9e9f23f75add126e290279169267c28073964bce3f2c4ef518fa91d3de3f97b62b9938ad37f2f8b4ed6ee4a5aedd27955edf" },
                { "en-GB", "090706790bc26bc25fd6c4fb6b55275709db446b653f0e62697c3fe744de300ba1b1a8104668784a09aeecd71aeef32c71bb609b0f39922f85994d0d274119c2" },
                { "en-US", "85351f5b92b1ae9793a991facadc443c34ed60f61d8eb9a91d915fdf63de774a058232bb94dc23d41ce0d814a7f02bbf18924dcadc86f7b23a7a788c8c7cf5b5" },
                { "es-AR", "b5d7cfee83292100d374cc0c0bc49c6de8e03d1dbe3d6f8e81561b3fc5bfd7a705235ae1f0fac636b84ea95eefd6f59cfed63a85b3a2f1107923bbc3b90f1b25" },
                { "es-ES", "092024c59f1d7c7e0c2c099ec38c36895716db279a74632d6f49846d92e654c2bd21153ca70fda0059782972718e9bc7d8b3df14991eddf0abc792f8db20e4b9" },
                { "es-MX", "88bd84a4f5886fc65b1785a36005a7b278f7448afb41a002143793ba40952136eb4939d3e744ab9abb60c4bb0a417e484b2811a863d9690f89773e37de870884" },
                { "et", "b7f5b62fe2a7895ead54757c541fb6ab36b18f9692c09add465bc0ffd41edecec1c9c434493b6b9195c38f3a64a42cfb6290404edb75b37b4bd920db0d50fc36" },
                { "eu", "ff132f435cd39174519364d051cefbd8c24479e5fa351eec92ffd92dc0227c96c713b399d338328b7a2240ff124628b0537e7c9212645373d3860e9ed2354b76" },
                { "fi", "25cd8bafc6c6d08fb5ac4f73245ad38d83d1cdc852319eaa7f1835ae898420c33b29afc30aaba6fb3499333556d35ed2e7fbefac69b8f9d61db38ec7c66b13b5" },
                { "fr", "36136fc738108555289dd7b1b3d7245a800774839db3407075c1333b84db9705d6be43ae7f1488079a749d357357df73a902b3025cfaeab2004c44f459ca8004" },
                { "fy-NL", "7f046f96a95d2f063974a259ddec54bb61dcdf1f4d11531abc70f6b54f9b05d2996f739cad94460dcb8de5d77730a7a82f9a9342d4337d9872954a669d551e7b" },
                { "ga-IE", "6697c18a3f00132585d414b394855a91abae7305cc8cb772931a9a047f9a771226c1015373f29b658c0a76024203503fac7e03371daab1ca1b0bc5fc6d1d4a4d" },
                { "gd", "2bbb43ae8ef11aad8668d3673b004c82c04317c814bf47dc315a0aeee9740b3c7e990d2433545a8212b134641706a4c2b10aa194d700d9f2bccd4e8d42f563ee" },
                { "gl", "2821b38878b5b15eb3ea426f35ae950829ccb06b094040af356592ab028f32e3ffd877abd52e578c6c07f2341e5f5e3465b0e49f4da56c974288690f1068eaf7" },
                { "he", "405617ea59fd43a2b8d35104b65667a7508f035e9d5fdbbeb3a45b0374c965ebbb34cc0aeb6670160cb511cbbd81ccba1369d33467be469848849e404e82ea24" },
                { "hr", "cc51eb7b8f86bbcd5b1a4371bd385744e9385e33846bd6baa9c7ee2c90c916fedfcc914c9aaba8a2af4f47c2b291f1f2a654024be6ab106d20dc34dfd9c56f3b" },
                { "hsb", "477e6a7c3404c6704c61e1260291a944b4c005b544427bda69dc9f945f75bb3ae8c16f10efd402bc6532f497346f5817682b2dc7fa475428db757c8a43342ada" },
                { "hu", "5f520c538066b2c09ea9bd49f195acb8943389c6f087be7a085f0433cde114ef574ed60433281c33a9da8bf83cdaf3477d4f9956983b11223ef65e6c3a3c12a3" },
                { "hy-AM", "9e9bad4c128b7cd68c377bb74ff7aba9e02ba7a94ccbb5493c7b1be64041cffbde9ae86f1fa6ed556be3da5df1a4635f3fc26c599360e5908cd1ba5a398866d6" },
                { "id", "be90b97bc9d92967f828fd2459fc35b45e5e5ac627c0be3ae6aa3cacc0d7fefc385d2456dc26d555819902dc4f9707b43219a8a71bc459dc4216a4ec13fe0b15" },
                { "is", "95b7986c4b7129df0ea96fe1637cb3e7216632a600feb2b490ff60b8f19e6de64b3c1ae58fc5d400b64368a480006563085635ad4457526db7dc3e9a95d1bed0" },
                { "it", "b66785074574879fb4413d23cdce5cc554e624f3e79fe4316133259f7e02a579e50b2cac991796a552617a467733bb754c9d43da3b4fc29ac3c52dbe5664db78" },
                { "ja", "b4721e3cd1c4b96d4e6fc9365fce26305d876af44863baaa5be934c15c138a5b524f88192f0c5decd9741398d2b506e23de374331c2864e6e7ab070df9c21fdd" },
                { "ka", "1f231a08533e299883657c767aed095408622975b712cf13a1e7f519522909b84397b4ea5535083d856b7632477e6948ff3aba454e6150391b0536eb955164a5" },
                { "kab", "d322c29a9121c8f4b5f93895abd2cef39365ab65227cca2fa96a1ececb9d42f88be0f1081708e2548fcb19dc9152aa15402839c87a56eb740afa9f0709b162ac" },
                { "kk", "dcef3673647eb3e542f620ff60255d7574126d0e8f1076c83849f3673e401576ac4b5cd6f32ee292c8abe984328db8eb86770fee422c0d9a58deb96327cd3bcf" },
                { "ko", "a7e1461c3cf8c93c65773e02bfee06961f3528a5900af658b337af6451ff544c88a6f3eb69800fbf07df7e493196c0e7bd94177a7ac3b7fa114ce62c3ec8bddf" },
                { "lt", "0f2503e4db5fc53e9cb618ac7ab92bcb83063dfd13ef80491b50eff748e57bb45f210cfa8021b08f841db2d74d5b09052d20d252cc020053f8b1f7de3112dadf" },
                { "lv", "1aad6fa01fe5edd86e10841d6c36388ecbdcce3927a5e83d2ffccc4e0dbf2eac2dad92d89af071b03c8e66b618daa8ac63441b36c962761cc9330f2534cdb093" },
                { "ms", "4a1469b36f41e5c857f0a46b5908a485a0eb26b0d3b1f6979b79e6eb745b585d6526f7bbdb6ebca7be579436ed78b2fe92627f774f2b985ab942f1e7bc07fad0" },
                { "nb-NO", "2ffd537143abefa72502964785e41d7d1db3cdcb07608cc8218689180a7c485489ad797b0a8fa721dd3589b6e386ee981ae894a457338285caeba56512c49d2d" },
                { "nl", "d8d0ad5ba2f7ef2f598571a48c0948f626e35e489bf123231d024ebe08e7d435ec38ccf86fccbe7f2dae045a9df363d80e721245e4255748070a171232e60363" },
                { "nn-NO", "b61499963ff869dbcc16610dd497bdc5b9a85374628b052686d2c478b48f9566f23cc4f66e4e7fc48252784e753469fc4f36d37e044b6148e6b2d01ef0513579" },
                { "pa-IN", "c11c3f08ee47a284f2f15ce35ff3bddbb8df8a30020c46d254bc3dcd1e48fee68c0154db0b5335892a02c447c2d9aa2d445e6734f7082bdae72a54356f641d01" },
                { "pl", "3e94a6c9c6ace8aaf9948972590844d3c0a3069a774c6a1dffdc943bf417b3200e8a99160f85e5b159c43f8bb6e67d461b7ca187484c8394f6410e105994429d" },
                { "pt-BR", "d6fb27d31d01a68233b61c8173f592ae544032612d44fc81d0896308468eb1b0a206170df250565d1315e28fb69a9f06d270f7bf508d0c7ca407ffabe7756eec" },
                { "pt-PT", "c758a42a98256aeb8b8efc9563a1d032c8b90ac62811c7792b7b8538875b4eacc6d4b5214050d3f8eca1eb81af637edeaf7cb095309fcd4272a768d9640a7979" },
                { "rm", "e05267ab75042e9219a145493d27fa67281691dd4aa01ab6b479c4d50955d47a31042001828c598ef0e6f08ba949ea768af9110a3281f8a233de61655074b1bf" },
                { "ro", "a430fc415c56ed8a7227f2de154848b98c55ddabde654e21ab324a252aeaf3457b14907cf94a5b5764ad6aabec0ef05f0fe3aae37d85cb19f4115a15f7913fe8" },
                { "ru", "1d9dd7858ad4bcbf8fc16fa871687e344082b468a870058b137e883ca51f312dd564a864a4321335d69ed2683f19ed7668f3d04ff0edccfeece1ee7c1824b006" },
                { "sk", "4e7466fe18164b4f57e00343723e5ed70bf3ef475e6687c6b0ec4d34c2bd0fc6a056091eec4d421e01cb72e4d40bf3cc01b19d541fda610580d8bcdcfad5003c" },
                { "sl", "0b52d19e5c2247351d3bf1367fb546fd618e093e0f6d99a93e478c666b90dcde1640ae947f8ea98a3d5e7c1107c45624c52058351a334d4ec14af5d9939f9965" },
                { "sq", "7a7766f0bf1a4abac0f91bd43118f044278e0ed7b23d2e9e48226d5f360643f6496fef16975adbc28080ac9f8f0221dbe05ac18801fc01a58582197d911de7cf" },
                { "sr", "21c36a5c2016f690d27ac003f76875675049261bf1242a1116ede9bf1f17e9667c5de73dc23a3f6a728ac6b1f4647c5c074e1b82c99187213184c7b3385de84a" },
                { "sv-SE", "8c7aa5f65045eba48eff7687d8c58b6b04dbf67944e04b5f6a7d20957cbc6f2b424cfeb1a9f9a5658faeb9f8ad314eca932afa61dcfaa7f3749743cc62ecc520" },
                { "th", "8ceb9f0b87ce82f2cf0344e01bcb9507f9d60e18ecc27aefde13428be5495ff1b3a4717a37df7c5dcf70e76a101033ed9282a851da4038bd0a457b91eb3f8e37" },
                { "tr", "f965344dc3fc404868107cbfab5cc84ac01855821c5e49578be0b035e11a3f2cc02a061c7611f5f3855bf7d60deac3b1d945336b0516c096fe92ac19f3c6a334" },
                { "uk", "aa2450c278a13d1d9cd7ee7629f28e84f0da2baac4481dc202a4dec01913e54eaaa1dd83635835206b1e20c9ee89c6abddf9ee540a8377e36f2e1726bfe39104" },
                { "uz", "ae16a4734be14997997ae90e8bccb7ffc0c1e53d091407725bd44255a133964b095ec6e960de541ae0c14183ef7fb2cbc0ba8fe0197e5cf72190d79d1571c822" },
                { "vi", "5d86346cb18104d9b1288c80c5f36ef3734db1584bd4e3609433119a067a1ead8ae7ea88c878f94c08fd15789e411dc5aaed212fbfaa1462c7c57f9dfda905ad" },
                { "zh-CN", "e841bdda77661f792e2af78cf75f5e6d71337d5937e460b93f4a2ed7caafd31770b03bee8d00fa1faa00cd4ee41a7c7e31a67aa9803c1274d96a20cd9c91c375" },
                { "zh-TW", "030ae5f9c5a3ba7a2bde0de8946631bc04d4adb6bd9d0915337130fbaddd3d4ae7cafba4b3b03dd0cfecd7971efd4a12d8de17b243ba5d1e8c804a7c978231c7" }
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
            const string version = "115.5.1";
            return new AvailableSoftware("Mozilla Thunderbird (" + languageCode + ")",
                version,
                "^Mozilla Thunderbird ([0-9]+\\.[0-9]+(\\.[0-9]+)? )?\\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Mozilla Thunderbird ([0-9]+\\.[0-9]+(\\.[0-9]+)? )?\\(x64 " + Regex.Escape(languageCode) + "\\)$",
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
            var handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false
            };
            var client = new HttpClient(handler)
            {
                Timeout = TimeSpan.FromSeconds(30)
            };
            try
            {
                var task = client.SendAsync(new HttpRequestMessage(HttpMethod.Head, url));
                task.Wait();
                var response = task.Result;
                if (response.StatusCode != HttpStatusCode.Found)
                    return null;
                string newLocation = response.Headers.Location?.ToString();
                response = null;
                task = null;
                var reVersion = new Regex("[0-9]+\\.[0-9]+(\\.[0-9]+)?");
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
        /// <returns>Returns a string containing the checksum, if successful.
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
            string sha512SumsContent;
            var client = HttpClientProvider.Provide();
            try
            {
                var task = client.GetStringAsync(url);
                task.Wait();
                sha512SumsContent = task.Result;
            }
            catch (Exception ex)
            {
                logger.Warn("Exception occurred while checking for newer version of Thunderbird: " + ex.Message);
                return null;
            }
            // look for line with the correct language code and version
            var reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/" + languageCode.Replace("-", "\\-")
                + "/Thunderbird Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum32Bit = reChecksum32Bit.Match(sha512SumsContent);
            if (!matchChecksum32Bit.Success)
                return null;
            // look for line with the correct language code and version for 64 bit
            var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/" + languageCode.Replace("-", "\\-")
                + "/Thunderbird Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum64Bit = reChecksum64Bit.Match(sha512SumsContent);
            if (!matchChecksum64Bit.Success)
                return null;
            // Checksums are the first 128 characters of each match.
            return new string[2] {
                matchChecksum32Bit.Value[..128],
                matchChecksum64Bit.Value[..128]
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
            logger.Info("Searching for newer version of Thunderbird (" + languageCode + ")...");
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
