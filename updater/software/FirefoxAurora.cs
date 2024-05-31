﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2019, 2020, 2021, 2022, 2023, 2024  Dirk Stolle

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
        /// publisher name for signed executables of Firefox Aurora
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2024, 6, 19, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// the currently known newest version
        /// </summary>
        private const string currentVersion = "127.0b9";

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
            // https://ftp.mozilla.org/pub/devedition/releases/127.0b9/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "b24d2f79abef422ef0945421fff8cf0de977ac77327bc9852e5e5af66627d96ffa19582de762723c89bee53554245c67daf0f10ae2f30d9caa2b60b305b025f3" },
                { "af", "69381bb225f00c7b7aab7f1a32e95b080c7c642abff460a269801f92ad852c19c14d3d491835de3c296a431fee9239261dffe9259b70bdffa615d5678f62038a" },
                { "an", "56d9fadd081059961abbc358ee0505eaa9a5cfccc60d2df16d9f6af3f5109e3d1bce21b4c4a1fd6c5e6755c9a3dcd353ba1da21452f3cd6e1794182a28e3de7d" },
                { "ar", "87843e2111ff08e119c6f64a204c2e1c838b125b34684eeebe601090534f39f803d6732fe9c7050914987b39370e1db5a80ff4b5d3d698cc23e26ae3d1842a1f" },
                { "ast", "56337b38bce11c45be31f3b55a10d0362d033910d9cb58a6df193d3be66d082c58d1818460dc91eeb8e818bd0a7adcec4f73abd0c8bb9bb7e56d170eb2d28b60" },
                { "az", "c63d059e07bdf72ac62dfc9a12974a08ff1cecc312bbbc5fe0a1a0b9b7be5a77692a6d3f006fa007dc1d4f598547433fd008828fb8a04829ab0f4f28967761c8" },
                { "be", "897e4a993eaeb6fb49d0e096f54cab07ede49d99f0fa3e69480fea64f3c3918273c6d99e10191513c4bc311346b060ef12c8b99cd840e965c10765c1ab4f1596" },
                { "bg", "e8d5a2ce92622996c00b7edf78d64ee78c0e22f0ab51db773f99aca82b2ace6b8d809209161e5c14903aceb424a1f967cf36676c1ea06c16e7aab82ea32b4db5" },
                { "bn", "c1719b9d48c75881762a67b87ed28d4ae0219bd2817665899243c8caa4bb39cec31cdd707019c7f469d06570f256d59da826ea04f2f8552359adc1a6ecf8f7e4" },
                { "br", "4f7b0a4962151b976bd03b5df418f384ab1149cae4f135654b380cf26d08fa17a7a3c6973854372f9c4fee569794042764bcc825592b7d9c6cb104df285aa893" },
                { "bs", "1a6a04723af3a4200070b9c22623358e85d2188afc0da0cb99742aacb7e0678d176cbd109597c24044e371eb2eae8149b42c3d326dd804459189fdb25f5720d4" },
                { "ca", "a6fea6e4a3972819fc09d2efb361156633ae005fe604686234fe44de5e88c0dab5058974788293513e78a1880d2e70800ee51ff17252e489f740587c3f660769" },
                { "cak", "f198e3c22b7cafb50243173eabc5983e188ee083cd2ca9516ac84ba0fcbfdcddd91e3113650b401a65de23e9c1e13bd2c117d453ea139e9d34231dcc75d971d4" },
                { "cs", "b79fe35f23d66eaf77723893e71b36f4ba4c7228d608aab65057e5d9154c217308e6fe85ca5b58825da185aee6f80004d4d06c3c9995e025c5af5c1bbf106f8a" },
                { "cy", "b368f4c6366829d93ee746723146fa693f38f7311b70885993c1e792eb1c9414ce7c0f81b77ed1a6b7dd25d843442f0d1fef91b21c0bd0a6b1dfd4ae8cd69243" },
                { "da", "f06e1f8e0c6427ed10286d7a09780b69803a002e4bbf16dd9c11ce2d78a3e0684fdeadc48c19f14c79ec1af75e18dee8ceedb16fa7c18de9965b1276c3ed4563" },
                { "de", "c8197ee2a9ce18e4a5df4afcf6326bd0f64fd20a688e3063973ac2a9f525b4ff058eab625d06fa57e86b3e5db6ed8f41fd9848d37bf55581e7074bac43a63901" },
                { "dsb", "43995479d24dd35d22c465bd87b97c60385d2531726901ce10f947d0ebbc15d2b6be03597aa6b92f5c66571be891af6c94a7bff34ca4d3a7824683430fa00172" },
                { "el", "588e2ddf8dedea67203f2bb56f5bafd14b053a89ca8bb41fbbb4024673874098ae6a97a2b2dcd5e399b4dadec301dac73a80c972127ce57a43c6d03c4fce1484" },
                { "en-CA", "8c2b9159645f9831a2f035beac0608312afa2587483ab1c9267b56143d478dfd75ddb3fc38252565369cb209e06a53eea7b0acc849621074c9df143b57fc5818" },
                { "en-GB", "6de13c1c84ba3d666723fae40457928179ec78a482b73132be2542d00e3e08c6a2a35c6a646adc2d77a6b16eb0ee4bf5689e788dd058db64b08dc09e8e671051" },
                { "en-US", "2a04b0aa403d68f86df68d2baf16253baa0ecfd38aab99edd94a3de51a1d4504911df07f742e35d8f23ba695bd1aebd06a8a03a61a1d8960ab1ef76132efb491" },
                { "eo", "72e5093e5a62d90db0968a01eab3bab735a4cbc90afac0a038cb98e37d4df4192d71c95d63d93833520b371b656ab4b6cd28b20fa12203b225fbce6b4d5de030" },
                { "es-AR", "ce846ac2796453512648513a1052be60ca047b7efd047103dae31c2e3e2c193185fe56b497f4e236cb8594446750b4d1f9e3919b02baf9de9b4b3ebca8ab6efa" },
                { "es-CL", "6027d0f4f49a31ee198097708e31a1f337df430577efbae72e80f564475b5b53b3cba479ee2c49b2153e8fef28be4b270759cc251c24bfa9a20537d004b0415b" },
                { "es-ES", "a3c76a9df49dc8bb6f84374f0d96ae9cf3b323cb4a5772e6d90b61cfda51f6bd8cad5da0bc61ec99f8fa3ec2c9c16ee07c1c7f4ba4b36227011d1711164a0781" },
                { "es-MX", "f9b2a497a767b63f2f1f8e623297bd0fe505e6537ef5688aafde299f18e770947593e4536709c8a97bf589f54056dd3e837ad621c86c7ae78b4fb721c9472afd" },
                { "et", "f2fe87ee81fb646010a62ffad71885e6fe1cfe157b34e4e3f68fef3155fe6d7ec13ea48623597315e66bf73188665a2b279d5559f66847bc7f7aaf71b1e9afef" },
                { "eu", "d0da6a7e4b473d585368b71e4a41afc88e23d5ddf122b6fcc3f388077e73594286952c58c0879ff965aee0c969aa8bf6ad96ce269bef7d529bc0abdffe278704" },
                { "fa", "0c30595686dfa2a60a4388813e634a4a437dc4d010fd91b16d62463e4dc69f30558956c9798c4ff42bab218c2d499841cbb72397aeee8c70f368af609bb7c163" },
                { "ff", "dd5ca6eceedb321500aa2da3d0e082949e942d65714c8e6ea968bd7f22ef28560e987da7608f70f3233a036070f7a62e25ca3bc0bf6ac99ad28515bd4369baa2" },
                { "fi", "2fc8caf98097bb413a221d3790418ab3cde6e0a2d9f148395ff910622832ab975d0eaff7c63323a4cc06a63fa54e014af2016d37f99a502085aa1f645903cf3f" },
                { "fr", "6cd0947df578932ae870bdd57e331426517154587c20b1544d2f2d401998f0473802079fd22e8948f2fddec435cb430ae115352827a514de48b1478546022fe6" },
                { "fur", "665ea9e8eebdc928593d2fbbc910329f0d9df21e4f04b23d1f3386665b9bac2eef22f74bec8b84064d1ae26c5ec586d6e95698eaec7b39046bcdb548a57da951" },
                { "fy-NL", "1f7c50896892ccd103109ee534801bad85fb119b3fd29b631bc66f4cae063b114cf60bad771881d3f0411cfc4ad159d37c836b56123dd10e4b74807b338097e1" },
                { "ga-IE", "aac8f0aaf92f264251a46dd805e1960cfc1bebc17fce55408eb3f77a0b6624f175bef8e74c10896da03f502a65ec857501d6a1a4484c3a92fd57abc1870a55dd" },
                { "gd", "d5232e107589827e4b00d854eae83f311cd29c801f2c25000faa1f1103bdb6bc3c5d4e66feb1aa3f3130a04450430fe08f0b30f9a8edaeef6c5652f2392968af" },
                { "gl", "958950b62d206384eccca706995ed3f61d7a398f5a4817c34fab4513c61f38d8c9ac683296a06b93b207c9cc462e360ba5ebc1ee5643cfdc4a6e22c5a82ff8cc" },
                { "gn", "9fd192f5b1170a25cff54d57128fbecdaaf9890d16b7435e03d669059ec67f42b7fa541247a3a2342de4c8580eac7e110485d37564e786426b82172ae2a7e5af" },
                { "gu-IN", "822467ef53d44f199a6830513b626dff0daf2c0c7d01dc95e5b8e6342e35e398bfb22c3ff459bd06c7052470f3cfd2a7b678db1159e487de97be3b5663566f8e" },
                { "he", "bca2e03dd0b6fa8960c32aceb9ff7baa1084ce2c340ff885655de18040024fc4c0d19c10d500f66da2201bf7bde509e1a3adf1b327e409ccb3beb2198627bc72" },
                { "hi-IN", "dcee1832b30a3f19c66adcec25a763fd0aa1e0ad27f87ada3e854f2b7044bbf8f260893006438a6f14ac54bf65e17500ff64e2e8715505f2dd1320f17fff37ef" },
                { "hr", "e41400d97e376555c4381b64026f24e7743d1dbfb4944cf611e1bec962c80d8ca4b55d08857c45d1148981721477f2ab95641d69d328cc761c8c2c92604cddfa" },
                { "hsb", "925d8f72184dd7cb065951632db0abf404ac91ae2f3a9315c2b8845997ccba7e9170e69283683e6ff2039caf6eda457ba89c2f8ed4a2a6bca2070e99537cb277" },
                { "hu", "5bdba1ff2ea8b44c6fcd877b03a43ab6c907c9f2a71772188bc16ecae0bffa6a843d0360df1239c1517173c6f98d80cc83420f458ba7abc790da15475054d809" },
                { "hy-AM", "d1b978bb2bd0ca741df8e6a8870936962d5114b2bcf1cf97f68cc4ca617ef1d7bb3d31b4e58919ded3806285ac400b494cb1a5ac03ab07b410b6a1152bf7fec1" },
                { "ia", "60814103862ba42cf4ec1c55340b8bfc4d7bfdbe27d0169175d7a41bd8d5e1642aa79591ab9c7d1510e7382bde7d65e6274996692dad23a4e0baaaec32c3171a" },
                { "id", "86f0fe1e935d762474732c4abdf255883f3c5157479c5788c996a62ac28f13af0f35c45b39c6e6ae582a60365395dbe41611af8ac6ca7497d39edf1e6b8d0440" },
                { "is", "a1cd9473fbc76a059a408f8b73de22d464310f36823a8c1dd54d4d02508e112c6a6a3fe0a83fea4547b71f33743a0b20ce3b06b57829c536cd729d47545a100c" },
                { "it", "4b48ed0e21e1b9d4256742bcaba9749589cd730ea2b8d456a6c31d8decccf852822a9c52c497bb19598e1027ddddcd0cbf871cf9aadc0db2c61944fcc58b9cf1" },
                { "ja", "2cbd9fb6f71192a2a082ee07acf8bf68fcfb52e23cd9567323424f78aba9d8bc4384781af43b0ae18ed99e04276698b736ca4a3e71bb84ec19a675a182ddd915" },
                { "ka", "f1bfef79c29316dcc6f08800fb371d45d028ce780dfa5e22a845e254876421e1cbc72e81d62801ffe4b4d64ce64fb0905a6722afe9e58e9a2538dc0f895362df" },
                { "kab", "603df1501f02a77c22c252b8a383e1eda9af1d44a4dd5e15359d9942fa1bda5fbbd671100f0778cbe1c0f2457c48f0ff59b3e150e2669e6468b20657e1cd577b" },
                { "kk", "4407c03c77fcb2d2564df9ff92c1619561af32dd3bc340edefb2fbac78ec22675d4f655d5839dd5dca6a2bfa66391ad91b5475f0118fb342cf89e1cc2115515a" },
                { "km", "8d03add8ee512d6a9f7faeae3e44ac50cb7e241d0604f942e0c9afa9182c934364ec6fa38b8124317e4b49ae7084134637b2973fa8970b533206fc9d8170fe1b" },
                { "kn", "f8cbf03075cbda968cd31b3663996ce7d2156b1589b740b222455c8adfa0012cb655122903e6170dbbb9cee5a7a26df55692b79e1346a4b809bf622729e2501a" },
                { "ko", "04448d88772babbc2ca7bdf8ada307b1d486c62e4304a71fd1c563e1230f8c5caf5846e66296567413a7d8217ae06334b3e1fea2b82f0bdc7a3064acc45313b5" },
                { "lij", "37f379d60ec7c9632b20e176a64d01a6d57c642a986940bd46252cc65e38f267e629956bca6a7af93e4aefe252f43569ec9b01a0441064dfb1d8b7d599b321fa" },
                { "lt", "c77fba17684f22bda9b88cc508a6775ea178cf0ee9fb4885f59e950c03ad86d5c48bef836df0a1de247b0c42e03f199b55c6be392a00b952ff7e6a12b477c7cb" },
                { "lv", "3566bc4aa21ede5338266cece68115022fd4a31f46421f15a4aaa431a883330f88fdbdbdd89bbcd748483e21ab4e3de51a3befe27ffe5e080f58d957fdc0b441" },
                { "mk", "cee68e26d251f18c5e99e4ed5180a299fba95091b302fd6f59b4ee0ce1d64e57d2daed80dcd9878400f9342645ccc19c8f4f44b9f8454e5786d7d4f175bc57a5" },
                { "mr", "10ed0b813692770a94a0d5c14fccaad015d2589e326d2e5eac759c2f0b23b84143818526273959a4859aae896254315291c5777f5b48a73fecaf7833c0f1e562" },
                { "ms", "c6a41f32eff5fe4cab62dbb1ccbdee054627c7487851714f806ddca9362918750cca9a6fdfb260f19e16024fedd964b78d4edf31ee9f87e87cbeb3e1d2a4bfbe" },
                { "my", "cd2806737c8755db76ef42fa41170c2d05805cd953a8e92e9d650f890532a52e2e27accbafeaa12076b37d76bca27fc9965aa520dd02f9d9a4c5da75a361f976" },
                { "nb-NO", "3827e895df35b8e4b2156766edc43db91ba79d0536b76e28368732dd51b8084a6df9e0636b5f3bada890dc226807d59233129112f41985132465a0f4839d4a3e" },
                { "ne-NP", "36b0f7118f9d516cd9780288fd9ce40f58d3e4c82efe52251f045780fef6cfd9fcb0f26049011f57cf4078b96c584a8905dabaf151abbaa965a350af7fe39808" },
                { "nl", "423c15c0f33d786269057a93f80758affc0783dd2ecd0b7725a28bfdc529980b99d68c5dbf242d31b2065601ce622ffec7e237abaa0a985bd48f8a114ab74530" },
                { "nn-NO", "b073566ccd2e746eb8325019315323df88f3088270bbcc3105fa1ca639752b7aed4d53e53984b3546ef24e21c4e04b2214e4e95c392151a507916d6435b6b73d" },
                { "oc", "78ec3ff28f3f2e5022b542bc768d48614ce495103513c7523021d165eab2c0ea253c5705b5d18671028264f16ba7ba79f442a98ccec4c936342a5c122bacca78" },
                { "pa-IN", "1a4edfe18703436251d77c6dc1bed5a69c3deef751c350e0f2d0f3cb67ef23c6f915699f21aa736429257d9077a60fadb6070a87dc8b919c11a1a2bf6b4027fb" },
                { "pl", "8e63ba4635bb0a59f7994b92168a4d750c01033599211213b1600fc32c4d039cc6ee681d253d9505a60c5d255d7383ce4ae1011a69ed7f922dc97294f81d5457" },
                { "pt-BR", "38698c9c5a5fdf4d2b1ff590370a4ce4b5edc78bd475c8f6c7c4b31bb2bcf48cad2e882b8490aff93c43f6940b47ba4c55ad13687a58b9012f07006e629bf921" },
                { "pt-PT", "6c6088a94a95a1b9f62daaaa658c1d49b0ce8e6657831af9196ae2de595df0008696b737c806c7a13f3afac4cd0d0617c5a2c3dbe96ccae07462b680a8a77402" },
                { "rm", "2b6d778b23f26794f086e13a5a2b51eff0f441b097a2499b82a14d48ccd26b06a60dfb1ceeb3a6cf7915da164f41ad3c232c4825db680bfa9f499262c07bc6f2" },
                { "ro", "a3bc6c752a81e78c4c6c7f7b94f1c8f91ea91b7b4b4b3a37a814a352fa04a5b44ce0c6b9fe442530b5b9f96d26e8055ee7889247845bfe753f1b5fa37808b593" },
                { "ru", "87c743518a70464315e61b46a05818460ead87d44af5be72a11d755fde1a29b1e50f6c74f1d5a7694f2000bf8c306ca9d9932a95b5a4f1c56b0d8bb1f26f37ca" },
                { "sat", "3954f35b3bf0a43ad685472d9ee913ae14598e51e43823ed783c67f1e940100cd5dc53d63f51dcaedcf7ec65589222ff3d992c02ff9511c4082b70035d4c3476" },
                { "sc", "513a5ffaa0ccfd03e0504773574d5d85a19a676706731a22852c2cb0575bae68b52f0d82b5e6d6179d43052939b80f5daef527a3f4cf218523d495c891475b54" },
                { "sco", "3207c547adb5ed8cecfb69cbe2b73685ced3ae3ff7b1ca374593473972f81677d3fa155ea9483bffae12ad9ba6316f579e673a8911202664b20ab89403fdd670" },
                { "si", "7ccb68aeee9994b8ce1bd37b9aabf1e953f7ece2cada8dc21cea4e0f50853866cf3fc2fae28d33d84da693a591981df947879c0c3dff5bb8ffd11fff9eb909c7" },
                { "sk", "dd87966a10393a46724be8beaf70d06938093d1ffaa482b818470a012001a7c998e7b25af2df244294e58bc7d767717585638a259a46027391515c5ef90a517a" },
                { "sl", "622e451782dcfa7ab661dccbe836fe3a4f83c10c43de929e1be0f97c4a7885b3919190f781ff9ddf80fc63274d668d4371cd9b7da995435672d85419ef621e2f" },
                { "son", "e47f51317b3b54ce09608cddc0b95e957e8dacd680cb5d845333dbab71cbbc19e318af17d55936bb531266785c5bb6c3198c5ca3c887aa557da0abf195391905" },
                { "sq", "db9f7c0e48917f6afa43e400a0c284eb0155bffaa8babf4ffe21117d6919ab3d246c2dda54dea66f466cb2860135b1a0c939818310ba06869b4aa5f9b866853b" },
                { "sr", "e6d14e2b4b4656dd0f6cd718d40231e5f9e9eb6b9342a8fbc049bdafd1659e0a5d64e788e15752a41451c2a24ac2f47a7041a35c37ed1006ac9a1c16d20048d2" },
                { "sv-SE", "c1856e618d4f8872735d2ee6f9b335faa9b3ec34c6fcb789651aa433386e4cc134a1671826bc179e43a188db3f6362eeab981d923bb7ac36ac9e7f9c861fc737" },
                { "szl", "43f9543d9ab478bdee7c1996caa51a069b138eac31ec26ad02fe904c72d9fe73362994bb50aceac092f0d2edc3c9cf5f71a3967ef12403000cce3b02e9a9d892" },
                { "ta", "488e599c3d7a77eece7536f573edd7eca09dd8c58a7e3f860a3019a39752305f9fcff8aa723b454ead02b0f9f17bd95861d47617c91bd43ccc5a28a5f5a0de82" },
                { "te", "508bec014135a2474603facc13dbf0173d232cbabf05f7a77d516763ca4732d7ca88f4bdada82f08efee924899f8611038da6500cc176edc911f6361fe16fa1f" },
                { "tg", "fd6aa2bf25030f2721a113b714b2b0263ecc3611e9826b0997a223c31b62f0c442bf3e1d3d8319b9d82506670993506e14a36e970397d391cef39012e9757a45" },
                { "th", "f9e55a579c3797bd7ef2f5a112bb66fd7a52b974f05238a87c70aab71939046583b8a1469c5b3274c25baa932af9f3ae52a98276b684adf0b4c8b649109476e2" },
                { "tl", "a219f48ba14da5b8f65ca281f9773a0d5f29e549cebbf67ab340566b2c5cea87128b4212d1f8ee9f2c9142b1e2d48d9fe0369db88f59ff65af20c06fcf78f9aa" },
                { "tr", "a594333f99131c7d8608b01eb8424e6dd5f79b14ad2760558535a79dc8e9e567d1e67ddc0d29d85bb4b0995b22bc999633b3141d7d95132c5794b45a2c1fa148" },
                { "trs", "253df7ce810f1def22eee55003d4f68d917da2667fbd66221f7b50e9c5860b975948f51af4c5efd60bcf60191e6cbc45303babdac20061ad98740ebc1b1f6fbb" },
                { "uk", "823a983bb5b0dab4927ca64cb098a5e49abce66e2ace0494e014c72085543052c40f10446f57280aba61866ac2c8041266f6f5bce5418aef3edb8490a83398da" },
                { "ur", "6fdcb60e9193ab732d2fa0aeda9c9db171aa37b66f6e4020bc0cdfef2d2779393bb49684284ed8c3a33745ebe6df5a1354538a5f1045bf041ba652518117846b" },
                { "uz", "2bde87d43864a9cd5e5c31271c584c4dbe1aaa8ae811f53ac9d1a8551e117a608958dfaaf604feb59d4580273b07c65b3279393401cebc3eff218574b89c822c" },
                { "vi", "ec2e6a2bbd085e2ab304baab4662b437096993c4813ae77fa9ec831cfe8a612ccc557f90b6f47f4af0c3794273df829acb7278343f00c8b10a29ea092b2c5a7d" },
                { "xh", "f511d03bd1600126fd9a290479e802231bdc126b2439d12dcc1911e48188df7b124f3bfd10bffa4627ba768579f52df492cc8cd2d1e441c834801c1603bcf8ab" },
                { "zh-CN", "a022cedb6acb96557765ed780b7f1fc4de16536ec68d57e60ae5b0686f24714d474564f495f034d1c6d9e9a30c8360d7798eb6fbf32a533d224f537a15fa5bf1" },
                { "zh-TW", "950ef15abaf8396da14451befdc89571465a6d8c6464f7a07b48f7ce317db6ad9e4cc29503dcac35ce737fc6ccad5fd8cbc48129835e1f5d0cd4c22e932082b5" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/127.0b9/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "723f4b71b4a3424e9508d79b19c3afd5377282aabfd0fd10fc98c91f4db0d09e86c0c98a7aa0f09ef7c3f96cd245b58f8d938886f887cb558acab3e4dcc60397" },
                { "af", "da2a773159e457cba42cfefd10d587a99d0463178833342102dde584e8839341ff32787922916e312a5870a556cda3094f1909f40521ccd662123b6e62638f49" },
                { "an", "cad0225ba9ad3bcf3a6ec92136c4e5fa0136f82cb06d7931adebf7ee9489c90b9495848f2f2f008e4235798e8193aa27f11def3c09caa2cd9a8ac8a71fb6c51f" },
                { "ar", "7558fa6134823823b0e90ffc1584b80a10e29e0fed92ff6db98c86192de3b16c0841fe36300696a10175c478c3735b28d5a9f46d33137c681e72e99a9af29fb5" },
                { "ast", "5ee9a2908255ad9e8faa3bc66903dd83cc66f23e45559707816b4b139db61c72b277c2548a52d7161c3f93b637caea4bdd4e390acb395349d8ef2f5cb5b6a99b" },
                { "az", "9178a638bc884c3fb77d4a9ea33604daeb20877b4eab789957ff92fe9dd079c08fa18f2ba422b304f848d94390f692d6e355219c5cbe3e7c7ce46d84d38a45bc" },
                { "be", "16dad4dc1a93361a4a3a3bc258ae83dbcd784509304cdb2ed3e354aa56ca6e8f76654f42982eb705f01f5838c1d475d72fabb9a0f5b703bd04ed968c62b3c333" },
                { "bg", "0ede3beceeeab98b24bb8fe47187f3c6671918c546ad0ba3d987f9c6b2962c4c9ae5724f86f65ba87ca87a6e590730b44e8cbb54398cc24cf410bad13d75aa60" },
                { "bn", "5ec14b7540fdb1a49b19dda8df29ec4486cada25c1e029d6df85ac2f6a1c2ecf7c1c75675c69718e3e0c94ea7664a4fa663b149d4fbe8fadbf9b7bb8927eeff3" },
                { "br", "c0ef88ea5da7e46c5f87783da304fdfa75fc8283cd0079802509e7d6ad47e6251997e5b83dadbd6a26dbb80f373c95bebbca5567d2a4a0a3ead6faff7d16eb89" },
                { "bs", "dc747d89fb056c8d1051484bb23404ddcf13d6098fdea967048256256123e7a62c56515e3411a074552b44ba4794d07fb7a411a36deed5cd98ac8211c88236bd" },
                { "ca", "a44fb7affa093c0b66ff6845ef4f4df9d50e3bbcb33d1010e423d070b43b6f60ac123d16013a035b6b4befdf9a229be16bf5eb75a3728b43655013eb628dc0da" },
                { "cak", "e6ac322561c20d646eafa9194388555b3bfbaab410f07a79f4c5382aa5299f5687786e5a36a02c85466fd35959ac88e6272fed3d946b9dcc55854e4d049ac5f9" },
                { "cs", "48256dd90fa1f7c3280caf3fe9d591a91af462595d0841ecd3d10575e5b6c0979f1e1e0de895b6a52905f3ea65cd052d3511a890dfc71e2a3106d0f3b7bed5ea" },
                { "cy", "4a1253f4fcc9a789669b5895665ae7e93e346c368fdee1b9eee4839a71b6d9427dfebf3385f002c083249ceaa0c0ab9d5a53af1574d7a26891645f9fcd975411" },
                { "da", "247c7241f7b71d70c9a4c467f062dc2f0679f33b3dffdf40998a5d3e8156369c733f75d2eb7635f75054a48e29dcfe929fecac997bf70d57f1360012ccf498ea" },
                { "de", "6173aa91f7995578e52dc1ab1da58af7044189cdb9dcfdf47b0609c0b43b605a7c7b5eb2f8335f5ecd0049fad006ad3615f5b1a548e1eab34208f0c8a8b2868b" },
                { "dsb", "71955efc016ebacee511557805ae7f2447048672f2b5d13400ee96e3196806af6ee1927fb3250bcfbba1464c800d773887f79d5a7ee785878b8b8ddbc683d799" },
                { "el", "38ae026adee20a064bc11b449f00cead6776c19f7e05f1d972d1f544b80a8c8200c457a90fa5f4d7e5168fa879ba01d49577146fe97c488af2574bdab69dfa15" },
                { "en-CA", "b46722cd18f8046a6ba781ad8912fc8685f595106239e5c41e725bd2f155b903bb8444b990ee4676f066c558f08480cca7b9c68542ea113dbc3c712cd043bee6" },
                { "en-GB", "b449f05b4bc81e8c683af78af47188b4633732041c8a337ca73e7d9ab785d1bf0aeed4b5324b301c1050b13b6769063cbeb708d7c1f2c564882417b090ba6e0e" },
                { "en-US", "5c2f80a4ad29b39754f31ff64f53df3dae7067847f94c77df02049a784d9d57cc56fa91961978be41e5b9b05215ab1c8cd8e824f4734e5a56fbadd25ce4b96a1" },
                { "eo", "b1dce40af09cd2aa76460ca87a38cef5f4770545c14ec890f1e4106e30b32639669d0aa2ecab50dbea5fd3f8f5c188506f872f952ad0ade600f10e4ddb040ee2" },
                { "es-AR", "6bb5326f61b6fb658c16eb49dd04cf283927f7b78f69ace6c89d6c38d5c320e235f59dec66c979345df23ce2a18eeb2d5032796fc10513a3cd75974583d78e40" },
                { "es-CL", "68d9ca8f7cb71059e843e33be46d646f29f8784ad74f9b3463223ed28c8cd5b0078583b738e83611add82ba3d3974b6d64d86985bc011db22e9f661bdc8f0797" },
                { "es-ES", "c95a9ffda7358b779df4ee58aedbb4bb4dc20acb2f5840a4b62df03b6e1a20aef0a52b51bcca17022d7ebdd9f99d87e00657e9047257237da4631ee4922d603f" },
                { "es-MX", "f3ba8f4905573ea709e7c7ee6006264125abd930310c8e21abd52316200c64f6c289118f773ce4736e65d9bd0aa234e0728bfe89b46aa9a9297d5fbfa0f0623f" },
                { "et", "efa2e093d4e0568fd231886ec8b781beb5fa54e9752b2f9378ae79edcc26342beef8f6150b8833b96111350591d50b49b7379ce39de6a23ee1e8b859a2bf1dc6" },
                { "eu", "2d248f63b451c25766d42952617e26c2c77ef7d073ca612e4ca72a155c05e258da11219d243f1adb25298064eb3f57d9d202bf58b66ba5b9818da92ec8a038ac" },
                { "fa", "0405f8e7145c303845eef44b19e57bc9d06cb2ac402a66621ba10e673d9042f3fe0e3291cac737fea9bb7f87ead8c33e7f1d8fa2c896a98f55065890f3e18d85" },
                { "ff", "cdd0e8e35d1b9e9ecc9b92bf3a71f211ad36cd2b05a94a13d632aa583ab049be33bbab8e39d2a5cab80902e09cfb00c1da8f568707b496297390b8d72ff7dbfe" },
                { "fi", "10400f59a304eaa2e04eb6aea632a6a7b0f0cfb53c682090b9d8593ee8bcab42ed729c58dd39ae900f7b88f889919de2d53ae4fe4e2091d7dd9850fae9833b6a" },
                { "fr", "3b6a93ab44c8df68c97ec37503de4d984b27801024f35763fd0c87a61cc6ca1fabce476c5e5403ead4a3d05b12ea016d807d66220f504ae3559e186f6697cc04" },
                { "fur", "c6d1a07eb3d70d143a88ce3a82d4ae538e252adbf55eb1119cadd395a98a45eeb63c9206e8677b181a6345fb37eefc861aa80cf47d1dd6586c94585fd85fd287" },
                { "fy-NL", "ec98cd10ca79576bec8beacfb568338edfee7c589859a256fd080e1d62431a9784b4310f047c13e8da0cd31191405e5709dd9c556e7abdafd3b1103458122ee1" },
                { "ga-IE", "ad5cee6022de856e06d3b84cbb55760e66b0caf77a5ec274696552eb390a8fdca8c5c87b8ee27d700713d3f7c9176330327128eeacd71ab1ae39d06fa85dc018" },
                { "gd", "82bcea3a867edd96fac48459f4105dcf66ff7db7e77346c4c870923df09cb6e183697abe206e9db7f1798c399662c398505398026a55e2c07baec32427546516" },
                { "gl", "e242d0a0b6b89ccea8b9ee6c5006bd9354599f498ea2a090988635c6b80a30608674e53ab5e7d74aadd64edb35dccea754ce06effdd212cf5d195738a98b95a9" },
                { "gn", "0825102235969e9929ef5c7c96a72227e1b7f439f3273ebf75ede815c205901cc1974d9c291c7ede43ef55880adfedd0708845f4117abd2c75d0f2574e1ad313" },
                { "gu-IN", "b02a661623560e4032f434feb9df38310e8fedffabf3931976be3cfd03956634edffa34603eda6ba0a20381cd8a45679ab7dafa8bbbd337583fe2660b9117a02" },
                { "he", "22d9d995430fa077423a5044635813363f792a385555772bbbc4022e7f2fc27f5034fab87ea42ee8e71a0dc1b3cd733819ec31d84a0ea617e0c742421e68ffd3" },
                { "hi-IN", "69fecae4b68f6ff90795d1e93bb24f11b1ab75409685c4526a3c1abd572ecac22cb79073067dd8181d960273fe48054fbaf7fb959e4531cd9842a51ce0416664" },
                { "hr", "66976d4122530871bfa3d0aca4f7697d891ab58325a452fcfc7ec64bfe5f218c46794c5c6d2072bc55f1c852158671a75f4cdfa2915790c94ff830be3fce4df0" },
                { "hsb", "ffea3ddf6f8465abd543a966e1ea4bf90dbe9dac0126580cf7217f83b456047f8b1e5d52a1a85ed9caddee23132f1cb16f2c333c0af0d5463267824b6821c66d" },
                { "hu", "a4f31134401fd31784cdcdd71c4772458fa4e5717cf9c94a130d31e6b8fae756d19c3acd63345123edc9d6c70c7774be2aea6d7fead730409cf04dd021c401e8" },
                { "hy-AM", "9812745cfdcf5da1086531e967beb141e114c25e5c3c6461a1bd067adcb71e72510eb8d4adc0563e5381637f74cd82cf8eaeccbbece8a02104de07932f9ce26d" },
                { "ia", "d7bc58dd8b78d62a066cf3cf53b272c2f36d06c4313e40231d372f161d5d167b82ce1617f5f28ec60949049a843de9dda4e1fd01207355f51c479ffd63fcbe3b" },
                { "id", "7480772878689723f65e8c53090e617fc4e6ddf254a070efccc6c99c61d6afee711282021f7efe48ba63ef4b4b3addd7622ea65f7925d629965270851ef9ed1d" },
                { "is", "a9b9bdc9a2799170c78f1fb5d818a6cbc7e5de7ff7f5659db2a58d8cbcf138ef895813b2eecbe435c913216dff6169eadbd068ed748207e2884e8f9fc4017a98" },
                { "it", "04bdb54a787f155f378e5cca4efd77a56d1b4456439923edeea8210e46ab21d0ceaf9c8792fbc32b18452079e5fb9978b154ec5c8643fbc7eea30d87eb41a5fa" },
                { "ja", "f0b432ef28d40fe26a8ea352ed72cbc2f4e6d153f2ea31b617f7df2eedce13b51479530ae5d7cb8510ca9a8f76b6b780829059451859805d75a3bb90b7648670" },
                { "ka", "93dd890f9adb664edd4acc80b5c876bd87bf6924e98b8ba1679b99634b8c85b666e63ae43a4657421913cfe568c05d7fdd2faed14ee8ed97cc08562a26adc4f4" },
                { "kab", "dd42a585ac626b0bba5893d6281acc5cc0f57a643de1be6fff28d4efb8e5e86aaa930a31aecbef9871fcb189568901512ae4a68f217a35dce37aa12ac06f5608" },
                { "kk", "b3a52abe82fec934f4121560136b47ae2b8b1d01068b7855405b7d70bd0fd6d1e418d2121f821f5aa80aba75808e3800bb998c5c95cf9524759bcc241acba9b5" },
                { "km", "53952692dd1c6e5d86132a6679a5d26a04d55959297e4a8412598992162c9e8193f5aff36dcc7e33e97cf05aee75639b985b461ff5d436f3026b6bbde5e07f6a" },
                { "kn", "a00c7e3519c4c59b1032c0f11aca60d5f620ceebbf663c697fda537e83b82dd94e8f515faaee74f64b938f7af25e459e127ab06d5de8c50e9b811384ffef9828" },
                { "ko", "8d8bfc6c9640dface91190b7db60a5ade9ffcf8767aa716ea4443bd9de8101cd603ffd4ec0b724a2dc91337abe4c5497224c01a7cf774a5da79f8c1ab5d922dc" },
                { "lij", "beb50d26363bf2c029942507c84f31a53991d4fd0cb0efd905ed093342e413d2af2cb528353a1a647f0f8e6dc371fcb8a10fb3d9b90b1128f45a619f50bfc79a" },
                { "lt", "2b14895c96c41dc1af8690f99d58fade00aa5fece9cd58c202d869da5ee5981f14c27aa22f612f9ec8298aa1eb05b43e91a4db0c99d3049af9caf466bb0192df" },
                { "lv", "959655364e551b17dfd8c9a05b1bae9a5a3b1491a4cde7c52acda88795ac515dd1d5d6bc7614964cdf6fb576c32f22937335c604cf68deb087fe3223b53ae43b" },
                { "mk", "45132baa7c9cd07c1c69370da8606008a7e881951b5f254a77edb043d2bfd27a90e7050dbe7c750d2f3e35575dd70de76a830fe93c4d0fdc4b5ccade0bdf6051" },
                { "mr", "b0aea259e5bd3691fa7659bfbb5bf0c1fe0e7bf83053bc84164d63989e1f212afb7e44158043549e1eea17919ca2f1514e79059bd43dcbc02e6f5a8d11828edf" },
                { "ms", "ba7adf70ac9d89fb5b40e09b6722727990134ce177e89e57d1489eece90a723dda9ffe7cbeec9ceeed2c78db07eeb32f5aeac0e9f52984b1e684aa43a3ec1a41" },
                { "my", "11ab2490a2b8abfb6a71bbb9fd8260d49065c4be4f14c96568b2b3c80c21cc64425a0b131ae24a0224c53ee0c8601dc3a6b1607c5e52400e633b0452e7654107" },
                { "nb-NO", "592eed733f11101812e4ef830d99df732a79370de621f07d1277e6abb06d602283d1e4c6f3e1c7d513619b40dd93cfb392b3382496206940edfc1df0f540017a" },
                { "ne-NP", "1140a4d0c83f3bdc3f9442f3aaa07fc7c720436762e2df117cba5940412eafe1bc0f8a30e2ef7b7b00609095e4a2df40138cec5b311daac648a9cab2750fbd64" },
                { "nl", "5adfc4e8b06587f366e971eea59a126deca38e6fa92e6fe57b2f888c90a6fafeadf5399c9560292bad846b6d0f0971ea0944b1128646f797c21c37e5c1ba5b75" },
                { "nn-NO", "5186058f4ebeef5bc0a5829943788d904bc3b17366aeb76a4b07da75a151aae4f6ee4654c8f9eb943a784d0143cff669be67d9e248263210345712b4e6d22885" },
                { "oc", "6784a73efc11936c1ded221f487b4d529b3e1b3a2edf841d26c5e03e62b46376f6d47d5819bbd4d13f7765aee9b4b5dac033f9152973f40541d162704d0e56ce" },
                { "pa-IN", "dfa12185946570dab5789cdb1f6eb0097518519ef358c484a75ed7770edac8b0be88646ce962358f2664dd53e639f5ec11f4f354927818c95455d13fd2ef07c8" },
                { "pl", "dec42e75bc3d1ffeedbb9f54b6bcf2a6e96b3c6c07f367b8941f7c98709b06db5675e1cf229a42cd15f12f7329071d5bb37ba428f9ea3a143a184b7be30167ce" },
                { "pt-BR", "4eb2b21c6e40404ce55a4b8b1867eac96c0763df215b47b28ae2bcba6f547c89eb08078d3b4d7270b136d7e789b8c89afd597f6d20d0c56575d16b831ad539ff" },
                { "pt-PT", "669cfb0f09f8aa66d80ce092c90818f72402f43fd7776e16da21c5415553d26d361c3b1c55b7acd93a5362d8fbe1980d489849aca73474e3075ddb404e64e26a" },
                { "rm", "d1c7f03b83bead7a07aa38e0e110420b2584f415b8b17757f2eaa079ad49a87ace752b17d286b5345e2d8ae3d818390e32debe72b8b04bb017b5f090a676e82c" },
                { "ro", "d70faaa5acd6ae284509115071c3bec803df1770a3bfd59f1f981a0055c3680bdc8fa277807ce939c356e12b6032e25b979cf20225b2856c7384d3b72f18d54e" },
                { "ru", "0d953f30e3629fa0ec0df97a47fec6f4fbda1b4d36f99dea0d2be945cbf6c57fad2b6f52d80222bac5404423b9013bc729624ce5319d4d29172a4dec2b35e1b6" },
                { "sat", "749951511655fabff95498dbf107b1ce6209d8c85b1eddd0afb24042290f73051489cf4ed6cd720fcc32ac0aee1b0148463bd804ff7b0b23a8272a933a969a26" },
                { "sc", "e02ef3d3cbb3d1e164af65c9d72318c280d51457e19a3a5b1bbb40c78ae774e3db6f9a5178e33a6c8e67ebca2ddec607f39028029dc5716533044539af87207a" },
                { "sco", "d1f5cfe2b5783b8a7682f3bd83a8022c1df18b61687e6b8f15db1ea5416d0ec55a0e9739b95bac70f4a9e2e08c8bb2ded752897dacbbe419bb17eef834501424" },
                { "si", "a488dc1c11edb2d2c3458aa4e378de5a19df1a7480e1cbdcd9ecf2417ec52589e273df6b8ce4c74da7ff3d270fb7208383f83dddfc9752d05cda1a40a9b6ace9" },
                { "sk", "89972f035a0cd6c16c82ce1272b5094f2be45259f044c7aefaf34200521f46890e981c0e8a42059b2ec8821f3df31333ddc1c7df85b2c80e8deea11a27d7094a" },
                { "sl", "1df23dfd530abf18beb013d22e575648e4740dc4b4e4e7c0e4fd87ee77848e77a6114bbf34018fe116a53c85cb7aa51fb50e07ff8c531e8db6b9cf6965647858" },
                { "son", "2195ef3777a4c216b2df55d8a9e48cf8648d898b02ec0dd0c25f57a3dabdab596b57aa2c0a79d5df19556a0a7a87007d2b582f746ca4e443e2ba5e2316310723" },
                { "sq", "3746f6873e8f0d768d4b778f58987a22ec2dc0d2565b6f01c09b1ec2a59f3f8efa51676fd766636ebecf69b2eda2830e673be336e77ec769dba28506c9c7eb2b" },
                { "sr", "d7b8b2b1ac7805fad4d5edadb5b3645b82663dcffac55b79af9f9c6fba18eea8df319b3e2b31dd74042b0c9ad71d68e74e004df5368c254383289e3f6fc66fc7" },
                { "sv-SE", "2308a63ab3c213d245c8e35e113923b9cee3879a2adf90dcefb728a51b0c7351fff433271303f26339c954220beb9ba11b3ad5df48220f26c942ed820648e1f0" },
                { "szl", "8e7edb70e4d63f89f440decb80a31a863ac0d867f59ffd378054e560ef2b15154fdb20f12975aa03588595a16f0db2ad39101bff84bceec95df8705f9df307f9" },
                { "ta", "a72548fe7656dabc519c251cc3557c2c9ae8834140c6771632343851a2f12a1b19f7fa1f046c756ad2ca295d0f70cdf510c85410ca93fbf1ef937e9963a3f59f" },
                { "te", "757dd23c82c645f9a9641872c0a9464ba41902269d9c7bb2587ef3f1dbc7fd9ecddceb8f6140711e0f394333c55d3240a5f7139e2d5503aace11f251d3273c08" },
                { "tg", "fc36d298b993e08bb4d9f747aca768e81ee0288e6ac2285b63912eb60449363f446101e8b8f74bac71fee83f9df43cc1406329e0750e19ed64441777a42dbb68" },
                { "th", "d456ec49bf3b3709b015682c252a8a526304c711749c566de99b4d8756e580e9ec4619e6b47ec77f56b50831e73c43a4b2c21c8e1005d857e8c305f566b6e977" },
                { "tl", "bce38fa1a39bbd130eec13ce4c9a7bf902e7252e7c6318881de5287e4f829a801675ebfe76d00a4b92557a6d4ed6ecc3b475dd75189a7b003a3ea69eda37bfaf" },
                { "tr", "59a35f411f82fb32d562bf0c6358748a292b8f21934cfa152a813ad62ac4504d4c56b19fe3b3fab4cf6954ff6b0dec8cdef232dc931750de01879c321d100802" },
                { "trs", "7dd307b706606f78e2249bd5e296969b14faf14502efdceee495e0409d51b54982633bd097509409ea5622a788c2d165cb1b67734aedcad9f11d9c8bab675031" },
                { "uk", "1569d22e99af65528fbf341a483ac8b89daf74a2688940e0e6e0256a93d81366b7f6a6b59c01546d7498375e2e4abdca64dd13d12459c63a146de4226933a744" },
                { "ur", "bf9e3072a7eeb995d2d0dea3016c112b399cec311a57ee259372b4d2df54933765142830881e966f0205b50aad454b8e83a55fb3f8129c5833b6193d73ee37d6" },
                { "uz", "66d159c3f4c0ccf293917651c30dae78a535903993730945902f28f332199aad588cb0965aa63d5a04734d5631da54419cb93515f8671248d27772854d9058ee" },
                { "vi", "01c8172f45c9054287a40165392ec1af0f9c4c497ad0454432b3d4e5d5d9ecd6d08be253ad82ead71458ad42db64488f060ca10dcd7b5544c530a048d587a506" },
                { "xh", "9bf6c447023b6878f86da452c799cfd42e31a3048dd29c4060016de93a125db3f441fda970584e199915c25fbcd9d9da1d989afff92f20aaa31605d9d35af4ef" },
                { "zh-CN", "5e274824b51517f3323e75d6204fae34313949c4df5df759993992a66bd34eeacae473c8da872f9e5d5fe9c3bc09fc1e86cc5e6d2b0ce7606fdebc0d0ff5573e" },
                { "zh-TW", "84ae2c6cc041ccf39953fb2e2a96c7708d847427419fd9679f0c3cf46abe17f977f56c842e00a4d9fa126a07a1ab34c7ebdcfc24233ebf81dc12aed3a138a614" }
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
