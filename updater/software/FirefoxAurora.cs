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
        private const string currentVersion = "116.0b5";

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
            // https://ftp.mozilla.org/pub/devedition/releases/116.0b5/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "11bf0d69cc3c8ad0e3c58d5c63ddb56dea03388487cea43cafc12156e55dbba610ff3041adb3ce82692658ff8aab146779e716beb3944c26732cd4eb7a722d1a" },
                { "af", "93b233beb1100f815abaebbddbdf97826b02f2da48b1eb31df27afe7e737849630c777a521b3586472db90044224a160dd223047811ab0ef412805de1f736ce9" },
                { "an", "1530c3527d2d90c57f9dfae39447e2a8da1feb48f4e48ca86f43062b3551a7d4dbae2019d081c0cda65b3c8ba17d0c0b9cbebb81d42fc48c0dbd412ff4924e82" },
                { "ar", "237da08f773d8beb4200b2beb6bcfc9a1deb80a21c8975343a2c6d0e9ccae9672dd9293ac80de9db9cf2a3efe47bba3ed3261de41f525c33037f92a13658afe1" },
                { "ast", "1114feb220b05c023a41b5e891f6768c25a5be43f9c1377b620a8f1ffc417cf5280f1f23e445e2b138c304a6b8b1ff431fe398e71f309d2570cb2babae36d6bf" },
                { "az", "2820f5afb95b363fd080db75643b1508bf323c9ede330f7ee206c97aa1edd2baffffbd47757e0fdd62efdf386ea7caaa798bda8ceb23688f509f01a45a2ee6f0" },
                { "be", "f2dec11327d631fdac2325ecfc29b31de183ee1c28fac106310a59b97575c5db7e4b5fb3b59b90ddcf13c453b648b1da023844a7ae667b6ee4918b8ab6c6f411" },
                { "bg", "9780171517ca36497b2fcca3104c8bef9613376d2c174359a444ce68e319d2e62357c270cf928d83bd311567fab241df187acffe48d2bb46f2a80a86141aaafe" },
                { "bn", "de69f2bdd2793e78d7b205b1b14e4fba4414c422a7c38c854e934eccb08f3532e38079c7a64a0c9fb350459464caa14bae43600c457ff3c1d796871005565b8a" },
                { "br", "7a648da19c9d5dccb471aba35a7537004ede8a3680b1dc75cdb7acd972e15df30f55472e9b57eeece812eff5e05083276489417d9612f511b9e17a41052da692" },
                { "bs", "9cf9317cc2523c958a53177e874d8eeb1891186159ecacfa929b617b29151a0ba3a9cbaaf4766204595f9b1ee1b9c64b2df810c7162b2990d139d8effe64f2f5" },
                { "ca", "652b8fd1b5986e8bf6b45e51584d49acf25abf75ae752caec9afb79e04ccbd33ac00506654bfaf123592e62d661ebe07493ddf577d0f59dd76ba39b87efe388a" },
                { "cak", "9afe07a9558dcae88d73a37e13e4f217cc06c6269ccc2a9671a4474f2e9fbd116d2a89f7ea5bb2f97ec1fccb0d7d283f14727a8bc154f8ebb58c07bc9bb71d0d" },
                { "cs", "6d355016765492df5bf510ff4f0841b252ea904999bd70c04063e6d0ebdaabfa131bf0eba9759aa3bc25b73a44ba90695363a9d49bc5e106d2797918bb8339f8" },
                { "cy", "7c0420b76523b19dfcbff7714fdfa4900c3f2e0397cde3b9045438fe3ea4b45c1e8f265b1d087dfa702f94b5aa70617a2d6fe2ca2e9825c1b64fcbe594fddccc" },
                { "da", "c736b448241f12c368c4d4342d705a5879381fb18f66c9555434ad05e6287f8f43e1de16044fbcaafe130948e7fe293096f2afac165c55fe4670cce861925698" },
                { "de", "f5c8230deca51532ee7c60fd73a89e3c3f2bee0e6ae52cb54040dfa1d76c6cd2c27f93d828a248c504d77b07cb9442a5c4969779ceff64c295215008aab40825" },
                { "dsb", "85543abec47c7b3f7ec04e614a344d56dfc726cb3448492478ae09e2eda9bf916f7db495824aeade90d0f3ab7919551fdfbf83c4d37e4d0dc37e072a595148d0" },
                { "el", "b1377bde8c777b65756bcb3e4aadf459edf82332e4b7f474c6a9d68a836b16ac27787a0fc9fc3e8d1e94e1a8d84b352c1deea2dac3fa6f31c78c1a37e6740da2" },
                { "en-CA", "c69d337037ed74cfc5306fa8d1ce1f21c36e763dd3a1158beef761aab1d6df6c31c073e34d0fbf446278ee02679438daf5aca44e6d4632ca6577cbfba279c51d" },
                { "en-GB", "a0a0afe27101bcdcaa4b67c2b97da2588504ec565b9c7afda4ccf9699907f0ca017c816924ed181909aeb5aabeec5923fd81ca84d28e0fa6cbfec4446cd9449d" },
                { "en-US", "231b45ec96cce203152b33b9efa9838c326c17a1f1dc9ee9df20a618a64e9ba6d93df10a362b6ead72c6841675460b58583b4de4e2984bfa49d509d861844e46" },
                { "eo", "62f62a3a7736228e65d4da426d20366549813c93911e5fa251c4a9f325c894025f34b08d3039e7b05988acbb2e44198bb813f928d3f19963e6a005191f5370a8" },
                { "es-AR", "39099b4f501daf57a53d583a9f1e76f6c8c2fd82deecee8ec98c45332bd94d8c601b31c047cc9232c67b5f86d55d266b33c437761644834a97814892bfa5263a" },
                { "es-CL", "881ba3b3d07a9098ad0eef36e47e7832ab0109cfbe19f05e7c2f09ddee2085f105c1078ddcef4e9535cdfc9e02d2c9846bd104e493513bd5e3c46baa8c6e7bd0" },
                { "es-ES", "60e544574d55f4751a94e176b397cf22177c79ef82464f1cae4ba9e9fa76277e08ac896f2f19a4e3d25d154ee39f2c72df94be9bfffd7e63e55e0c118887a534" },
                { "es-MX", "8fa50d4645047da8d2383c7ce8edfc9b9f9926aabe52396ab53f85d7589e06fab630e32a4030ed3178da26d30cf77d5b6dc4a02e787184da8dd26957919669e4" },
                { "et", "7645736ebf9fcf1e93c7d8c4ef4a0c8e15b92654c6797849ae8f8d2921c307840be66f0a651c357ec282bd6df1b86ae2c9b2d3fe5a33e04dcf51c1298d2eb433" },
                { "eu", "81ca885b1c1901ffb0dfc811606429589280b8a7ccfa28b56f7c92023f85254d92d74b09542855279dd6c6cf6308f1a8d4e2a7a392790f5980eb19f06ae346b9" },
                { "fa", "d4dee8ada1dd0c30c2fe750a8a7b218e9dce046df6443424a3053e09c27d8d9752027e06c6d76bb343fd33c5019e261f9a7b8bddfc14082ae079f86df937e87f" },
                { "ff", "3760094175d4825544c74657cc4b371cc4c2b3fa1fbb7ef1f59c901476339fbfad7d22ecc00f8ae2f91ddd8647764d6a6538d30a5ff765f9e4feca0d063ad932" },
                { "fi", "d14353a56c89347c9da86a2724b7646c24b368a0d3f0adbd8a935d1b6b86c5eab83d916227dacf276ac5bb629fda907c8ad112dd6c1fa8ef5a2eb46d7009f2ee" },
                { "fr", "2603d4f03bcc89eea43b46cd1a0db91cdbf12dd07b4b643e98a0d9116cdd923330472dec863a07527501d80118c4c861717f0b9e436957a40996d401af8336b3" },
                { "fur", "cfd4401a4fc68aaf2fadbe9b1e4ab7bf6bde6c367be3a93191b48cc34a14199de4ae8aa04570cfbc068ce2eb2563311b3498b149100496ee0115010b7dd779f1" },
                { "fy-NL", "e944cae46f26cb2a95cdb2ab90d4be7fcea913437d9c3b1332e9c60f5da219749eba7850833b5a620e02a790ca222f4b745208b593cd014d1a966faa1bd8ebc7" },
                { "ga-IE", "85d0ee9a72633891b7a9126ca8e7522e91de3c5f6d40b78df190317cddcafb180e88d7c73f4af79be4ab2f1720b952f552b9680092bdf127dcb6cc248f2331bc" },
                { "gd", "e32b88cc96dd85ea2820318b31b59c427e29e2324dcf320289b637982eef3e02a92bad5c9c1999b586bf7e8fe13fa3d6b4451fd0f35c09f920a3e1cf0a538ab6" },
                { "gl", "351693bba9523e3f1135bdf4e44d5279e512c83595f02c38d186c8688a713b823ed79919fa9fcf3273771eaf68e69337ab87d769dfca10804e364968d69f63e2" },
                { "gn", "aaa46842bb4f5d6bd038d514471f0acbdefe01d2081bec56730c30488d4375dd18fa60fc454b89a2ce89e8c703bbe23ab9e142e952ef3bea2b5110d6af3a9122" },
                { "gu-IN", "0354db309b4f6a73b4cf490ea6cc6b21fd9124f5788d49236db815901902d171484f5f88100984258c4d687f04536ab21b51e7cfcae78601e0dcf4e83cf5e968" },
                { "he", "67e85f0005e50a813b57af244cc3c3f6ee4b1a474ef9d4e852efa9f44697da73e084a050d7b6a65538ba873844df464ddce594d7c281abe05466af924a3712fa" },
                { "hi-IN", "a2eb7180e58a7ae5fb38d30616099032b04d36b6d3c2e59bd28e084365d22415f6ae2ab9ebe4362d592748f600f20ac980688328470e885c32c25f987dd92304" },
                { "hr", "54afdeb38a19c1fbe719a44d18b218b9fa9930f03da3f387a62de419357b8168a537eb8cf0406303e3f0de3f72588777049d8662ca354ae481ac0d86d9680bb2" },
                { "hsb", "d648f49aa1dfb7ada783e96793374d65498f641af4957398ee2d98cc78748860727d2c145e07858b090176d89ce994c22e93d613e84c952e590f4be27df77378" },
                { "hu", "93c1d5dd0b583da1ed2f02a14e2fcad795796293ae6689655536601e997e80f4b9b8e8b8485ebe6fdd3160d4fe98769895c0b3241b4e98bf52ce1c1c24236076" },
                { "hy-AM", "face86bd3580794d019a9544dea73a07bae71471800deff13206128165fa0f8ebb8ae64462386663445d52753736d53512f2d31200e3df4a6b95bdf79e3b124d" },
                { "ia", "8e89a93e78a1d78103c88acadd2b1d5f8d0c4a119660ff68e6dfcc54334a6ee54341d6f5212986846fdd6129c41deeb5056c3bf5391d8b1e8ad31b2c3afcaa7a" },
                { "id", "2d1fc4a59b06e908392a3a4eabcc6d32706840e7f64e22acab86801f77e71abc714c88567c5dc80657a3ff44d52ab18a76842d002c3f2d856238ef2d29915887" },
                { "is", "a82df2ac63dfa32d179387eb34bb39c548b8ae89ba4589a389bbe76bfeb3ca73c96b7ba654601a566e3b7a607af6fd9df54411219611961a04fc2191621681a5" },
                { "it", "f7dd3aeb5c973d4c364ad2d4a10b7f9765d98c7ee26b68c17f5da4c058cf4a17a24efa5d7eadaab8809b0b5660401dcc0f790d044470f2aea128fe3217c53662" },
                { "ja", "0864c038774594c100d752f3bf96117b6d6e96d004684bda532f2cc544e46ac93a40d2ecbc4c76212573dd70e66dbccfe76dbc565928942b46e8774aaf1ab586" },
                { "ka", "0c775c58a644a20138ce049d7ce61c6ee4ccd6a77f0af24e14c2e0576d3054c37b01621d6d16dc29f5b471123aea12342240c4711a0de87965a03542c72cea61" },
                { "kab", "dc2577e0c527d4f7507860b1566268ae76ac7763c2e9003dfb22bdf35c7f24931482b808c44370712a4ad08b4527b2fc844acdca88adcebe848299e21747d72d" },
                { "kk", "92467696683089b2487f362a4d4b030744ee5207934ec9bb07f981c1016a57eadcfcf48e7b3c2f7db6eae29a7884d4758ed5ffe3ac1d4a26a5bb06e798b7eccc" },
                { "km", "cef2d34843bb668769f68ecfa80c2693445cfd6315c61a38041b8a0b44f78ed7c86e3e577698b3a7c27eff7da213aa06098d106e4b30570f7a5654027763b552" },
                { "kn", "67b099aa2ca267944a1d082b599556cb5f92eedecee97ddcdc1850000a38dacea019dbb93ecef271089e1c113139bdc34fff611fd9ad02fbb7ca6b2e29579fb2" },
                { "ko", "fd91c974491cf3b122958fc0a3950edddf976ff8f8aef314ebdb05863aaab69d466f8b15c5dfb61cbbc8ea83088c1057fab18c79d0f23b738403a4e039814be9" },
                { "lij", "ab86fcb507eee12cdee9613e36125b0e81bef2f3761f78478331ac41a43d406573e5e565f26b9e37f3cd124165e777135ff12cdcf8aa91c28f7c6ab14fb8ef59" },
                { "lt", "774568a52f5865b49911fb10429538a610e853078b64d781d022c7981f5428017ebc0e282075f594c18a1b707e27b2e74b6f53101d8b21d6ec2c26dfb75edf24" },
                { "lv", "02a2f41234c9b3a0f8adc7c6e9f8446f04f78a83e6bea30e45ac963cda639e74c2769d505e7178e844ef01afbdf2994470dfcab7d1439487d2172b4f9d17a951" },
                { "mk", "64dbe2f562129e06aabfcdebe29ba9cda8ffe36a197ec1c05c11355b45cf747931b24fccc75d7eea81dbb04f213ec15e50072b656388eda966f4a36fee37b09c" },
                { "mr", "cf6e12d58ca2bb890b2d6dd5b8f41a9aeb0d15b2fbee4a6a92da2e6099f120cc069fcef41e28f3e3fab10cdf6d28c24f922ac4551d1bf137fb0d025ef4b0b5f6" },
                { "ms", "c980c2caccb16e24aea8512df07ed84c75b39b2d8df72c6956b220d2b873661011a383b3cfd142061c46875f6db222c5ac0bc418358b1e6322d51d966e20ce8a" },
                { "my", "3d7f8adf6fb056b2d62298b832b388b4ff6d4cdad50588529a86dc99bb68842aee0b1d46b56028ba7ef37e8f24192160180c890506b7bb59480cf63ff3064603" },
                { "nb-NO", "5a8411fa05188e6b7e8f096d5a639aedfb5528418fb359260c94487aabfae910982fb251c3db381149e3f06a90daed789de1104e08c0313902c920773ef42b58" },
                { "ne-NP", "d33adfb16b14f2c92cbfe3e2e2968b69bdf45131fd657a2419965a7f3f2ed08be8b9e1eaabbeeb9250b15451834f144e4f503a177a552bf0346da41788ad7d71" },
                { "nl", "d5991bc1dd0dca4990e212da87272122f4c3939b08a746c817db1b5bfe2351fd423d628280e1606ff03f79c80d1dd4827888f81b6dc6252a233a02619654da86" },
                { "nn-NO", "da455ae823842dba538db3f93c91c76ac1df44c8241805b90572f7976e19014bf93c26602d9a816541ccaadceb6e7a0071fbf6b64f0a325a79ec133f5d898d11" },
                { "oc", "db87b1ae8038ed76354c5d4881278bd745662e0db8075f075f0ceb0a6d271f3fc0c9ed6491fcea006f01dc562ff8e485041320aa71e8654b213af697b99c4c19" },
                { "pa-IN", "954d6909917b2c390cec4cf082e28bc684211943598700705b57d2972880ab5b3cc559371109910df9abfefc0bd266348684f22549fb75a34488ce63743f23de" },
                { "pl", "a5ecb39f918bedd41a1be00e5b0a8639c456d66b846eeb330109d330fa8505e4e290453996cad86dff1816c5bfd2344c2ffef9f7716dd34e8edb6287e0e3eac8" },
                { "pt-BR", "0f672a1c732d1d1003b9d364640b6badc193b14b80261ee511db6d874f450cfaa0495522e29948a80b484ec85ff5f04d91370f2a53f22c24b347d56c67dc09b1" },
                { "pt-PT", "e95695ff09b42193f8ad8733dac457353d9d9bf0aebc85bc1e62e35bbb92da7b64ba1b2ffaac8b90d5bf4fc3a2ad32dbf5bd6e9293673cd929b3cf7e819d57f8" },
                { "rm", "0cb45b2a244719c1a56f9d7c43c9a00e53fd6a74780cef0feea9c730db0bb6acbed068f7a11f0b13234aff1348f7ada6cea053cd6cd9a5d04664faeff5e7ffa1" },
                { "ro", "b0d86ee52462e7f7e6e9fa252a797ad2a77dea2da609a8a0405f54cfd9d6c16825fd397477074f39ef829054154ccbf2eee5c549259873dad482faecd5bc7405" },
                { "ru", "009a290d73974221d53d584393ab55c93a80cbf28169c1523d6c76d7f8fdd0e78aa6eb6177e2f16d0ddb1531a69edb3ea66ab4d8cf2369d0af96271068ba17b2" },
                { "sc", "151a001df11bf931ace3859dbe5c879bcd3723aa19f9330f23c1229c57e5b6cf8b9fc5e017047929b3d568f9d56651ade0c9146e8869c8ad4bf9c1527fa96e5b" },
                { "sco", "26f4d0fa929d07b6c7871e02559ca67914dfe7b6c99f61a5edcd4c1a8b5faeeb4fc9883fea4f9abe529832f333ce6993ef181f4213a6fbf27ce41c19e668d9ef" },
                { "si", "4494040b7d4534f763fb7ad3326b79b282d80e5ef82fe05e44f5b920f8e0da8cb85cfe2330a613c4306ebb46976628a2bed5fe76a5b511dc4a9901b4bf43ff02" },
                { "sk", "ea7b03a1971001349a00e87f64f558c920e318bc619461fa3fe63998aa4422dcc6fbadf87abd56b07e14e81d0bd303719fb7ffde8c11d808dd65a4928979d3bf" },
                { "sl", "f91d49df8b0f0094bbe7721ef2f6bf8097f5ab93c3e2c51e1d25fd28216da525b009198deed2af3f1167092cf831d51c55137c814f86df7245fd49cddf341c62" },
                { "son", "67ef946239e02b52cd8d26a370c66ee38849c8a5bac47ba1c232c671e616931d3a25552a636c2fbf86828cc9e65ea09ee9bb2f4b257f56631b4683a9009227a5" },
                { "sq", "d4bf8bd19d3a5323eeb814ba7fae417edfacead380db0086b4a94c1e3c08661ad3615edb40c9a429bffe5bce6de663958dffdb746f23bacbd6a7347200613856" },
                { "sr", "8c52613fb024398082367d48c6b83743785437c2691feb93a98bb1aa97882a4525a28016efff44cfd0a019e3662f5f3136809ff57e5f0e623405d0ad90cbba1c" },
                { "sv-SE", "620a38b229f737f21181450ff79ab4f8a94c1d2800593741142388acf042042a2c45701308885886bdd448b2a6db322feac98b9f310e4ec6459d8f9ed96c5271" },
                { "szl", "90de211658b85ff626b51833c9bd508762f251180132ae41ce3726f9230ebf72f27df3c631570786f7e1651ad8c9790819b3bee2ccb75d6fa26595a1444d3a05" },
                { "ta", "52753969a50e1ea63e8fbcfd11e0b6ee8fd46285657812c793137b4dd04db56610ab4a74e889baecd5e66b4885e664d40e2daab9eb22f395617231aeb508b01e" },
                { "te", "030b53c911990be56acb9f23eb0d9592b84f67ea9481cc56ab768082f7cc40311a3b0471c4d06359eef0fb2c0459991efc8ec0a27eec1c5c6c75a85f96350b7c" },
                { "tg", "a2f3102f91fe28e5782d8fe66d3a3cb44e707580c69bef9dd572d9e625cd827838945dac37b671a7aa060960f5ed177869fdaa7a2363d0c41005ee545d6100e0" },
                { "th", "33de806908b38453e489359d7df07b30b4856f08fe643a51db2bb8baaf090ef7a2077c0e292069afb0537b8af5e8fdeeaa40ed11a027ccaff15a97fd13381d01" },
                { "tl", "e37375cbf30240c45df9b58abae536724c81a2ce3d70008e40b6183952b6bf43b3eb9d23ffdd57f1b8c0346d8171946fae02465b56af7273ccfefe5bcc953b82" },
                { "tr", "3fe199d1996bdb691345bfdfde7b69bb37150b55ebcd0d448ee9738bbf3ec823360b408ce2f7f7b0609fd3718c59f9a62bbe740f180c16004e39143e0a4cdf8c" },
                { "trs", "f0e16c7d9e0a7d2eb68623c9d2600cf3e1e50a4ebce95feee69f413e94f3bdcf24ac0a1e5f556c6630d052ea89bd453fd486625d30af932948080baecec2c974" },
                { "uk", "cd24b9a43257012cdb71b0095efca7d55917347682ca17ca32015aeec328fc6a1d43f74c9b6042c4cb4869f28ad4d02226359e14f034d00779a21be48d68df67" },
                { "ur", "53db7d393b726f601715e5b0422b914fc3fead805a2a4237ff381bb7e6a2ad6a180a9e8bf6fa180c70de435992350d290a0e25fe612556bdbf3bbbc064b2b54e" },
                { "uz", "46927bac00fe120b7ad0cfafa64f2a03b5470ebcf10ed0c317b59ae3c2a8ff71a48dd790a501a3b04dfb1bad91820789267e29a6b1580c55786d88fd70e02352" },
                { "vi", "823718c22d39d6863c1ec620ab08fba6fb1b634828846c9807ea7e91ce1506b541bf378e4f565420643829f27e501a4943a430d9ec5e460b9f65d4d081ec14b4" },
                { "xh", "a3a07715f864818fa967fcfabd8eb10b69bb05c2b509e4163bc6c832fcbe04762adf97eb37aa6e750598dff16e418224cbe162a66a329d29884530f51b489053" },
                { "zh-CN", "977891909dda63e0cf8a1c76d87d71dc5136d7b9144d3d4cf307f5c21c1e69fdec9d4f82937ebb6ba31feb2165491220885d3454fc5a0b74b82b1bdd4570ab3e" },
                { "zh-TW", "70db0ed677a434faf7bd613e9cc16b8ef89060db9a5823a4302f4a4cea0e92de917a3662206f7817243d2c3836f46e2d4ec5d6058a3683ab05222479727c85d1" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/116.0b5/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "4f79692c96579f66e388a5cdbef39e6d1d311b21aaf8f091a0102bee8646bb3316979a3c14402f9a24abee973c0b25fd597fd4b72d590fbe39e8e5b064ab8c27" },
                { "af", "d3050a83534a65388d3a1f47ed0bcc0fecfe8a8bc21c3884353394949a62a48ef2af949793f2f875833f4308cf7777cd521000a6acb112077801720f4427554a" },
                { "an", "43b9d18870f092ea51a25b536f6f9eba156f173fac118b920c60c998d5716e57006c61477f55815093a668beabf44e32de189a572098273f086adf8ed646f707" },
                { "ar", "fa54cbbacde00e94125160560de83cd957017154931fd6c1a997f6ef9577ec90ad4492430502ae4a149ef040ff17ad4c1f1e2cb46d8bf215771817314de36901" },
                { "ast", "ed4d7eebd405e3f2ebf3d583053947209d643e5639b37057f4b7fa399cc03ce937f30480857f71dd7f111b36cdb63479eb13522eafd40aa7796303f7b7024473" },
                { "az", "b73536bc99677035f97dae803fd9725c500947662f0e8136aa39c4dfb5eb35457e0d76dcfa67464c658f184e1365b50df8634c3c626c94337e9e5590e7a73f1a" },
                { "be", "70090935e283b54daffd1ed26c32a4d67af03d36d1036e9129ecaae241f8f57070713748c7293cddd1448b7d2c4eed3f861e4510fd4e6ed2b2f8b0295b1ebddc" },
                { "bg", "794033522438c146d77e5e14fffb07dfd37890875165fb937c42522e76112aa0f8783ffc4af57eb61252d48a14c2618cd74e2fdc8091a9f5f2f5605210d40620" },
                { "bn", "1d46d1b158fed148f25356232cc8b4e10ca34f0304e08fdcebdaa6df4a7c830ea9d27f2e8b83de6739240195acd41e38ad1f8060e02c1a67dcf054c6eedb2782" },
                { "br", "449f5be0e6ea310be6d84b1513480c734615d62a5426baa7609d76ce1889f29acb26e03860d60ae4739280b88129ba5af98c3f62823f94b55e62c0d599564e57" },
                { "bs", "2f348b80bc274d2798b33b295577548a456ba34516302747c8aaff981750f4fb2ca37a6a234b42d00692bfe79a13d65fed69eb09f29b8f75da9b3e9d9d4a4bd1" },
                { "ca", "6e3641ae4e87ee553c3df2ddeb9a068446bb1d05b967038dd63a2a4906a32ec54de2b3fc94d21d5801776f85e1075d06464f01aed9bdece81c39fe41038eb09b" },
                { "cak", "1d5f539b412ac5a1c2aa337b0d2451226003f75931bffcdd246ea601a61d5f53c492f377c5d41148ab40eea4a5cb5af3d2f06377d65065c8a304a878241371ab" },
                { "cs", "16210eb750bc1bbd400ca724f0ed2f7242345aad0c159dd2ca8aec5241a3190cf42d47cea08ce8d5197135c96c397c29a92757942f38210e5cfb56006747c3dc" },
                { "cy", "cd32372f3ebc31432ddc806e459b2a54e441b0363f43bfa585692023168db1b32bdbc83c03b83538c3ebc4d3c6224f93895807e1d6f86ceb20b537c0559f3184" },
                { "da", "827727edf3308026ce4be2acad56cdbe08d56d51af3bd18bb8617fcc98d293b628b51b1e7448185b43cdcfb2c774d329f60fc517d4fd257438fd4089b36bb98f" },
                { "de", "a81797352312c8a97ff3bf61974709833bb7e0fa4cb104b915151b56efa8fb0fac094450c6a217bbd1a129eebe5b4462fcabe77db5772a4a6c5d4676a462c504" },
                { "dsb", "abc0b699fad379da6f1c91196d767416664fa5945a00f525a58cc8c1202592b9c386c3392f8fcc1ed516289166cd6420809a9b44d0c6b0c16437b3e94d23bf42" },
                { "el", "73ddf8c48fd36a64976464a774286d67e08f2fa742999a35216b9292b08425caecb10b8860f06c1bd0386da41501b75286d1b4299d4755d3a72f555d8cabe011" },
                { "en-CA", "951445bd7ee84c38a1765b0c14a89b55d47b5a3f842abfafffa8a3628591608d392a9cc1984d07e7851bbb4fe4e71d53234116a6d746973105599fda4562b63f" },
                { "en-GB", "db9ee9be80077ab6c97cdf68f19a3e30ff4245b9c43c3d881f1ca59cb1eebcfce4c0c8ea6033cd2c7a8bdcfd9e75bedcf10faf5762d762fb7609a4881240e1c5" },
                { "en-US", "049e0d1eda06126d842cd2ab6ee91ab13929b3ed020f574d1acb33f87bf06c17a1d9ec4b40ae7be33538ed3823c4070b37ac4c2848dc1ece94629597a3671a97" },
                { "eo", "68c414c4694dea9edc9d45e7e27830375d828f98090e6d25c1bac4f65687bd8e6cd4311a3246477ea9c99c25643ed4c842b7c1308e5d2e330116856cfd729542" },
                { "es-AR", "85b8ace6cbd462ae9a112ed08305cf132c6b38264181a0af6be96dd087a217f529ac68fb614c788c386622e8a6fd78ea4b973b16a455e5f17a141533faa959d2" },
                { "es-CL", "6aea19fc7a943cfca13a92619855def6773d8496666ad181f1871fa8ceeeb71edff39b073e74a9d33ee147de9d1d8c2778035d6c518cb66fae3153604d8bd85d" },
                { "es-ES", "9f05de3c9b033ad3fe3cbfd9858e47ec3e7681c80b7931abedd5c9c75a8e9589845b45f7405c37ba78ab36ecc641f4b7031fbf408a7847d5a2507d2601e69261" },
                { "es-MX", "cff61d8b7b116ecaea7bbafe90296b1ae7633d8426a18d104c1b7acba378cbff75e5799f67680d915ec12187cda983e058e24b000289b98f098375c1da504991" },
                { "et", "39696a1c9f4addd58d05a76806969e7c4ea6cf4a388301a1d9667f5a463fbe46aae0f7d1dd61a1c082fda84b7e446f0a919f3580991c5d688880ac6dec676320" },
                { "eu", "08b8d4467d4e9fa33aaadcc319483be9583a1b2da75fed207ff5908fca01f4ee0796a69743755c716dd5aeca1a3d40141350e546f7c393716ea4363bc9f453bc" },
                { "fa", "4dd8189aea7ae3433c69e4dcdbe1a7335b2563ca7d14f76c4cfd7c4dc97c441229af19bcb407118df9550819818ea6ea2f43a3b648e266a5f214c9933677b088" },
                { "ff", "ae3173c8bbeb49a817111b2e7bff89e54aa329d6c8dd10d0cf93e3f13ca0e11ea28326dfe637b437b97db669912508d481c77c40ee910ed1bc9feed7d0428d7f" },
                { "fi", "72450a4a0af218fbcb5c43099c582b887e98ffec370fa6a0a87c43b64cbb7572cc6ff300ba1fd7695042972419e857fd172af43e040f6106d3e5b093ca72760d" },
                { "fr", "89ba2bac45cf7393980cc85bf55d09016f98903aaf447fa6d7267e1bad6ec4271fff4d58a6b870e71ce99b4eac608690a91768c0be5b430feb631eeb144d96c9" },
                { "fur", "85852fe2b49561fa428ad1ebbe07d4df2625cf638a3947c6cb628e8ed06d9f46d8fe974089a40a8f845f4408e114de0f12e53db1361e0ce74ad9fd19510317ef" },
                { "fy-NL", "d7019e022bf3afc36b61bd9dfd2823298aba6232035d760f9e8ac923ec0223ebfd44aa016d3ac9779c316e46f7a664c140a24abd5446184ee30f172e356b4493" },
                { "ga-IE", "1da97085fa3ab80ebfd5843d7beb21d36f5d3057029a4010d6cd79544936f0c71d3c94e20222cd70a9c5743e9ea0f13c42cfbc07eda9ca6abd7781a2bffbffd2" },
                { "gd", "c9a4d6668e87a7c7418ac31dbb4ef074468915dfd5c504b2b3dcdebcad585c357b0249eed6b6cfcf51a401b70794bb9c44848d22bc81dd071f0564cd87523bdd" },
                { "gl", "e13068719f3113fbd7853eece4013217cdaaf17841dbf52fe5e9596dde1eea8791b5246952016d5d29b098c2c3b1f5240d27104fccb9c00276d04ae149db0fa2" },
                { "gn", "38083617c34b7f75ed27230b08654ad94408588078b5cafd66fe9e1b9fc2e2d9e4157b9dc776eec0d5527f25a3fc3a0627af6d7b51b6d2f12e5088cfd4eb4a53" },
                { "gu-IN", "363789668e7099156d042c0f73281f353ba2032c76b743923f6a22c8976b8c7a57a6b45b2b7098e262a363afddcab5f29e916f82b654d9d7680dcca1270b5fb9" },
                { "he", "1595764f2f2038963e04784aedef4384955593ef973f608db6d602bbadc7809b639b26f65cb1223c9d834b2b95826cb049eda4705cd389cce340f797bbd73d27" },
                { "hi-IN", "9cfab5518b93f24f273720ab03cdf6416b167ea9d0f85d0db30193867f8f87322a8ebd0ac4bce3f26bb528252f47aa977cd8e4ddd2be7a849a667e8687b12e48" },
                { "hr", "78c965f8eeac5fe10af93555ad61e9b9ee42a1c07a8cf87a12df64c7b50fd58fa95ad83e3f790bdefd191b7a755a3d5c63a34afd7100366c65b131300c9246d7" },
                { "hsb", "dace6c3918118039e117d9c2466eb0cea16add92765afd163595090c141e3883e433fd2bc91ba350f5cfbe999a7f4a778ca3385cd21c859170632ef7e2b6aa94" },
                { "hu", "672479dc435f73215ab9770caf69f7c5da929937933f9feda241d3fcf7af17fcfeaba19f67e8dc13e1a4fc684e413bd3e5ca99c5caa95544482c77844f201ba7" },
                { "hy-AM", "00b52b9ef4592dfbd0131dfd17a31272b9ce939a60b0fcb4a5155c98dc19ff9361b657eb739d68b2a7ba2032bb6f3f879ee9ed5ff9b79314c006bdff948aea85" },
                { "ia", "ddda1571cf332ec9957fc92ba94f4640fd144b7a6baad690b0de0d1cf8f7cd1586ea15588b75bfdcdb14c718686f62bb79325433bec02a39f8351b8374ee4766" },
                { "id", "1b95febe4264fed02dc0308f644b2860fff085913d12a51fa56449562e04dc2b3d4b33160583a9d1fbd354ed180f08e832f76fac582ee09d4848dc5c55dfeb1a" },
                { "is", "3ba6f5bc728fe7c73bcda634099d54be35dbd26e4676fcf787fa49d97428d964a67c757c5ae573f616ad4d2153e2b055f56af3578420e20023ef7707a4ffd4a0" },
                { "it", "41a34f138132d16352c8d8ec4bbf241e66b12a0d5a0186c002c72bf896efca40551360ea61d0a85d7c18cdd5318ef1c392eb455d55f0a9c791f7a09d2501e917" },
                { "ja", "d03843ce00584fbc6b3cc0213db4d6a79122530ec5b15bb0ae617f78677df4d3b3630257ecf91c564e1e666b07a189baa9c4bf8eb028d5d6e4293358ca0b5058" },
                { "ka", "44b202f6462239d6cdd33800a77a917e69358d6812446ea9e0d742e2d8ba3d7cbebddf3aa4aed3f007cd7801586f34a40663ec65c98de57eb5e3c9152f5c495d" },
                { "kab", "5c2c75153e325f6e6141b74441ff2ffea8827dc2ef5fa3336db86c94867526a39bd22c1738db855f786e2634a6657950bea7e2c01ca8038425f72da090d4716c" },
                { "kk", "e983e042060999f3661fa7605834219b7dbf826deda8d0f495491850f869bc9a4adc228524b0f394edb513f99ec2832797994253649b40b9ca1bf06abb19e062" },
                { "km", "e0f54d0d483aadeffad03bac4b6c9e7ff6aa334b0092766721c0fa3d10244887d350bce8ad9f6544addffc923822e1e272bb64e39994ef7362fca71bb42e216e" },
                { "kn", "08a97a7a5efd70a1c1274489e4983c26122b4a3470c0bceef2dec685855bbce1b202a334537b2b826c790bc51e85a5ba74cd8f9e69a54e947cadc7b46c9f7e40" },
                { "ko", "38b181349bda3dcaa27e3783afb132f9a633c83904cd795a9d21a15f238b79f26392fa8f78c23dbccc0dfd10aa09d6832e0df44e22d6015d072e7b44ee186d9f" },
                { "lij", "a70489a28e3b4bcb205b8403a5b585cd7f389431942dcc2a4a81b6ce8a1dc1b7548b6ef61f8223df2c957c01edc608f6ed50aba04cc14fd5065adf35cb32939f" },
                { "lt", "afc7c58426fcfa0b4c450fe543700899aab0fdf9f231b107ae58b584bc1bb77d377456de8d08e04f1ee0515053f86b2025b01f506a3eaff27fd47382b8880898" },
                { "lv", "b5a6427fe3cd461706360cd6318f604910e326bdda37dd912a0f5ab4e322e6f3545092d75724af6ba45fb39acbb7e9726d2caef69130fdf1cce79a051b405d6f" },
                { "mk", "84b564e5098194a279606fc9de4d9dd5914bba4c7a775acaf022831b63a6a5e38b0e65b1d0472bc8b42f486d6565774d34bc1ea5e5d24173864181db1dc7aa2d" },
                { "mr", "11fe65ef700fd8c02352633c39992b8af9d1c5a92f57855a422020c7d2fce9e1ece15ec391eb82f32f2000e153f6ebf2fb17cbeb4a01abdf0dfe2c8b83aa2b69" },
                { "ms", "2a1fadae318a0e73fe33cb7d419ac243bf814fb6b825976bb89bc2d99c6cd5cf2daf51617f2b1d936aeb6a88e552a19be7b227490683ec7d344cb01c3e0bffc2" },
                { "my", "7ab1e331f5077a72dc1ccaaa09d66b30b6565a3af0f97138bb64a02800185befab9dc3bcb2a80576b49149e1c1a91f759976d7bc96203d41c54aa7b4f462a15f" },
                { "nb-NO", "dfc150a25b32f04c2ac22f8df385014a833810bd8ad581a57558d3829765dfac8f100ff98daff5c2d6e361f455482906c50bef9883704a28ca7d2d6826f546e7" },
                { "ne-NP", "18acf4bba7920e795f1bceaeb120b4aa96d3c8ef29bf13a10de974b518e84d31ad6373ad529b535724950ec47cb398425879b8f792396b53fc1a27505b7f0e76" },
                { "nl", "eb5f4a098bdd1b6d8eadc7dbb6335b80f562261fa53070909b5ffd9b2436339585b38e14bd772ff1d3c297df04e003af9e9f7a5396de0413855c89e39a513f3b" },
                { "nn-NO", "eb1cb7442d8daa376a68d5461c6101eb6694333544fcd4142f197586d6fb755b1266e53c3df6f263c3aa6f5cc1d736432ead7de5e56905b063b30e1900b8ecab" },
                { "oc", "c9ea3624d0a29228c797900cee728c17873a8a9c19ecc1e947a0e00667dfd0a9330af1588e579feb96b93505490cb84de7ffd8ea57f629d0ead16829d6188854" },
                { "pa-IN", "32e8cbf9c4a727131feb4d6d3f0367d12d522da5cf7989493f3ffd5b5374035ae9b423ce6cb7a1408ed03d7dc8963814fe5dd4cfbbd13b93efa036f261efc8de" },
                { "pl", "46b4f889be411dd65e5005697a1191bf5899f234ec218d594b406fe194b54906910a4057a32176e15e2b6d56e26c6a6d41ada9c85c5167de3ab403a06a5230e2" },
                { "pt-BR", "79784523c0a572be6f13d281f84ae1e4b215410919ed27e04f24818b7bd356544044bf4e8b5fbc62a59cb71378515a9d1efe26654b17ad896e6ece4b9d0a8827" },
                { "pt-PT", "13c34f066d123cd672327f8314b4d3534be6c234b501fe2d3dd00502927bc7af61e79a7eb02299a7521583dad6811bc214dc8d40bde1408213d34c6f22d10206" },
                { "rm", "8bf19064dfde02d3a789cc572f227cb821f3a214f24f0eec39787bdc1d4c8320ad150487ae934e124f2ec4a4656388d74ad0cc72f0e4a5e504b23929264226a0" },
                { "ro", "7b2468efb826b9c98b41521fcba4ede6f30bc55da1afd9d58d9929e4d31641594da035dec29961dfdedd07af8edf5871653b9d7be585f399b225150fffcbcf2e" },
                { "ru", "251616eab371c16fbf4700aa256cb62369d70e8345a5fc083cfe69a5d6001a2a94fe801f03eb159359037a137e0d1f767181e65c57ef424e3e29a9d1a82706a6" },
                { "sc", "b9d707cd486d4c5c6a43ac6fe1896136ccb1c33b88188ac01bfb7ec3f3f6294aa5ed579e0f2ebf2fe4387ff593fec07111d1c2bddc1a977f4a2447d11103ae02" },
                { "sco", "ca9bb35171532738aabb27eb7e9976063aa0088b98f94a5c25c7258e4625c492ab757cf5e46d9cf96599b27491f5b98745b5fe36109675315f18738085796559" },
                { "si", "b30c55950d780f2c9f6610d929bc0c65902b5cc7ed267ad52e7e7785ab93a26d9a6099e2987d82e2ac023d2728b1a1ab05bb1bf68cb1335fc8eada05359972e6" },
                { "sk", "2d238645b586916b274dc583f2776abbaea1f22eb33965714d7d5601bf2895fe6325f169032962f1c93fa7710b2162ed59e09fff33401603ff82fb1a76930d0a" },
                { "sl", "8a00e84a14044479f9c750a8dd6473ca9ec54a2a1d77d908071432a83c757fce672936db5407016801d0819de587566d334ef524e55e827f7a260ce5050cc9bf" },
                { "son", "395eb4fcad2dec15f74a8a4a1eabea0a56bfb8b5bf329872329ce1d0bd471d2e1fedca74b29de69c25fb9819d96f86a5ff5d7a3dd94e6668325318f9eadf18b8" },
                { "sq", "140c21219d3c80ccb8d44ddcf870fa44ebf4e781f49d1bde13c923db275529e9f837fa8e5d27aadfaa908e6c6c99e074b3cc6e34de1dec8d4e0f3861eac47155" },
                { "sr", "947150f5835e231a5bc449d7cd3768181d11849cc0a3787472ab1a3145276c9fa677be610b72249203fe573eef9c62210764e1a79ad90eb659827ec3df924c44" },
                { "sv-SE", "d5fe77202d1c9735c41ed2d382378d7b332d22a920d53d05d8263fb1721766df2ef9b5b8a5f0b934d92cbfbc22ff595d5a37df9e166030d235a7af392d344e28" },
                { "szl", "794d7e0036c5fbf4865b305e2d7d6bee14d8e9546e830271e108f32397a5703375a1f204f5b672887ac407a0f07a9937388a85e81e07d22c22396dc38d2693df" },
                { "ta", "94dedef3a04087f806b0196c99a21a6435c17f9d98f6f3eafce571f9303f67e5509e72a4e0772b995c0384923a353b3f88995c9a31f20d8c644ab8daca1edce3" },
                { "te", "cce4e7521eb57aa2258f66b268e65be4e6803d18a54bc3232251b95d5ab78315e48ead8fbe40caf07d57f9de8ea2fffff90f4484f2f6e8c188b0f0b81f6daa99" },
                { "tg", "aa5e6e404d1171134a0faafec02c76ebf4f0008dcc346d6fa3716b78f76852be7c1d60ad12a39ab068ccf2a574088472b5ed515af3afe75452248f36b94c1559" },
                { "th", "850ae0d51a8aa348c117d5438ac842b1305add58d4c09246f2143dc564f4c7c4aaf727d4451cf70e0c321b525b80e3f212952204cb251d15e7050abc5df266a2" },
                { "tl", "d343c7cdad59654495a48b07b659f6419129a82e29affad8d2519c58732ddd9d57fb2cb0fe48db65962006050a47013fff58060da389a497dbaf29aef431072d" },
                { "tr", "1e875baaf32c1df60e6aa527241617287e4a05c3e28abcf62187947d2b1d9b2954e56a7bf8a6808084588f6eb44a2571270aca63276aef8c9d638f3fdf2816cf" },
                { "trs", "6062b0147331a16d91374a2eee470624fb443c1862b5ca97134f4b7085751b56025032f2617472fd1855174d63475659be6f21558a957cda189a68cdaec76e34" },
                { "uk", "a7caaf23eb2d3f498af65da5ad821405b54ca9af8b0eda198c0bb939faa03a509e2cd0e4d67746804d5e8fa73807771f543305b8f005b4b7181ae6f5567a02d5" },
                { "ur", "0b68ef3136f6b22e09a5ac42d3b1e209b8af9edf47bebabecafbd094b95c44a7cb90e7bdf6a086970844442a724fb2ea1e986381e0291a42b871c124887baeea" },
                { "uz", "dc61c8211491464b3c77c838b3e7c2afdec3c281927e9fafbf0ff6a4a6d03a7f49bab32f1b9c92a639e44f0cd893f64ab1d66f90159280a25949a3cfa490fa02" },
                { "vi", "0c0f920cd5f0ba0f6a645103b0c3f7376b7e24f3d9ccfb5e397c8b59d637987b706718d481764df2ea5f8624fd7cc8c347b1f4aced0091b5e7912c6dc2991c3d" },
                { "xh", "fe2e6628a8528cf68d3c616a9616381bcb4ebd4563c8f1e79b8e0a3742e6a2b8b76980e2f3c016f9500c59f7ba5e54498383e1e544ee6008d37da2277570b79a" },
                { "zh-CN", "cef94bc5aa7ff3fd25b47c21fc7c56b07e24b4a03364311723a607d5e7624a45e0015fb07a13f5560a051e8d7dd1fc6dc2dbb994f11f412e2307844f98d06a02" },
                { "zh-TW", "f30c6cdd7b4a6bbab25248e8edca97a5bb0f20e7911a52a4a0a55f6ecfc9b1f3bff95601c70826f1ecded2da912ed1e73857951314350f743497e1a22a020710" }
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
