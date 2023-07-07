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
        private const string currentVersion = "116.0b2";

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
            // https://ftp.mozilla.org/pub/devedition/releases/116.0b2/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "605cb4bd7a4e67e1b87ef9ee5c98534023f2290881d77c008cad1c62c3a20bc19478dd651c6b89d0f2e31f823f5bba36bdd6e51f0e30e27f87c36b7d2d8e15e8" },
                { "af", "383af4cfdd851f4dcdd36bf9342c40f04cc3f1c835f84c3f5c024fff287fd7b54a8372e283cbdcd541ce7856a1777b74dd14806a59463b0bd0e2c730d7370ddb" },
                { "an", "318cce373a35ef7b065612722a5a35bd21025f9aebb85e4603ed7104a3fd7f9e4b79f32fb057246cd53479830cc91ddd133d9fe9bfe605c6bde86d4a19e3fb19" },
                { "ar", "a2b889c1b0d096906175ff324976f5933d80b3c8f6c0e7d1bd835a48a843374645aa822a22ae91149daedda01c02e326f4ef233dd437ce14ee5bc5e88f697e0a" },
                { "ast", "2829233c2bc05a1b686254ba74fd0893d7e628b8c649c2065399c8e929acb8c8c48fb112cb6519ca38db57f5e182e4a41b3d9562b551bbae84e943f5d456fef9" },
                { "az", "e148b21d77e5b86d0b79931f479bd154e19711bbe303c57d4a7a0ce9b83c094464c64abe55bd7c65cc22e906ab0df01c9b836389d154ca62945c7f58e9bbb014" },
                { "be", "dddc4a52a7271eff2aaaf13e088096e6c064f067c8bbcae2ec161760db3f69919b209567cbea380717940fefe294052de3a24c0cdd5cb85cef6ff67eed982bb8" },
                { "bg", "83e34d48112115c0f3160b400f2aa3e50d9c83732037a149b01100bd3e1e2ca92eab8f4100632c977ea99bdb6eeef1030a41f49bdd41d761f165edc65f70f46e" },
                { "bn", "c9c333ddde91813d195fada5eedc17cdca6ad826905bd518526503ce84c2d1735e2f6fc1cf414cdd609cdaec45b74154930eac686ddcdd5d86682c37959fbb29" },
                { "br", "7e983541acaeaaf9483b91778bbaa49b585c6e6d57d3ddb6cf89854925825c26fabd535c401b5a1e1d308be46d3f9ede054f83de77915ff3a82a3361b42a3134" },
                { "bs", "7014376a7273b335dbd7b362510680f354f7a00bf87640a97a294aa913781e7b609dd7cc34691b8628469a4c87241e08d3ac2a54ab3bcca3c2fec35d128b0aa2" },
                { "ca", "c8cb9c62acffdafcc75cbe499b7365a6761c86b9c50ae85fe71191f37e3590abb4c214dd04da20c14f7e7ec65b1a8b16ea17a5db62c26dd6eabfa41b67fc0efc" },
                { "cak", "7205c1d80f21b22b380256b918e4ae40fbb17a836327b0c21c76a31ce6f689c3d51e7c9a56d17fcf60e22b2667384bc88b2e942860b6c92fbaccf4f1947019a9" },
                { "cs", "a57a749667148881a9c970fee3978ba06c78dfe1dd6f14d21959100cd00012c01e02ad2bb9a3017d9213a4e87c34d907dfd13a117c2fd146e10f1318cde43b61" },
                { "cy", "0f9a5beeff8210aabde792555fa392eabd235eb2b7697d43b8009eb89f5353acfbe2780434a49ab5af100d8fc1b7399f401e5e342d158491e151446e638e250d" },
                { "da", "f4fdfd4e6f2ce2c9bb372e54c6e86bba3d2919fc25b90f194be2339b6940d7c234f702fbf118a2c3798011bc0f20063107bc5c0b8a6472bd3250c8b67715f49a" },
                { "de", "fcaf237bda8b3672a7597ef86415fbd9f1bbce113504e0c1c78fd06aea6b667d7c9a477d1c5071d6e68de20c6f44b6dd9f69e74a4bf0518992143134d6091b10" },
                { "dsb", "2dd3fbda9ab91d512cc328c5ab3c18ab3f5897a293a6792ea0a3d3e87e7a013fe3ce80d0df71da1eb40fbc6726b1e75a0b582c7b03ed4f37028a4af4fb2104fe" },
                { "el", "5ea4aa5b40792920339f7e321a4266eb7c3b625c8e6ce5e1502ed8f17d81a017dca18f04e2cdd3e5cb0127fb2446c887ee0265a36806719f85a7fe211f820f30" },
                { "en-CA", "3582ca94a0c762d7da30aa62ba3f090e1c8f10cb5eb93cf02d453ec670c38b818109102ea3ab109e195d5d10780e432206a2b84f061632eaffa7d6c75300f3cf" },
                { "en-GB", "a3d9a319bb98282948f16dd1780371b8fab14c3844d605242151e62c41457b850dea746c6efd4fff8819ff9bce5670739f5089cf71f5b51347df1aa2b09be606" },
                { "en-US", "4e96ef02b81dcac8444cbc844ea23b8d9a100b3dbdbd871e66963e20197bd931d14b1b8ca78c2d8a3afa4f02322db9025f67f0d940c869dda5a7eb8b4b297dc5" },
                { "eo", "6022b8e8eb6aef7c9c5ebec841105b8f3a8e163e6b2b157fe167b5d6c7fe3a03a60745d00b8f696b295c673c9fafd953095a7800a6e4ecc34e5dbf4dc1ccff14" },
                { "es-AR", "4c5a5381cb1e7908de7d66ab9392d64fca5fa1cf53e4c4e25a226a08a04d4207275821e435db6b9de336b3e514d358d749db84436ea1fd6f5897513e8fb0400c" },
                { "es-CL", "a3d4618200e35ae32908d496ecbb8684928ea1a74f907b36733aff53f75cb80fb5cfa0a5ce32f54cc56f5facf7baaffe3e2361933f5722d614ebcdfc1607c705" },
                { "es-ES", "2b60c2288f743fcf739360e4d4def62072f98e68ed392c0b254e2db9c290df8c93b1e615b88ac45d455397d9d871151c167ab2806c55fc8457d8e90c6f3a658a" },
                { "es-MX", "fc1673524a8470743556ff7f6e08e27f4afb9bf57810dbf40f73e5d5df2527eb69766a56edbe2d3149678ebf411b5bbf18b83576504c129e5f84f4b22409f2f0" },
                { "et", "308c933e779f2af6db7b1e6fba97f74de8b026f44aed7ec25984690d7d37458c4eee641b1814925d570c0300c0a30b8e1b4e42d976e27dc9621aae48ac81d741" },
                { "eu", "96654f5af75879b1bfa965c7434b053ea785a03b9a9a5bcb062d328c2cb4ba591dea12b6bdfd0c8d570b71e2f6d4a1c5f58ee745eec3397cd68bbc942f775203" },
                { "fa", "4b2f05dff2b37e2a9b2b93e7714fc0fe49e8e0e1ef40653c50d5c2fe0b5aa624041074481f517855f1a43242c971ce9c1ac2cae63da573e29b2d04db018e7c40" },
                { "ff", "7a4f9156b4c20c0abf40c4ab2c0b068ca2b8afb2d2e89d4cbe18ec3bfd47f91336a864579b1e0a938b75bdf8f9a80fc752879d40c5681022423b8463edd84b84" },
                { "fi", "328fc8246e7e4fb86fb6b9ca7ce63e3cb3d51753f698aaa11a177804cfb7478bebab6ffd49519c22ba69cc40edb8199b3dc80e0f1da1631b0f1e4d15f8476858" },
                { "fr", "0a491c475c287de9729da3e1cbf00ed5afd3b4a73382ace407f004367eaa6a2597710ab30eec89d72901c2cd29796ba08e24bb1e301ec6ac05931bf3fe1d238b" },
                { "fur", "a46e50c419f9ad721a81fc43a7db38514326a7bdf06d0e11eaf99e3c5ebc6bd3fb334be0b12f1ad7c97b1d0a8eba96be453e8bddec2961132f93fc43b40d1ec2" },
                { "fy-NL", "3b099e3e67579275aad72893daeccd3ae2d5e35a22e252ec4afb7c1998da8a4c3185763df6de5ca19c32ed911a0075c2662b9969eeef3ed8725dc0c3f9fa429a" },
                { "ga-IE", "8efa6ca47847d951d35790cb32a5a91c34347955d132415a0ba9d4eaa4960141d02a3dc4c82a3817be504cf9d7d125ac6871bd8a034abad8ff3aed1efeba55f2" },
                { "gd", "22002f2b06c5d63eed804ca9426e85bc035a3ea4308beadd293986053e93d6a873f797883b5f758a91b6fe7ee2d4bdc71bc5f1050e86fc96d526193c982e5420" },
                { "gl", "0183b33a892fa777a87ceb05e2d0f40ed917cb9a5cf6e3a44b82a7e80b5848047cb345c5a13ee726f2c514991df7a220b6f7178fdb3b7701e5e809e8e5aa17b8" },
                { "gn", "9d8a15e27f8d9d44566156effca955d38c75c4d21769d63be6c09f6daa9f2d8db9041e0df7faca3a48a4c6b1ea9570ea4739235bbd85a417baf939859d145481" },
                { "gu-IN", "06eb38f9b40f57476a8f6273da2cc28a39009702dc28c1efe6c337ea43507d6a62843108ce021f5dc6deef402dffe783583e37e6b48f6ade2123d6d7ccda7199" },
                { "he", "f77ec9bfa6eda6d2ae35194bec0ae50cd4fff09ff15648fa1ab5355dc14547ebd2187b8460162bdd7ef28419ea5db1c77cb804a8ad106aa65ee10f114a3ba073" },
                { "hi-IN", "97a0d25630caa5541b95120c33c2e83fb4d993d0b579dd6c1ec58dce3b2ba8d1d04d52b3de2d999755acd28027a32aa0b1dbf045ab2cbe86f2a8ad37de674eda" },
                { "hr", "a49f0c6c58ecec36851bba58e6edd012ae5e6f46e12ef81e85ef7b901c5e2d89d1fbe482a6ff01924646de6b9413165be51150b596785f5a9d6d6754e3f615e1" },
                { "hsb", "f6d2178516cbd3132aedecc159a7dec33ca1406deb1ff191b7bdb84dbbb8c7f6b994d0bf7d6bd8ed8beb5ce7ea2ef01fb49711a808df724e8b32851f4b9a29e5" },
                { "hu", "64ccb55413ee593c7d928e2d82473e0eb7ff584051a1be9f5acfb930803ed2f18bb74837c44b4fff05fef83b05714e2ef747e3f38301830f036a08cfd36bf4c2" },
                { "hy-AM", "d178e3e6c7fbcac998e69954f9087aaf54271c657e5cf1986add4962dd7b9a606e6b3cbd1f04b75731a2531b6e3e5812471644a0b5908d646a1838e1b89fccc0" },
                { "ia", "c20a37de1415804f085fe056ec260c17a4b70c9b7abee6068d6161318575634641d0c016de467b843e4411ada04eae325217b439469e0692e8f5e403145f7159" },
                { "id", "8b323cdd8d240eec0db9a712f41a993b5e9706ed422c26f29788edab18a760a62ca68269a71c2c8aaed104c034a0cfb794203cd7a4dfbf6e6ca5db324fdf928d" },
                { "is", "4fb2dc8e35b6771eb440268a3d61affc352107ef19b94fd0b77b7b450e7d78de2847bc1810b6b1f04faf52e7181687e6b77904468a3c1dbe5fd258a3c9fc8083" },
                { "it", "60205b5aa6a508208863cab2bd1f90cfab2c346eea11dadf2634d74bf5cfa3065c92d979622bb26dd4b5a61f0c5c7c13c23172ff14fbbd5a0510b5e8b06fee53" },
                { "ja", "494e8c46c51c46398b8cfa035db0adb2689c8f098b562f26da90f376eeb0782b277be9e0087784de8cfa569121d10ed42dfdb1bc7614caad337e2d0853e35fa5" },
                { "ka", "11c01c0017d2eb08ae69a6d997d57c988647b6049f40408d79c4ad8257fcf181038a1ab723cfbc5fbd632db901f2bcd7387b24345725e2a5d5ca220347f6f3ec" },
                { "kab", "86ee4c2debb1eb94d404f866c985ee961a0276cd2db96620906aed8c3e73aad929af702de9bd83d3867c6f183aeb61cd5516a54009c77f34637d469152ee49db" },
                { "kk", "262f9079a5b99ac83262e16d7d70b282342ac3dd43d9d7669908c869d5ab9d994dc1dfbef99dc2eac8d4f3f7a600f4ec5c1c2396aa97086f6173e5042783b287" },
                { "km", "c39e81c8d584db4cc2a478b4d32ff476f247dbc133c78da27d4e72f27a5311ed95069c1863eca32d4ca98f1d6cf071efc1d8ae595f6bb9dead7fcc6deed860d5" },
                { "kn", "33eb8cf055e498549e5e75041438250165537f7fa1a35dff3c4af78bd97f7a0d803e2d037de24e33635d7b132bf75503033cefeb1e4e77dd23132298b63c28a1" },
                { "ko", "3a0c2f848f4cd91879f613712bd8e32d5a0beb4392fe59a6e4c1c9fbf79bfe6c2950cc033fc8b3773b1b89f8b38524cf392ef7b9ba748f2713cef6966e9280b9" },
                { "lij", "b77731c0ef3e8df19b614c286c1c1aa9455eb6a9792181f214399cbf4909a94e0534a9fce4ec256796dde2a9ef4900673fba67589ffb1cb77e2d8aa05c6f96c7" },
                { "lt", "5cbfa0c75e447748d22cc4bea0cfe69ceffcc7b07b98bc874fa7e0ea3880f4ddde68b4b8c304a536adf96db9fab32cbc29b6574449b6686ec087be3ef9109886" },
                { "lv", "05b87bd3f7a88075ce7fd39b63470836b808dccf0fbc53692d73604fd826468c28cde8bd8ba9b7d2c9ead400a572fcca4ae46c4934ac7a5f169e9c8aba66132b" },
                { "mk", "6a781e4d970685d01e83e3bf1e4a6ec6154134736695b588a019117efed5b174e6e93b027aa2c956acd22a239613eaa8507d814bf0c6cdde0bcce027a6f0bdea" },
                { "mr", "a5c18ea52db3fae630080c2bfaafc65d1366c7447186aef38eba7baa1846c5a10320d9969571b5399faf42340b9ff2f8cca8b5c46988f6eda0e09bd214242dad" },
                { "ms", "6b5c0448d0eeabfb0236d56a5bccdfd932b74ae2a7a4ce515c864f03b3fbba376e651248ebec5fe494f88f3f24bbb2bf5d6c14645479993384309589f3753e6a" },
                { "my", "d1fe6387e2da90d4c98f2cb91775c823888174a427c672798f8c0d41abb9cb13fa9fefdd1dd9106329a103cde210c74220624fa8c34a1c70682c0fe4af916fcf" },
                { "nb-NO", "d5aa73a7e40b3f46006a557b149fb61290ca9e45127fa3554ab01cadcab8575ed5cc27514e420a0775aabb51b15653579cd32260802b200ecd607790c789d8c2" },
                { "ne-NP", "9d214fa5741f010c976708510be343717edcf5c853fbc7dc0c87620a1c453071462c2e7df110dcdfb307e154a1d0bcfaaba599efff7836a03e33b09dcd29cb02" },
                { "nl", "8221b8681c987489ba0e2be40581bcfe1b4f25604059d09047ade16fcbe1142605e2361f04506d2fb36a21572098adaf60b43cbc06bc9a3d571f657d0c3afb96" },
                { "nn-NO", "6bb1183a8298744db711c21d35c3de246e752cd2c131f22fea453c7084e7574136531ef9d58c2109cede3425672224191c3ce0510bca4f075df70a69ba47b960" },
                { "oc", "e2686635b9c6560e69a4d4617d432365db1cff667a29a9349538ebf10897fc65d8d0ceba5928e820aa6f32f4272a4c8f661f278b8973dfcee84345eb60438920" },
                { "pa-IN", "ea64404c81c2c7cb94c96e4f9156b4d84a644467d2f00b0034e849d77ae40d2089d1df37487d321cfafda42a7cffda2ee044a949f0e508b4fa317c0f0e75fb2d" },
                { "pl", "1e82f8da4e2ca57e47e2854435c24c131f31c7fa1b2911dd653b5d587e3423c0050f9d6ea25294cb320db63e22d17f6718cace2f3a4fd1c9ab9073fefb9c4fee" },
                { "pt-BR", "ee710d9a919840ba7108baa26eab2a4829f1ea6f2134bc82f7f1697fcfd3509ee6102064c20967d9e7e2cb59148b4ea128770ac7b5eb7a75adae96aaeb5e6022" },
                { "pt-PT", "a07bd1cdc8631f804f7bfb977e5aa7def4212a4e13f1f2d9cbb1ca87251e62571ef38952499f547d5ebe57be36e642a3c9a61bb5e4d2cf2bb14d3fbe3a5e7b22" },
                { "rm", "57eec70f435b7d7d36260b34dcba66376b47400c233c6c22662a96b5f61f598cd808e999ddf63d167b3c6e9786a301d4e8bd8c0a723050f92b62b1251ecd3745" },
                { "ro", "46d1f3d97120dbbc19998c04eed829d7bef9f8cf2fef36df678d460c35fccf7d9d228fbd3977cb04b77cef85e112bc964ce913bc5524c0a27e4a77cbdce95483" },
                { "ru", "2679477136673487b5dd2dbf31b91f9b55bccdc3ccf8e985d125ac649ae6768660257b54aa5e3b2f076d96d8886fef7d730b17ad5e17c07026917abf5c5978e3" },
                { "sc", "7816f9b428927be9ffc03d269829fe9196abb3c4ab05617542722aba10acf3e55456d806e66b82c900779658eb3d0146368d348da2a6f51d31519464ea130ad2" },
                { "sco", "62febca4ac636df7dbfb51ca329f980367bd3cedd20ac488f9d4ef5a5b6b1e922aff80a7ac26b76a48385e6d114cd83e58feefe21748ee33f1206acaf9a4edca" },
                { "si", "035fcf06c6ef5e252897fa7e667f2289c529ddd4382bc1c093f9d5b84a9a26543ba21b96f463f61db99c6577e4afbdc3ef85c8fb7a34c8126f47ddab38db28b9" },
                { "sk", "49e464b0f76d2bd21c291948c25fe0f86d1ba14da20ce6ee1e14aed9b0b1210836d402d52af50d029ef07e496f5568895fc687a32f5a6d917d9716e2ce4b0747" },
                { "sl", "cabd1b239e02e8338858a3056d244e4f7ee4250526460b625380bdd8831fc0e578079155b40809ada898471be6e2df477b5bdc23090c4ca0ae89600a25e825c1" },
                { "son", "f1f83e308d102d08f763a842aa414845473965c473a982d423171fa1361b2b9b24e67f1dce3991d31c2a24e5925e01af50db58263237a143ade01c29db854664" },
                { "sq", "f2a6d73eaef7726aca207b4b1d5dc68e30afedd8b212bea17e36379fab2ad73e7258c9e4021fa540210707c1f203337b10fb0a9854db5a5941f9d219df0ace76" },
                { "sr", "d110b79dc7557055b8a375d0f94df00d7ee9480e4323aa7dbbb2f4399955fede7e652d9e7844f07e9ccdc20f6438fef2ebdd26af8f07602f38137092b700072e" },
                { "sv-SE", "e46b91e859af366ab7cfd7f602b52b6b228edf3efb517f39756b8c801da3e30edb700baa539dbc9da584ec6b249a71f3feb6fcdca9196d1484284cd75635bf28" },
                { "szl", "8bbc5f271bf7b7c7a4826d56cdc59122fe5d8b70f53971c77adb368a209de989e427bfadab99f0a4c106bb35a67517a723a1afa682d240e473174dfb2bb969ca" },
                { "ta", "c03d93e98b2e542dc075b6389d32a2229a16a990a432f6fddfb27d497287ceddb85fb702ff246b268fac004383fe0dba8ec0654a6ec6f5f8ec149030c8cf480a" },
                { "te", "d81c391b6cb2fd0384fe00b5de32943d6d664a142b9112e3cd719f3c251f901d4f232c7d3ca1531c0383b0bec216b092519b49d3d23f99671e9fcec104a8daa5" },
                { "tg", "0924b6f080f93b282ef77e345c2a44047469ba66ca63cc55252007d03ac33279ef5457d113c1fe235f6aa44dc950374d8ab3ca7e74ad5f35e7adfdcf2e7f11db" },
                { "th", "8a76d410e525263ff579701dbfb3dd80ef7244ddd242968c665e4d3292116c664a2969cb8fb7d7b52411d82196583805fafe5305c2e2fe7b82b68cfbd3c56b0e" },
                { "tl", "93735a6b75fcb89b9e0719e36614c07df46ce9ee16f11a8466266cd39fd4d40937d75b3d56aeb6269f5730b8147e0fb5a5630aee25e96849ef45c32d33aa5750" },
                { "tr", "e70bcb71747851df59f37400d8c2e4bfac789aa73735252c53e5119f6e73d1290d549bf82839639a75b3bad39f5cba1873ed593804eb90b4214ddf5b6f482fb4" },
                { "trs", "d67a09c249376e7a4ba7edcb55538e33599bced0ac074ef325d754ae3df736e67cef5c0d07056ece754ed753e46fd48478d9847ad3d21e3caba1ca9f319d134f" },
                { "uk", "dd05e6ddedc18a5d413afb2bce099a8f91da2d02a13d16d5d316dcdf6a5366237a7574f2bf034354578d3c85631f7fbb1f7e4c77accfe8fa0e0b6d865c470676" },
                { "ur", "b9991c2c057035bcf0ae6f0c5c69e7bc67cda1b2eadf9679885ac6e8e0a6daeb42ffc28e3f607c64d0e43c096e21707e6414e2003d66c20e528cf4668e75caca" },
                { "uz", "d060f0466c4714b34ae784ba5b116d6c27e8d0295483dc613af4ba9e0f4fd6a91a57bc4dc6f743e456fa6c6adcf591c948f387c8b845f6babbb563791ccafd80" },
                { "vi", "15de7c6b7d00f890d98e9e1a2ac7959f1887190818add45beac0f23ce6029147aadb2fe644ee79a12243d87be077b5f71a0d4027c2ab8f9fb95f811be2a6bf32" },
                { "xh", "2a7e780926bb98b7dcb203f02e6cd70e56aa841856a96d95610a5be184b5ab9b831e9d05010c5eec176c9adc829713912f11a232fba7476cf68d7d15b2b8e9f8" },
                { "zh-CN", "8fa5880b2e95d409f7dd84e35c4ad713548f8ddf4b1482d9c25e143f0ce5cb195ada23e5cea1cec63f410af65f454c0ca0bcbd118a774c7bd92ef5b681bb492c" },
                { "zh-TW", "6f534ab4709b8e8290409b6a1f18a0164ec46fca22f29da71f91698c0fef2f6ceaea5d04853779bc07c8e4c473f689d12b949789b429eb76d197fb12d1f2bc4d" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/116.0b2/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "3e79f0fee43cec5eed66937dce0b9ca838edafc06f93a46953122605728587258d21b3a37ee038079a8b601f002aded52f2e819acf79b98b8b460a9c534b5e62" },
                { "af", "b44597c184af44cb7e15c5f1e3c9069e2c8b794f3737de0c8f2b1d6a9991238230e5f4e5d5a69f65fd9c4d24caf786f8029f9884ea98df8506df899f8d16bc5f" },
                { "an", "390d475b947b59737afca1bba6cbb9709ded31a38b860d08f1cf00cd9b48a0b92d1e3234ed70139d9ef337745e2afc2fcd90663107a6df4b52431e1242e814f6" },
                { "ar", "13914e3d9987cfbd360b55b86845f183d22daf2f3171de9f6a6c4c7ed50cfa0c0de27d8e2fb8ac3035458b8b055021c76ddec9858666735dd88ec391a42f9c42" },
                { "ast", "31007c8a6c1d1869780008a8da497eeefbeb5199056dd49e6b8b7969391a0430dde9561dcc63884e28a0a591f987fc38aa0648c298e35272f01cdb63b4b6ef14" },
                { "az", "18afe50424a1c1f6696181ba36bd08c3f48bf4962233da75109fa108868171b9931d985f95123faa2ba7f27cdbb424d450cb5b4bae06e96cd3228d9faafb9f8d" },
                { "be", "c4367b51dd7dd163dba193e2e61ffd41bf0a74ea434f90f709958f2606251817becc4928cdd7c78138a68da743c72b28f48c82170daf14cde0763f10679ab29a" },
                { "bg", "b8f5c83ecee89d13d8e886d2f9c2900fda59cfcb7199cb72d7e25de39446d9dba7ae5fcf538bb2963044d9ffa6b6303f344e6df5cada637be54bd19ae67a1ebc" },
                { "bn", "0f6d7d7ef3a4497d3bade5c5c0b3d263968025cb0e72a6100515adba212d654c6b8f3f3cb53a9bc92afa4ca35635080db9d937763a99d5027b0f5645ba19c3e4" },
                { "br", "344dd4c13fd79cfcf0e04100ea781bc370b129ab366f3fbf703420632f03684e6d8aa5b972a0bdf667197c460b99e85c218fe5ee289f9d44d1f3e7a0261a28fa" },
                { "bs", "eb563a160bdea860d37592d3cb0ff5d34e90f4d1e615321ba1091bb11e9ca8ec6d42e5d51be022267e217ca09f4e9111e4fbc6952de3b4d4e92d3c1dec3eb850" },
                { "ca", "5fffe30f2a231969bcfa6e0cd3917a10f40862e936ac10e0fa1a4c6e3db7dc5671f1ae645b71c3e89cedbd8d7713488730cb4e09fe0c713c6c9284e8b3517511" },
                { "cak", "85af5e3e5e582f5aa92d40652fbfe5e11e83f9db4353bde74127f76840669d60106e14045e8dcbbdef8ad34cd49bfc6b0a0bb4133e51a69e15f455968b8436f5" },
                { "cs", "5f9da8c41ad76cff492e50de1a747a3bfd1b71f369518f6ee43aa913808a2c4dc395e0c07b6e00906e24731f6239d1dc844ce6e268275da6e086ec3697e910a9" },
                { "cy", "9dbc8c4a2618f175bf9e8351dda717efb5ab5a0843db7bb83ca291b06aa63a6e4d24a7584bf7cf2848705d78e0f1fc05d3bc6bbafc91bdfcb0b44a96a09bdb02" },
                { "da", "cdd3fe2830a9327c345624fcb03e7015bc6347890834c73a1aa1006c73a706219cb4a7a88141c476daa58cc441a8e12ee57d1cb41c9e521059efb84d99c401ed" },
                { "de", "960058ed722f0f3330c5c4c3523dc65c64d2779284544898bbd755aab77622674adbd8f5c61291a33568cb45a28e62852ea8818abadcf2d52fb0c144a7f18fcd" },
                { "dsb", "d99ef7c0c81b1cf072feeb98cab7eda1bcc18d38c65895e808dac40200b4b7697fcaa50076e1596b660e097df8c3e21e7df1216bf02baf6ba8a2e7bcacb3ef2f" },
                { "el", "49d3a921baaba053649f2bfff8aec3ca32ee58a510beed02d9386ae3594cc8aa76bd338ea49620d8cfdfabcb5b6d47f03428e30c2b9855501085139c9c176d71" },
                { "en-CA", "b75a3705c0292a0ddd1ae632609d6f992c13f5972009ac646a45afd19a961421a2a52d7f57e4779cd3e96de7a68d4efb048e66a223b062590d669871bf0f6c06" },
                { "en-GB", "0673d87f0e93468fe7610cf8ff27dfc656f8c79fdd9557ec262c0c15a52f022425d38579b1e1845282ad4df3a5d77609a405a4c20b2c37b9cf29c606b67d806b" },
                { "en-US", "96454724f2430d86fffe0557661e264cfe98e46525231a69c8c3db9dec6191d8b102cc8d046cc82b44d230feca55fba456ed4f8d59c6530233e50d3607af9d58" },
                { "eo", "25606a2898be605d9c16ac96477bc9280fbe71a5ece69b99ff60447c16ca9bfba490d1f6641312cab48ff646f2c994a9a4386f88741a12f85130e9c512d0d080" },
                { "es-AR", "439dfafc9e9746f185b9c4748e6a784e2608da983c62f75d100b57c1b64e6e8a0bf7cfb2a546d64247e119dc739eba92d6169dadba522e7ce439cf585f9aaefb" },
                { "es-CL", "e84828ef9c790fe3103bd51f61a7e4cfcce186d12f644c52db93128322cc79ac300a0a269cab7ef874bb0f5a97e0c73a32087bbe0d158f50d4b7028d8c42ae70" },
                { "es-ES", "cb4580583a8c4e7206b62d8f4c612a26019310a1113c4f8822cb30d30fd5261471aeaee7b62f2fc5c9dc88bc8d5810a2779275b812cdeffa299f597a03438de0" },
                { "es-MX", "da6ce7483a8fed5bc7f024d75c2862aae5c96c72b34b115c63d8b7401ba0c384f7a7a94a2565b62c8b96a92cfa0b1134a14804b80bf43f89cb8a2be6425674c3" },
                { "et", "d6533e838c9d6ae768668abf556072be2f5958b0a20964d2dc83fa24de9eedc9cc1e45e2ed4fcaa6c6186406c67fd435cf14251dc202dea7d8738910de69511f" },
                { "eu", "9da652685bf192e22d5fad67589bf43f96ab9183e58026594ec3d5d102858545eb4586b790215d94f3ee9219db56d70aaf916e4970c55a0949669d83fea518fb" },
                { "fa", "f073267252ee6697102bb6a5c7642db49e32e6dbeffc95e1b8d51ef7386c729e909923e4e00cbd24326273ea78430a40fcea0e22444f214674f17828f488f888" },
                { "ff", "d3e944d5f5bf0fd4fd7f31edd1ff8a3a0378e0bb13e13f3c3f3ab33ba35e0a4324a44fc858d1b0798a7a402811242b0d24673d0d62582eec1e2ed7a2d74d3ebb" },
                { "fi", "0f5eeb6d37c5db3a71011cb3cb89b0dee404616302952e030bc8ea94fff1ada85fa205ed2d57f0b2f0545f2c9612073d112da216a7a04f2f1b26b3619dd7279f" },
                { "fr", "2b85f3e881e986edda00b630608aeeda18477711ba0a919d6db175bea72ca280ef81d4c7b154d54392afb1fb27df7ed95d3721afb948a522c240edf41d79015c" },
                { "fur", "c46e2ea9ac70dbf12a01c388acbaa9cd44e2dfbc5fa855bcb0501cd8523e8f0cf1ef1c4981268e3fab6fedf60ed2004db4f6bc4d4a896e89e39a78548b05e30a" },
                { "fy-NL", "c207622bd358cc30b081349d174bfd1d59248ef554e93c5d2569f2f4619f17a1b096260e41d9a0953021c81679669701d17753e59d346e8800d80aa481bfc9d8" },
                { "ga-IE", "6abb055d3936a48ee458afd6747d798d1636c8ab77f1e68e1caa2c5d72ecc6780d410c67d1ef722bb4842cbd4802f678004957028caa69210434e184c4fe521a" },
                { "gd", "515ddd72a45ac2defe5e6c5c412cb0492b84cff80ae429a70d2f09b93bdae07d6ba131b6648dd6f57030320a2f323464f0d18965e401aeacd82161db67e7d111" },
                { "gl", "e18a62a032ca90326d7d13d6f3dc508f12396a880fbabcb0a702a94c4d87f27498673760cc546ef3dd73e50520facb4c08f28f3f2f0677429394711b370521e5" },
                { "gn", "6f6ed9abea9110fd7aa5974f0db9c4360c1e9a8002ff35bc9a27b2079211d1c11161cc46419b84a32321fb0945b534387fe1a731a024665a34065af2d927d15a" },
                { "gu-IN", "10fb9cfe01dd0fe71cc7baed7bd1efae5c315c31af76b01404f7ef7d40a6ccf74a021a6854643a89acbe8bdd573da0b9343846f673fffb8494b3b03cfe11ff2f" },
                { "he", "7a7003881e22df8c4ba68fa0749c806cfc86e27d8db65108970ee95d3601ffbb7347b3a3247e9fae6bf37646be9c144d9b18d3c6ccac605e657bafef907f2fb5" },
                { "hi-IN", "420c41a20c80f62e08e9d31de47b197f3bf27c551bd6309213671c2bf692ed54b2a145aa582f1b5d97873f02db45ded3c6b467bb7344b953d24794d9865de558" },
                { "hr", "0374cd21bb2ef77b98a941a65c8eb7fd4b594ed553cf2740e17cd7e496b64d18cd2d7e01cff94d95eeb84d3d689b432dc0674d6e4f93c009ae53ac9e14be695c" },
                { "hsb", "4f952e69b854a040af7269380bc955463cda4c9d6cbbd0984ff228d7de60abebd107004e869cfafae21b3d9c3eb2c1a4a2eb6afbeab79bacf4a433b05f56fe4b" },
                { "hu", "2e6dec9481eaad3137e600351383944a5f932a6150b0dd52b451209af1ed90a3b04041322d7b73c66ec865972f789dd76a4a5b9bff0c92f0d391ff947273ea13" },
                { "hy-AM", "abb64500cc1a008c8be495e1fae1761af75ecfb67ca3ae7df48d7119288919194250b4e06b005d0e1afadb5824c97c2b34081d7405d813b72446a36ebc60e1ee" },
                { "ia", "9f855bc9c13e4bafb916412cfe79d72025d84123a0e84b10716413b0756e3fe8a8b6b0c7e6cfccb07415efa11c039b6216070bfd63671df7edd23ce7e92164a9" },
                { "id", "5cf02a96dc9c2073859be3e54b0838765e6b6e09ec2165aaeb6eb97c9079af18ee3a45f61e7f0d311fe75b8119c6f949261c44d9f0b70ec7ba95155e2e73d2c5" },
                { "is", "6f4de448bb0c94d45100a6ef4f0c93be9b3883eaf2c1ab55eba6a55fe5b4c16c7fb30124d3d8bbed115453965e3a7ef9a30e0efa64fcadabec9be67c70955789" },
                { "it", "0556e4e1630cc476d296d6c17e3d5b29a41383b3f217ac829e721729356cb5cb28b2524399d70ec5a603b94e084b2f4baa3e1927f9debf44f628340461e31c05" },
                { "ja", "c04665fb2736465fdcd1b7c4c91fb434622341f1fefa89a41d6acb7c678fbdd42d2083ee964762d1dabba4be478342d76e9050760bebbcbdd9c69b79eb319b71" },
                { "ka", "d3eb9dc2141de8b662f1b7004ef6e4a703c15b81f54c0d6e6a130f9b1c9093dd387ab05261bfc9c416ec0a89759bab3602f6fc130eebb2b0bb82fc14b08b1a76" },
                { "kab", "522e4b873b4823ec4297318b8e5bd2362b56fe804d6959c46fefdab4d330054a8bbf019c065d8e701ee5a116c8d50e36838c77318c96f465692b53ec5d2c939d" },
                { "kk", "2547e70b670c53d7e74e27b701f0ad0051c07dbaf1ed124904b8bfe43c7d220fa561f40aecf17c409bb621172fb314fcc82fec9ec131ad11c14520efa80d9265" },
                { "km", "1d1b8b10f6ca1d096388b6927d569e9be6008f416ad2a53bae2d11268e850d193e1cb2263d4ff316e8793e004eb59fea0d46841602e96e26bac79f0dac979c0c" },
                { "kn", "45ac91b45970c9cf8bc38eb4c476bdfb521f8d5b53aad579c683717ab900e9b5087042c8de0ccaad24c7666b566aaf2e5082468202a96173d95c28438d895f3b" },
                { "ko", "3c481eb9c3f1f62bb0112fa92b5b882584e11ce810eac15da9501b69e074b6a559e821f68efe4c142476fdf360144d62a4b15784a27f1da656384194a32d79e8" },
                { "lij", "478a3b6c3c8636cc39c4c6dc8b65b7c20507c9008108d4df70c73701b10b265752f8c776f5ceab63e7c9d24eeea2e6bcf200b8b31f39dd5c103c630c65633f2f" },
                { "lt", "536ee5497e7db21ec55e10645e076d1b0fbeb2c4be198212716173cbc27b7c0338f29600833c70c1ab140047d791394651f48fafb9b4aa5e4e8e37807cadb225" },
                { "lv", "05ec2aeed16661273c6d06493681327952515c77dabd7f0f57be81559057ca953368d3f7d1f41544ad472b0199f782fdb219eb151fdc6129259dd35fd80ac23c" },
                { "mk", "e51db4c90742d21fb07ccd05caeade19ede147fe4946d90d8011e65db7f7116298ffb893d4eb0c3745b9c5b96025de6964652784f4ce7201ee57d11e9bd43982" },
                { "mr", "57e5b78c09f309491458b6c8340c34ada751355aec3267bd9744bae5e3e3da59e7a6c6caf99eb1f137e02454e6f226653da35d8b3b9ecc7006ef941b8f8c428d" },
                { "ms", "69f7b53daed8fbbe7ecb6fa907d5bd56e6499a2dd790cd511a73cb9e86c5cda419fa52faf6824aa22359f59c66aedae08250717dc000bab800492d0a6b2a91a4" },
                { "my", "d029a399d7e8799204fd5367af2dd498ca3d4f4346d172390ef5011c15c1a40d6f5efe6aa89aef034b61b6c66fc0918b7b209e29ad710876f893afa07ec95f95" },
                { "nb-NO", "56925aa78beccf2aad80241c5458dd228f8d110b7ffb7ed7ea8d206aed679a40e1fe7e2673151fcb5a7577c499f42f24143c9143e3595474f933d2d317b46389" },
                { "ne-NP", "e41f08910461bd0e134b5a500f18e4f489bf6bcf1fd08b4992997fae7d5e387a9cfc32cfd9d4d57288d41f91228570bd54af747680fd5e33731b842bc61d5c13" },
                { "nl", "850b207f9e08bad0c953af9aec7360345f7d4da638e2f1bbda04f3559904ddc87f8a50806b74318ef661d54020e4e3b2e1d6b5444f9a95dd0bb77d1be00bc4e2" },
                { "nn-NO", "5a0ddba1cc91492f9eb07e87a8ffc114be55463cf927c70e911965cb870427e426cf4241b57bfd2f89e3ba4f7ae4fb0b9a9f7ec53e73e95994edab6fa24dfe71" },
                { "oc", "f6e5c77a5e09ab11aa5e32baee1ac6fa685f3df05657b4651ce0eef1b3fd3c929fd5e1dad9e1643bec4dc011748b631c66340bf1f78a4dd290e933ffdbdcc7c1" },
                { "pa-IN", "2baa7adf2f1ea094f9d3f9b68ac4092f1a086f0da75fc6f1dcc7760d9a2b2789f05cd2d0e3269ae4b5e98e8f9bbc8204e801409ee2559b73c8bb11ec13703ac3" },
                { "pl", "8436a283ca6d4a523fff4e18fb541ca775c114f809ed491fc17c062e075f2cf1b1146adca83c7eadc314991f73da936cdc134ac7bbc33b7715e1bb8ac333c3ce" },
                { "pt-BR", "f2f5906ff8bbfe9c5799650392a8991282c2b3d3b19a28de7ff4e4c8cded6cd2c0d814e789ec0e195e0df12f525fb77a1db30a8b5055f2573d7c8a11de0387f2" },
                { "pt-PT", "b182a5113d85bc752be7c900a624ced380f78fece5a09c1c506be245f1dd3967aa93a74217121854128f96fffaa5d405b6b0a9b1b60f01a1cd331ae181348aba" },
                { "rm", "8406b47bba6f0568fdd9c6fd5145ad91aaea36447437abb394570f488d2a7f2c1014097f1e24f751a84139bb55f7f60368c604acc5669b1797d0f830596e1bbc" },
                { "ro", "05f5508ee1a14e37f7fddf68fd5ae0473137109503d4db6060e197e2d71dbad84ca9b3fb3c560093e00169f1f89674240f22eade2604c5d0b5945dff54034802" },
                { "ru", "ae2140f8da6626cd5eb3c8889bd431e427cdedf10e242411bbe87b8ae2b6a1f3cfdc2e3aed1a53aa4fa8ac7f7f20fd781078fc40ea7733ac3821594c63141220" },
                { "sc", "91470d0d9fb7097b30a8276b104d43f0435358964426b9feeaea635c41f2e1ee08027f67dade1754a68986a9c04e388fde92bf8998111333cf04c389e7a114ab" },
                { "sco", "a58bf22cc110d23ca456da7254a3f57765099cfdaa493539310f0e7ea4748c3bde07e46dfdf9261d11a2f895b2d0697ed883cb85febcf75af8043e73a5a5c68d" },
                { "si", "ea994ec1e1094c4e8c9a1adaa0f6591045dce239b55b1d25983b1d05e40bdf6fc3cea3c3afd6e09dff8d83f54c1b05c1e6babc8d07a55a95fb98a60958a10e4e" },
                { "sk", "bc20a781dbd8b82f23326f65d45370f3d9edc71908f4d1113219bbd01454beb73d0fca9d0e9a90a0b774e8209dbb75705d64fd1f469ca8e3fb4a790299d7d45c" },
                { "sl", "b0507a5b7d1d105d2267eba161fa0176201924e6b869a1ffc4c90bed7451558f173fc06bc4f2b8f737f995a826bf5170f41e640214f8d9645d1766f1bb315464" },
                { "son", "9d838423af55fc8ed73510f7b3d930481fd3ae642dabebbd2c83bdb87faa6d24f07cdf3abc45ed3f5b270c8ea09273988574afa493c588505ca563e2379ef874" },
                { "sq", "76d1b00d940858a621e1944cce21dd41b84ce8f7551c46896130a23ef19a264b6edfd49a69ceb502f6f793d198b6659219d5ac76e80ce543e74ae97a01744bf0" },
                { "sr", "4056af03bf79df64775d0a0237a6dc8dafbf9fa500c4b05dd6356427dfb5a0b44ed3b0d48da47f29a9808d8573499b70d53c63b4aa9bac0b36f7fc5866dfd9ed" },
                { "sv-SE", "1fe4c39e1e52daf72caf35a2a163ce0c37cb5f6d84e38ff52b6557c597022ebff64c9322a478b241452f335d9f317415cc8a53509f2dd336cf0e03a9553d0279" },
                { "szl", "dcbb4bb2652cef0efcd348d6dee4700670ad242ad414364630722e511a751226c112055ddaf8194ecbfd3b3b44b257abee0d6b43945f2a207bb64a2d8f0792fd" },
                { "ta", "56a835fdf8d2bb88b735d067decb2496556653fd11855a5beb50d9ed3218d701e140ccec169386339934406fdb5fbc026f101c29dbf8d123e73171125410c1d5" },
                { "te", "7ab16ab361a31eeab24255a6e9f327c59b14af16f0ca8e004ff87c897e87fe3a4bcf533762a5604c44b6c15d773d08dace813414a6f2de9fd10d8d796f30bde8" },
                { "tg", "d6a6951be39f5320f50cad777e1d85ba240c1493fdd97db62d50c2b878b6603838664ba33656f8a706f6641aec3e08e1510539af749b2ed6c89b159e5a8b3f39" },
                { "th", "34c22a7a9da265848557bc92ba82fcf95c4c888e021157e67d8a4f66b72910c26e42ddb38b10a41974a475158448ad77bad33129eade2cf6326d2b5aeb4ac202" },
                { "tl", "99ceee010596f9c0386375f583bf00c48718c22f3202566fecdbeb2a3d4eb27f203cb72511430927bcf18942ef8fe9224e5cebd691d0f7ea8a12804d083bba85" },
                { "tr", "b4acde7f2bf3a0b09ab091aa73ae73b428eb6592b8d2491911d151c3655bc7436c155a501a3e326a1da3973aadda1b88a9e520a03a7945c1cdcfaf6c5344b817" },
                { "trs", "b19f3204fb712fd9cee2c7c8a44eebf1cc1ca053c4cf9d058fa070993cf972d82704eb5046ba58562d105914e4e12347c665007c8f43f4ab1ca9f06a6fe816d4" },
                { "uk", "faa31e58693710c9494671cd7cb9320659953cd933f070d14f93116b8848b3a778bf075d1bd6ab61c769a6c9d1bfff2cd34f21fe4dde552682bf6752e2791ea8" },
                { "ur", "bde7d1a0c4770af6441f37d7262117040cb1233d0f6d852fe4c21c2e8a31f2f40122282fccf07cbe47f3bb85fa6095c5fc273f80ae8628a769931d595d66f450" },
                { "uz", "ab076c40475517179dc4504a9c92c238f325fda9a9115874940278d5fe66dc5512755900db053f019f701ac89f548a6cb99746bd2b2d37724aeb8b117d4820e9" },
                { "vi", "83978fb54bb228ed4934f700142fdf2f510de6de9039b2492478ea6cef0a8ad6e27e7f8c4ce3035b62b8ce6f7eafc86fba527456781db19e5749d8d4c622c57b" },
                { "xh", "6ced29f08df1c8446c5c6767092e89ea795ab42ee7f26604cf5065856bf8c47abc7fdd4380fdcd969611c98c3b1561ca3e05e50d7ca64fbfbfdeca3039332bb8" },
                { "zh-CN", "e0c53a7f30eacff6388c9e51b5306c3c20493d7bb2fe6ef7e6acff9af4802d05ace68c6783b403de4a95ecddf535e59dd671e631163c2fef1b106deeaab980cf" },
                { "zh-TW", "c841ba9d5bc9f15f20ec4c9fad11d812c2dcda554329b49c96444a939ba9e71379f4b35b92824d61dc485b6872548bb8ac402748fc60cf25b3369a38b7d72a44" }
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
