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
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=San Francisco, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2027, 6, 18, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// the currently known newest version
        /// </summary>
        private const string currentVersion = "129.0b8";

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
            // These are the checksums for Windows 32-bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/129.0b8/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "f369589cfd5667e10d8aafebaa537daf60e18fde30bad3d7aa5a009be368da207065584c2e039d52050f745c586cf29a96b1e13c3fe15883e16fae3a3b0e9f75" },
                { "af", "6a365bbce32434a05c38e32445801ba4d90695a8a92047c12041a8b58f06bdd16c2f79031b258022a664be912749ac2d46cc2d53c8b0184b64023083e2178654" },
                { "an", "718777e1655d6efe9792df5e42c778ac4186274257d66bdb17ace2b9de24e9f01835a18326aef9075e2c2cc54681c20ae0513566416174db472b3202c8958945" },
                { "ar", "332e51b878d77724ca661a2a1e27aaf9c261cebe9fc095b36686650a74239b6357982459fd9c11c53d3dc044367cf46a4ac1640d99f1c1686cf8b768ff026d54" },
                { "ast", "f7a95e081d4eff428a1309bc83e90a9c4812d9ceb9a9f3b952655fdd99696733509d694ba3fee67beab55067c1aa10f08016dc0c17e344ecc2c59d890ae6f37f" },
                { "az", "cab7d455d79752ed30a316b85af145dc96ccfe55ad91e90c9fbe4e12d893e6003c3882b606d27991b4cd5a8448982af1586d739a1b0820f6b1fb15b670b74df9" },
                { "be", "1528fd5c64c033f4376f4968710b8b71ee1fec2f5157b163248600cdfe16b7a099ba354ac258368436adf607be9412de7305b39fd8320e22e531ad79ab7ad1b2" },
                { "bg", "30129848b4a0796abdb0fd934a18e7ad431e8190ee33c317611f74564accfc16918055ca561a11f3e4eda01b8c97f7008002926fc329fa24c2e3322d6f3df1bb" },
                { "bn", "9e4ab421f9391521411981821d5d51061d1eb8a2805c802f99803fa21c12a27b1d58c44f58cda962d94f0644ec2c8e5b420857505c3ed66aa937503e37bde57e" },
                { "br", "fd17d6ed52a689013c81e8227b03603f9cfe117e29635dcaac1ea4a354c06dbcc1194e827bbe501c360c49503d095f638543488c47e0a4d134b02d10c2013bc5" },
                { "bs", "cc78760e3b735c4cd6c9dd416842b4257c0daa74fe9e9338380c239fa729408824a04cb3eb280c3a023447428f08b2f8c7a147e083a17cd0e9c9a3f758e4b62e" },
                { "ca", "aa4322708bb6c0bc615dc3c22f2f4f7c656036360b9af654bb946563555719d3eeec5651b23b7c9b25401872f1a7871adb41907251a3ad48e98ce2407460d6fc" },
                { "cak", "1a8ab469e15681aa19d722955ec182340862eed717114f0242c10c4bbb3795f92083557816e9f59188794c27f3270a09341b01d7715c797718328e10b8fcbba3" },
                { "cs", "55be29e045f6b4132973d771349992e5a61ea3e147bc53510d5a7ce988a01dd303cfd1b33b87e3d976f56833a00fa5e0b03b647b2ac6e6f8e935980f195f6276" },
                { "cy", "cf88b2f269d094350a5ec1fd1228af9d8687bbe6834403faa867efd1c54287dc808492e438cc49c6e7b7abd45d12ed7d8c152b836aa11e146729887b14a764ae" },
                { "da", "bdb670a098d06ec2af6edd5f26362b91348b21de3b7d28b79168d643b64b5d4250808b569acb0e0f93405c03926cd1a73ece5b0b9f67ff4ac63d2538d2f5b0f5" },
                { "de", "dfad3315f74249869c6c15817e31c917dac62f31850314c59fc51ea81e018a5d035d387840c5b5b401fd071a876db2b0976550318ff22280124557b6ba4df88d" },
                { "dsb", "4f8a5cb577d935084fea8dc725f7b77adff941325d7fe628261676918de38c2c7aa8bf628e175eba136686bbe0f3fc6cef0d67f3c4b991c6af7a2f183bb0a0cd" },
                { "el", "99b24888b5104d019e527a69691513965d3f407461513babc92cd013306356d98bace4327ad26fd4db1c45db40ed066d448c3188ef43398e25a52c4a430f0bef" },
                { "en-CA", "2724ef51668eaa55a2077c35fd62b956a8129de11fbdf9fc5aca6e013a4582c23f68d6aaed1082093d39af2e9e811cd744110f0a00a8c037fc4f4611d6bff136" },
                { "en-GB", "0e31e5a0a15d83896c6ebd957e8fd7dc9c2f7340008aeb50370c13313e4d3f3db4c18e1cb5a66d9d38b39763e292f7e80b295db7025edf9a94db2dd300fa68c7" },
                { "en-US", "3ce36ea29559c24280d3e02139858034fc05d4cc9d641f90dac990c2e2d7cc1713ae586ae10d327c05fe15888426884ddadf1ff7845a6849b3ceafb7a249dc04" },
                { "eo", "c836e90b33bd37e0c8ffe989aad51e0fda0e02055f62ee41d5a74e55334ca6b61b31805856af60ef50cecb16752c4793a7f6402f66233801bc808d3107a039a6" },
                { "es-AR", "5c547e8332a4da759491dc221a7b5d171df39b91b3237e959420ed44f7d53e95490cf0a9f0558fb1923994ae46da91e245e763910f5e3ca05c0d8f1d0f2fa9f6" },
                { "es-CL", "3f7c663a9f16328d5453a2387d4cab004d78c21677c49dc4fccb0fe9d4d6fc48b1ae2899f898e3f0c42922352f100da04987cb29af3db60dd57c982627b48968" },
                { "es-ES", "d7dd48288f6b2b11716f511cf148ba7f3b898b1717bbf1fd1a0ec4b1a39f5bebee00be3bfbaf241f3630daa049527a9ed86d9a49973d7a30f1cb2fccc2d0875f" },
                { "es-MX", "b2a52dcd7d403121aedb18adbaf6734fe0c2f3e36e9afdf46150722c20882a697f2ef15dc96a486d4cba7afea6f6502e7bb84e3c641ce91593a87d60892cac5f" },
                { "et", "fed2672b16ac63d9879b115cc0344f6a5fb9b2187cdc2ea4349bede15ad8048df64380d405be4408538d5f3c08634a44721575d8cb190a165f897a2fb5104cdb" },
                { "eu", "d9d8bf741ff85c0ef2069b4a11913d0a91aa33c66030c2a3624569e86eae51d494ece0aa5284c65751c8e56cd5c0d88b4f4c7d3fbb3c29281c8dc7982c7a737f" },
                { "fa", "34c83ca9e54d70c11a5568d1dbf578ed45d986d118806c3f191f288b32bc07534aa6e2f0578a64bcc15c06d62867de5942fa993298883c55d7a70b8e25bc6ab8" },
                { "ff", "1a07819e10705ec717934ef0916eb4c80845f954344288aaeb48144140bc9bfe6e0aaeffb2e69edaba1525e42d9dad18543dd410ce2a49a0c8ec6abab79ce885" },
                { "fi", "912c7af307b16795fb0ce6a4068ce4a1596c0f40aaad429a35deb54f947c9e4b25c2840329a20f68c097b9fd63813d2e3898f4a3697d02fba1d6b1855398f131" },
                { "fr", "a0249a9eb249c91bb56c99ec2734c5d3965fb024c2a04afdfcf5f8a4d189d50272ff75fe3ef4672e88574b5ac32ebf53974a51e9858f7f74662fb321c931e162" },
                { "fur", "983e710088e8a5816ecbef40f4a74543d24b1aafe4226572b33b4a934b8d48a169415c149eac0957ee263c3b0d0c7b400d8efd63c033026d191d58646828511d" },
                { "fy-NL", "c6a73f7dc384fc391af1f7f6a3fd05f34cd337a760e0e974fdb24a50de1074bd9148b452fa909fcfbbbcd1b9b784d607393f5d9e90bc72aff9302a0cab5763b8" },
                { "ga-IE", "6aa4c70d49186caafc201332394fcf5bb4f60d06efb2394719caadb26fa5d2a9454ddaff495d2c1f5e32c8b07788248e1a22879cc3b3e20db4ff7a9a8beecc8a" },
                { "gd", "15dc1953f1564a89cbe612eb5a68534d501cde75e48d06f62141ab332290fa65de202fdc36f3a05400e09a23adf79b520c7e282abf3045dc6b1b1726edc32193" },
                { "gl", "d9a3794afda84c6e75b34659e97226be80eedb107ba6fc9f6012bb42e66ab4aa8696300f1bfc1cbc0a0b2cb57f330a2c2770b6ed38104f2eec947a023e930229" },
                { "gn", "33e6a0da773a8c292f0f5cea5dc2c831fbfe0f29ede0dcb2f32d6f32d4bb8013dd764f392a7d576078af83cb289f83bdb54c2530f7f19f9744a11e1b241d87c3" },
                { "gu-IN", "1964a14781299eb55dcaef7c957556ea18d0ff096ec90d9cda674ca22f9a2efc7a21436be84886f4e7441e092ee6b95ae3d3063018052f558024ded945627221" },
                { "he", "74b90210ad19b17834c242c444a4dd8c0a694ebe79fab7930063fd9103785c7fbbda203a395dbc07271901cc36e0a0b4991e610240befcaa2da3932e3d8a179f" },
                { "hi-IN", "35b274c8255c9c06b5c43d383e90cb57a545c7462552b230c9d09c07aa768f2401bb532df4aeef523e64c5901ca51f84ae52f7d775692ae7eac62103fb3a6cb1" },
                { "hr", "867d90f8adc5636cf50ebe8a8fa2c341503d9972cd5db3eee86ef98fc0e5944b24c4afd74ceb70ce6dbf70e1e29840668351ecac3761554abdde4957d526e328" },
                { "hsb", "120cf7a506988c24c27bf695fcf3a95d3a571d3fd14b03d93e2d5bb870fcf6adea66f8ad3cb83b01ab3a3d8460481bca4a828519061278a73471bd2f60189def" },
                { "hu", "f649179d797c3e992d2c8acf9278becdd038e4d94c9c945f3308d6629ef002d89f07f6138c255a17f0dc558803626408b85e6552496cbba4fec21ff2bfaef6c7" },
                { "hy-AM", "bda27d583e1c3faf7046abc1e5df4ebb6aed52e4f5f3f53b4c273251555724976b97e5eb85b2a3500d886fa38c907ed9ccd05d77b478bdea95249ae1f1134988" },
                { "ia", "473f5a219a3161e34bf18244eaba31b5b128580f335be8bbb9d9f2dea656f7c55f32394292967d04091db56bfe1b73fcc2403f0257bbdef6077a53d684cdf78a" },
                { "id", "c79ae472dbbc29d64359a2b84400d04fc169ebd3bb37ca1e776998bf9e96f3405daad5bc94983e7a65400288460e0b94078f2651fedc1b54f671d4c3dac4bfb9" },
                { "is", "31eb18a9a3178541062d1bf864eccb6182b07ce589f3e157cb5b2b938f9d46203eaa1ee684ded901578413981f40cccbac4690a58ed549aaba0dd7b62523a7d3" },
                { "it", "3cd9bdb6b9f7f5590600700608b07008627fa6845d0939edc67cf0a0378b85e96c05531b9508a31ed3b80c820a12aa29349db90a49d647292934ac3f7727197c" },
                { "ja", "1e3100e6e0bcbdf973db3508534eef1a461b2556056cae7e9397fad996e1716924f81a6e67ea497654e6af772a1557c80570ad96a80190e1695d59f6fbf67400" },
                { "ka", "8c62b36e6a7151c95e51c6ba9755cc95c2a7e5b4055d2d47861d3515c63222905f8944abffbdf2d8ae300cb07a5ea427d92d88a1bbf9f7d1370b9397b2116940" },
                { "kab", "9404cde439bfa40a97e396789e0926be2492460a15f052f965768752fd7b9a6a617db209cc156e19d193f0eea3043273f32ce17aa6d83b258f71172bfbc5f81e" },
                { "kk", "e0f3e5b6e8ea4471b7e54601e96da81353936aa5c46b5bbc65258b7c7317be2fb3f831515fbce0684185282920a03889657aa8952fe6c877a21fdcab94ea5010" },
                { "km", "938268c7f09385b6952cf3ba4665b2b026962fbe0c07d268b43580ff868e90d651263bfa9793c212ff8201754257cb1cf2a41fbf680dbafc1caf6a82b082c382" },
                { "kn", "034a893e159c3a0ace8962bec681e751bbe93a78116a349f432d08296e982ded8dac166df40696475956d1dd1b0fa9d62f815aaad43c030150c0d64a4e2b75fb" },
                { "ko", "935180045a360c4ccfd3bb6a28558ca22d585a6e2875f275bc4cde3549b86cf263f7620c9f3cf122a3589a15a3efbec7866f3c789aa5ef3defdd863d2efa34e3" },
                { "lij", "f177968a9b681d374bd8be3bbd67302b9ae99513a66c8c868f4f4fd7ca99aa7125332f3029280e9e8db341462ef43767254cf430e1cd839941d8889c33d5b42a" },
                { "lt", "f117f93dd4f47bdc94c109a1e04e692d79376bfbe409ccab36fcb302a438857608e18d05ad6ec436fc3586bfc588132e22819747ceada513ce2ff2a7ea3a8601" },
                { "lv", "059aedd0a1d616857270a9e9ef48d4f4cccb98341a335e9a2945995c6504756ed01fccb7c5e6fac7956800e3c638cead9350237043943965a6fee97410a77d07" },
                { "mk", "28bcd8c9cd0d23885b9c4d0a41f9145fe94d34faa2cb817fd118c1db2ea548a01bdabe57863c37b206b68e97687cc8db5be9c56f36d243d96ca9bb47aeff71ff" },
                { "mr", "99df1ab03e24424fc23c8028776dc45f5a76f36a74bbb2f3f838e05ec6b33e56fa2a34292d56466bfa5146f573e93d181028723b380c0b54f9159fe9b7b5271c" },
                { "ms", "b0199af6b3382c8bee77613ad25e78638c1f091d6af21c2a73b9525fd56f3afaddc72f6084a5dcaaba591f4b96a6323355c9bb168a7daa34828d3021f90eee3c" },
                { "my", "1a6a2ed8f93a8ec10195b31ac7ede624590c78109a045ceae0afaf50b9f4435aceb1f0ff0db55243bc50ac1525a5a6ae1862d181ac2e540784abbd65f3e08a7c" },
                { "nb-NO", "480edf537a467dfc6225c5a10db40b6b8b679c35e65aba03e23cd5ad3dd46c51d0196bcc8ded5688c208509f90e2d108f7a241873c3fdaa8f4cdb79b4fe0bd12" },
                { "ne-NP", "8434c2ddee2bd22998e83632787aaf956993acd140320e006a8d302e72bc3ad641503d98eb3505dceb94790103e2ffc74d11b78dd9e9a0729f21ca519df93b3a" },
                { "nl", "47226b2f28f182cbd974e4509a916c36bbfc43b4a4133df6389a306f16ac07a7bf53d816de2f1625c04347f18b4aa1ce0fd1856295025fe44f7a3b38f76ea55a" },
                { "nn-NO", "ba0c94a3a716499b8c95a9700732ca5779de6dda2a66efdfd05dede32f5b164baf3e74b8e2ac897a8573188909ce6da4fb76ca94fb5997a2837030708ba02937" },
                { "oc", "a79eb0fa03f8d8114b1d0ee872ddba0adc2d5455b4315758b30650824b8cfed6eda5d77a48cd11b913482af0b5fa9052ad927d4663d8d5e61851dcb2bfa291ac" },
                { "pa-IN", "0c7e8c64c8467de30dfdc4e04720dcf955e20e850c66e1d2cb0f178e841efb16765abf635c35a8a8795b2230439bbee524c52ae3e9f793977fb2695e39b05eef" },
                { "pl", "c16bb02ff370b5d704562639cc9ff95505a24d4ace2930ec0f3a59ac8ccbe6213b7ba2d0dde199ac40ec1f2916e56a6e5755b802064f12dc16df3daed8714871" },
                { "pt-BR", "b67caa6d455e8347c5822707586ba257e8bbcfd15f27c6f3448e7b299d43c0f16625a465a90c8b26fcd71827f07f18458951f247e22a14e5a6af70a17b6f63eb" },
                { "pt-PT", "63d47e8af1a5ea3a5440781cbffcc758f24c2a5168947e714b68135298dab354652c71c5bf9d2fb749b55343380ea236d90c017c4625b54ece2ea20957aeb346" },
                { "rm", "99acde1ba5b6afb39edce7d4b9721049077173c34f9d6d9f502e363fbe17e497d2e413933cdf9561a332a7f3019d3f8293d92e00d25cad632900a4049954927f" },
                { "ro", "8c494cc8f0af3dbd3a60bfce495c521fcd48b45b75bd07c8cec2997668125c7b387eb28a55ba429c0d49e4e5788a405c1f827a0af85a836b141cc9fd9e6fc783" },
                { "ru", "e7ecdf1bdee1ecdcc51a83b0a6a5ef5a860e24403001450a3f052e8f13c83399dd189d7ace19140c89e6b5018bd9b0bebccbee32ab8962ebf4d856b1acd212fb" },
                { "sat", "17653b338a57eaee1dc54963d2446c49d5eb6450afc309fefebbd7b23e969ff883a351cc37c973ad08118f4783b111ef26fec85c3bac8901cbedb0494dfdb113" },
                { "sc", "d45d192332d05f2823c35a8bfc19c4539700f529789ab4e78ffb740e3908d9604a5c6d6b90d3e70710384218410c146472aa52bfb1951bcaeb4cd0960d131913" },
                { "sco", "2ba4282db5245f99ebbb1c3cfe0df3786fb35a92b0abee1bc5bc9017797a35ca854dadde1534fda094fc4422882d6b13953e1e504d9b22174940ce69006579f6" },
                { "si", "a96f7ea4784b07a7241074c6d1fdf60e285f8071dd4b1ee0b9ba2d849190950077832b2dcf908d20c58cb3957ad0b69658795320610b5840a800e7e276af2fa5" },
                { "sk", "285bcd6412ab6eab673483ce67866f84bf0e91be8fe779bc2b781db88c873ca0b1c76ce1736ece20e9bafd53adbf636dfd95a079dd0a8cb31e52679f5c2b0739" },
                { "skr", "6fc8130b3284f1745f5c9bfc4a85041b247a65ddea880935691bda5640f57956081797efa43c4a80bc073e3535026e28ac45e85aed882c2fd092722cd7da00ba" },
                { "sl", "f4b159d1a50350e0872c80034bd24442c8ed079b63880d37887a1ac5212be6af3e5a72e6d53937c442a6d8d620764a114ced3e96f19897e106275fd2b7fc038a" },
                { "son", "622d5ec8f58b8b61a90ef342bac22db5e950fd58004eda4cc0b03e6480d4a48dcdf3fb52e96dfadb9ba9b9b4443a16712d7687ba1829ac25c21f0e27c9e2ce94" },
                { "sq", "29699d5401ddff99e48061df8628024e04956bd9333c463e5b9032e8d2d7b97715104d87a7d11153c45a0c97b3d24a87f2227b55ab9b5fdd091a4f3939a7b48e" },
                { "sr", "c4bf08dcf39a70ebb7d959c01b90a87150a8513fab9e4b04b136912451c699c1b0a92fb90f43ba207a99b56cd39d99364ebe8847125cb65b73163cf8925c4358" },
                { "sv-SE", "f96241ccc97a24c81ebd44b368ad982535b94376120b8ded86fbb2d15182a13ed089f13b5243f4cc5b7687d9ad9afc6110f2d3dd39d83a1c1db7afef7133963e" },
                { "szl", "f833fed6bfa65396bb04483fc2e07e539d83b9e81832986d0a58d32342821bae8cfe11d82a1470aaf9db3acc0c495a5215a61bde90f510e04c02be2015c09a31" },
                { "ta", "7117f1a419f91369ec4385d97023d55016b6bbae8595173a184bae5daaa4da53fa587171a8e0c376b57d04f5391125d0e6e80c49503958d48acd21dfaed372e4" },
                { "te", "12fbf3885c408e47bf9f7616c74db390e9c2f45c6f89a3efa76eb7e3887fdf39f77b1ef7b7ecc1362fae33380ece3422c7bc779c0b9ed9ff3e1aad8ae8dfe3e5" },
                { "tg", "4e3efb9eed7b437b186648e071d17377a4af3e2c425a6b69c391bc631040a479781975bdae2d9d1286bd4a038c657f639bc05d539679a6402a80c8f50e652865" },
                { "th", "5c681d8f9ef8b0b6402402524aaa98b8621204b16c9890e6b432e41860bd9968e2599f3afb673658de91910d17f3cfea0ad118e656545cccf19c1b2c8f782917" },
                { "tl", "2c6e93c4f74ddb35d805a4f1030bafe8a6a20b41631b84f78fa7332b66c4017f0dc4a0df59c5a03af834303af7e643e3fd21521ebbfb2dc0379cb3a9c7444cae" },
                { "tr", "1ff848582ecdd4a75f14007740d53e17de2d4af74816263ac28ad356f8e2242026698eabbb55a25b4587f972149782ed8bc8eeec882ac8998425d3364a9f2a55" },
                { "trs", "c68ad12322d65b1df35563560b11967d4fd8671c7318679c60a9a6f085327d32eb24fd0c34da4d95b5437a1f4238e274b069ab0a87662b34a45d9c563d598dc0" },
                { "uk", "a3424de997b8a65fccd8df1db1bd285ee671f574d3c33f20b85479d4fa83b98ef01fff57089de974050a08bb846ac483700a1682e174cd4d385b342ec1158958" },
                { "ur", "e7ce3e77339c783d9276daf2be9d213120e5af9660c294774d613cca84260c063f3c3d2fa578c86515c5b20016eb7a606da829002bef89d0e79621fb96672b8d" },
                { "uz", "7146839abfa9503ed8ad2303c2ca12b94ae5769bce462309492d611d70e952a1324d0ce86f2b75d2ed523a8c462c79a224f4b4910e0bd61ca4caee0d084b7c6e" },
                { "vi", "6c3a54ec13856b9811f488f29a150f98de6f623866afa5dba1955daa5fa298d443465c9cdd76ced82b5015114863def92338a23381993c6c706ae2b5ae940c7a" },
                { "xh", "3289400ad9a845234888163dcc3beeac5ef9fbed9f1cbb9b7bb2d301d47043565a717156e53d17fe6534ab1cb6a826099862a6ab7caecef34a0bf403228f69aa" },
                { "zh-CN", "1d58b0a428a9eb5e09ae83151553a0efff0bdac61bf20a246c94b8903a2d3420016104fc545410bd35a5e56d553015b76f0c4deb2983208914be5a0d2947e729" },
                { "zh-TW", "13aedfeaee72d5bd14b89451bd25db20e922444b5a40667eef7a095074a6adcfa6f85bf5ce92971a0c78c5eced4302317147a2d882f4bca01682b3a7edf4bb05" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64-bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/129.0b8/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "78d503f2510c9caf3c4c364fbf70ae3a74aa975cbbf6164e2bd4af25b7bfd0ad1c179ec8be645d8eeb531b6e15d4da2d74b02288073961c439aac8ba8ad6caa3" },
                { "af", "9413b374a8d5c9f876a69bce68006c9e08c20a994deb2b80dc12e6e8870df77184a0dd11c5a245271d47829c9c9bc82f530d111c182e8758ece69b2f6ae0f822" },
                { "an", "819228d1305b0ec763c5aad60320f0954811ce21e964119fa94bdce68790fa7eee3e30b985722f3c9cd96c15b3e7e4c0f442d122e0e37ad54cd9e6432204c938" },
                { "ar", "2ddb9a9283a320937e3c7b423c211f27af8a193125e3277164e26903546d3738109728bce09eb2d45388a9699c43c3c01ead7fd214fcd5fb368474c8175d8d1d" },
                { "ast", "c11b7e9b5a13c1e7768247696e0019ac989ddf60a28b120d39ff0c696ce7c0cc517eb6566373335ae52b45bec322002f475b7e832785d536aa1cea60dc9e57bf" },
                { "az", "c13281722ae8509d6593ba84e88b8b3a66b6bffb0b146b5ce1db03fd84f1beb82cfd582dbe01126733853e1aea2871292473714c275a52e64d85f9fa9d3bde3a" },
                { "be", "a32d68dd221c5697bdb280fefb2852cc701d30935c20473610e577cf54f264371a3b356b5380874f20de00899c77b344d1a9485d2801598d2ee3546a457582a8" },
                { "bg", "205d67e3e8b0098b4bb1a6acad0e9b7bea3fd265eda783868ae20b5eacd5e7c2268e7e9cf3e98a664e715d19b84dc9c58335f22c9f7c0f3cbd7b2fbe9526c3e8" },
                { "bn", "df95205e29ce2e0c795278096102b6b85f9535c4ecb83bb207a88c26d2d5656796c54dd07afaca45a04c3476f0e691693e0d5314d3b0519bdcd3a4501275f340" },
                { "br", "b03944ad18c4d3d60db08b4473d856caed75d0c0ca3b465f957871e792c8fd0349296ed84aeb87db74d2bb30f1793592bb79678334451abcba87a1374727b745" },
                { "bs", "a34e1a7f3ee249938ec36a1707c46d7680b583b0c18873febd85d1ed9675ac7b2eb1ffa50b770f99f101683048a37b4c09d4b79d61200d0de5415c2d455787e9" },
                { "ca", "bf3cd431f9de94dd76b7d9af590de98864848e46faf7ff66444d967c3ae0c538ef8de5dbeef7c86ed0f707351993658cc60019ec44547e66b41ffb7cab0f649b" },
                { "cak", "23c7c64b56c754467afdc0af47dbd1fcfdd1a77b596ae54e1f747a181d5842ebb0ca5a0f3e7bb4c724b68bdc06a077e09d800dcbdba31a067d38082589ff52ea" },
                { "cs", "7686d110247f4edf6c8112c79fa19f0f4928acf288632fe0016ae3b571f749fc9a2c50e1f0e05e51c9fe4f07be84fa04391e8244d49442160b765bae1716ce61" },
                { "cy", "d79b55a33d29c2095ae28267e85576cebb29426ac80fe47edfbdcd10352a35f03844b7884f77ef53fde8ef506c1e7b99ecc3cc40c09d73134e962c56af34a92d" },
                { "da", "f88f8ab895202b66f1bb8769cc4f070e0d8bfb9d64438051aaf9b29abb7c883d6d26015111e59c0fe0914d5e1c5231bd92870b1ab01401b038396986a05a2e47" },
                { "de", "8f87b3824ae5a268d0c51bec81b50d3bb8cfb76cbc9bbc5dab5437260bbe197e1e7897338b6feefea7f6d7bbf15cb94a6505152c0ccf2b368f9e5948f0b3eddd" },
                { "dsb", "3b42c74e89cb7817158bb3fcb474277400d30dc750224635b7bba35bc3676b9bd2f1601bfe05b5fd8cff9847c4989325c247ffe5d86b866728eacd800d739884" },
                { "el", "831ffd5afff9869231e2a04fd789c6ea8c4f85667bdb863ae21e362920d71855970a09e6fcc0ea5d53f8dd1c8651094f7cb5339912bcae097800166d3906cbe9" },
                { "en-CA", "3c4d60a002dbcb9ca0eb5178ca529ac462cf1b32fe1398fbd206080d9f0509677362757f37b283cb4e96256bdb3cd19557d8c3b9b11bb59a7d472acc8db82210" },
                { "en-GB", "582ac1fb8d8b17381a7529e18fefaff4a2cee35fb674e71151716070d5b278122b8a74ffce5b2941b0c3fcfa38b36b0541eafd405a173479f6165420679d8b27" },
                { "en-US", "0fb2be5527b5b2c1e0be2010149441f55608e6a201642c5c3185b022514e6bfadef503f3ee10d48281aa64c6d4d823c53e164fc161e8d0a1ffbfccc68c5e2594" },
                { "eo", "4a6f386d76536a28ca195fce8dd3d168cb5ab0e98936d1f8c853a9e0870f6fa3fc6cee830ceb4ba60e1fc0bb7994e8942a0e0bc7fbc5ca2145c190efc1f76d4d" },
                { "es-AR", "e36da42275b3e36ab068c4eb2a4d0d5f0a7f866425df1d9fe757ac70bbb486401b20134e71eb2f7f57a5b73edd9ea8fa6d52f9183453fb2fb90fe54f8d2ce8bf" },
                { "es-CL", "a458dba5ecaf99d1c2c5f7175285a5149e9db25f035a6906d9414e9b4e5f3bf60283af279b89ffbab27c3b6d28d189370d9b667b691aa0204decaf303879a194" },
                { "es-ES", "22ad69ad81fa5903446d0ca9e0c7588e1e6aee640d932a39f64c85f8b8c40e09ee42fc8d3fb7aecb294835b9a3abdac03dba6dfde7f58d73e18c1bfc8a42bf56" },
                { "es-MX", "281f95a6ce029db57144d8787aac89711d96918548f9ecc6f620166dd0f2cf04a6444722ca61d1bea6ae431cccf5daaf895e298cb2dbb0cef99f4afd0844f7b4" },
                { "et", "e4c52b8c9b6d99d5dc233f6238449b1617409e041d3937c2cca5b1001942438375cf7526150cc2e6090a08d88a91aff533f22d18a7ba4593a239585a7c7fccbf" },
                { "eu", "195fd6ef98aff0b6b910969afa425c6b0fcb318f46fcc476f8db31e70b00f9dee2708619d39e6c92cff826101d7b5edf82b4596bc7a7d8c181013f0fe5b5b76b" },
                { "fa", "32ce1c3d60d0b077fa00afb1045640e21a91881b05262f55590dc2b8fbdbdd711bcfd7bd2b9618b0b12dd7704d4d747b89fbb08de3571061dfa89cc5e855064a" },
                { "ff", "7f5c338d0d4fb7ffd12ff3fd07af5fa35425fdff5fe51ff87aae518b8984134a00bcdf1a2e06af9eaf22457ee0bda9e8afaf5e4a2ca996f0eacfd6a7e57b1656" },
                { "fi", "995dc5c73081e9412b986d7af892047860be00d4081461396a96298ad505e533a4ab0bda9c11bda8d277108b4284e739cc351577c3dab96bda1b290de1419464" },
                { "fr", "0cc8d50aae1f33d8f485f10440fcb84cf56e553d132b0877d59ae539acf8cfc1c98d9b8633babd94f6df741db5e1b69d86f47757eb39c8d529670ec943e414cc" },
                { "fur", "d3387e7451d6f55863d4cbaa72a1ce8c04d4dac8c59d8d4a63561fd096690611dc3e16543508b5853f1833f65f678d1690b0319af62d450271288457cde6c7ea" },
                { "fy-NL", "df3a36103e103ba06be337c35b5d92ff093491234e3e81bee2fa2e41e396338446b4df50a11589be1ae3168c3118bfe5962d27e6fda0e68e503c431d62cf6b62" },
                { "ga-IE", "9814d39cf50df26c65f5983227f3106eef0359ca1d96ad1cd3bd15729a456938f148f0ef1040a9fef37f4d1d55db84ca03a54e1552a579ab7fd47c36c1007d6e" },
                { "gd", "2842779b101f9d52ef0eb7c264603a9ee5e63154d69acdb0475e91442e994dcb74dd18fce4755dd4cbb08f780aeb8f205cdf7a95ca86b62fcf3d3e9996358417" },
                { "gl", "569626b42ec62d3337585e1d1f3c41d1414ecca18fd946eb8bb93242bfff275121bb933e2219d75835706b319a0381c69d4219126ff035527e2bf42bfa61c04b" },
                { "gn", "f6d139f9332d79542eb5fbff8a3d4a79f60f09a637b3da1cf3873046be7c8ad4c30df3fc1a0502c3f140d4caf08b02408ac689f69ece4ce6538ff8c96ee03a66" },
                { "gu-IN", "9ebcb61ab5c774122e818f6510a35f6e1bfea437a2d6ae2c9fd2a2a424eb7dbe636c776ef4d1387c911dea033640ea79793d233a8c46ea842af02d0becf07cf1" },
                { "he", "bd50ce56fb006430be63c2f109ddaac2bebba98cb57140c06c93b417e47001e8aa07c8d6670f2f8dc04f80db32f57cd82c6f5a0b0fb5d050fb466e8d0b961689" },
                { "hi-IN", "2b3b7bb03ac137845e496d008849867c1c1285deabf7c01366869caf57492eaaa5f24dab662c94c9b8fce2811f5560b2b55cb5e700eef0f7472d2f1aff352260" },
                { "hr", "2ca911b8c7fd470da33e705ece1915d4814ef857f59de78d9a9deb86a2c71a1cb2cdd9d4889272dd56381fb8835451470ceadb14d2e07331e115fdb3d2f56221" },
                { "hsb", "0d89027d3ff121c26e52b771c8f991c4a956cbd39d81242ae27df89efb0744b5b55f65cce00e8d5f1be527b6784d15cace2ec2cfadf62711d7016d27b25f4628" },
                { "hu", "21c4ebb3b6f5dec96a3d34cd4cd4bbd14a4afe6c5c83ff36437fdcffdb1edfdea33c813bcc80cbddebd3bc50dfc95d1578929c6c94d2bf36e029d9ece46a3191" },
                { "hy-AM", "2c3cf29c0fcb469c3f4aaa315c408861a1c379b5ba1acc6a0f8c02720eab923f218d445c2ef0f628e84777ae969033ed1924f51d187c3e232c1742a3eda1f856" },
                { "ia", "c301653d786f7cc1a33d125f032bb6ea3d019d3f71410a293a9b86ba7b1497c0b84ff9b591f62c9f89cd8e67664548bd671799492abf8b10ee21cedc6dd3ec57" },
                { "id", "661190d76d47cc541e29d99fb15127792c9e53e37b4e003dff62716ecaccc8ef6a36cec691398a5d1a328ac9d7504b4b87883fdcd93ff64f1846d41733db4f7f" },
                { "is", "9c89254fbef0bf14a54d388c5949c9ef94d746a7ffa0c9a49201262f9c3f26210236a2706479c8f19a157a6f89d3254c41cb6e4c4176065d7ca8a65cb97b74ee" },
                { "it", "1c8d050918d12fef21317dbf26291bcf535856da0119dcd4207336a9ff47b5a7f3f4adeb0408c28d1286df8ad7d2070adeef71fb06613183ee53059af0ce9191" },
                { "ja", "3ba40fa47f27d97dbdedd1ba2f715ec36e38bce1085026253689f683153b839e6c84106112d1c11eda2b642e0a35739e0491fc08416aff19986c5091d9fcbba3" },
                { "ka", "85c67c7616b0ea2ad15d11085e3fe5c5743db192678280d7c6921b4d1b5ea627b7e9a83e7873a5a0aa32c4c0aee9d187832e62ecc66bccf686e8ce64d47936af" },
                { "kab", "aed9fdd8356474f799bca22a1b37901096ca8b39f07d3db8e74d647552a9f27767f15937a9fc6285400ececd1ae671129a86c91a2f00c3a4b878d4bfbeb83b95" },
                { "kk", "e13c25352615cabb6d1c1b2fda03f5b31bf897bf9f1b5ea7ed6bacb21028cc26acbeeba02b46b4ccde65ed62a8adb2e1cb3f2c04b6652b089787fd8e159e1221" },
                { "km", "8b58e9133b8840282f12d9504862eb2b538375d061ba13c76964a5da3759fefe974e6fcacd9eb63e78f4f221c0d8031398324598fab27568299fbb04e26d6065" },
                { "kn", "392b83438a952e6a8345e09f236526bb1d68fbeecea754d17a320a700bdef7ae6fca72946317c74df0fa749a0e2ee7f080146e50c098de0b72d93120445f4678" },
                { "ko", "68b3b43680e512320c974b1ce59b242b5219a19470ed3a81417bafb5eaa3d7dd62a9d2503f595183f9da42992b46fd22b6161920a62ef21e0f215019b742a367" },
                { "lij", "5f830f3311acf7173c0e184d77678e478f9204000359250225e33574959cc91e28a31d148e8df8be9bed6ee3cb276ae1f6ff89a1c3be033064a80613b7a37ed7" },
                { "lt", "29cee54b2680e0e70f47bb3598cc58a2de536e1037935df6d5650304f1d0826eff3e592de33ee19c70c934ed49272bdad66c46588b6c6e0e42015070fa293f96" },
                { "lv", "e06b01477efe486775d2096429428675475263a6277d2c190beda01714f3b7683757c99d88762ba4a347ebe71f8b6aae2b4442ae235438eb023f1d231e032195" },
                { "mk", "9d968960fa2e3376484d633bee142ea2521416973f83b2386d2d4013b04ebc3643d4cb6468a93eda9a4fd6083d8c8ec8aea4b1de3708fcab7aa718156a93bf97" },
                { "mr", "e68fec30c32c3cba25550637de71569a34271eed30bb532dfc91fac045f2896f843999265353039cb0031ce0e8c3a8d6593505e970e6e5ff302cd6efe3f9280c" },
                { "ms", "88a0b350dea3cc23c6cd1d5d736b87e680002e5d0fcf51c7b73bac5e21b9167c294860e7ff143e7e62225bf41be02aa5f2ef1b9507ffd4bbd318a69f154723bc" },
                { "my", "a6ddc03c3dcfd029dd4c9eed9f306eb5d4c6492f036fba76cc03d7d37bb91ddbb20e13efe515f7d85baa4f736dc5f35303f3b11b10a2ee2efc3612fc05114530" },
                { "nb-NO", "8145b0143ec1f8131498ede6e8dd748d5f08ecbbafde0b485ab5df98ff5525f0fba4b4f7a046bc3be8ded0a23557f0c679c2395e2cde2ee124d10cb81e51fc73" },
                { "ne-NP", "56dfc174b5b78eb6444daa518883467db355e8ee542bf7c92ba755eabd97ad8f6d3a0d6b1b5f1fbc9b2d1238899f730861bf52d09b0bb515b7d12372a4fad2ce" },
                { "nl", "f1b77475d33121661e79677d1b2e3b9bc2a847147f6af9fc7cf8fe4f6402eed7a00977d54d51ee53f2bf3d3f3ade36b7bd639ea6fde0ad82bcd3c1fadf402867" },
                { "nn-NO", "ca0487d5097aa9d6d7f3f8b131fddc007cf4929e5c795a93adc58a3a24ce607f6b6cd37f003758b2fc2c027172c281e03fd47efde1ba09534df4603ebc8045fe" },
                { "oc", "cebda4b0b9a593cde8c6815922aaabddf0ef8a29f7bb1e3681f3a48b8547170aaa3de7497d36914ad8992eb93f343cd912cab73ea82ca942d819c7ed4bc2f2cb" },
                { "pa-IN", "631c1bc154ce952dc558bd3f1f53a1568375e0816b2971948e86e73a3e0ae1e0ac8b473c753d0f0c55936e647d1da4df0d017d66c22b043e1d29489d162e2857" },
                { "pl", "bd9464895fa20afe4a7bf48c7b37c72875a90d46d60e2bc3725d4dcb0b2778a00ea00fb6753a44aecac826e20d891fc4dbeebe8fa281c63ac88963c1b15ef270" },
                { "pt-BR", "3d2a08f539fad4bfaa77a8d0db1cf4709ee954c310312256350abdbbcac0e5fa84979cd4dfd925fc2d9ddbac104451bebb420ce65a5a681d9ac9930b5c585580" },
                { "pt-PT", "494d2ccb99e23bc4e9590079c0ea3aea8e915b23bab36d49107cab1052574c201f73e3c2bfa438fc801c02b3bc6dfcb830285a78c148279209984bae063b06be" },
                { "rm", "a60c626daf610979a92cd716a6203d3b21dfb92521873ab9802b619606e32bbdfa7f5647800547c5f0245f50310bca2bb7ab6c5f46f78edf30c47e5b19685103" },
                { "ro", "0913a7147fef48b986b66a7413e358414ff00da559e7451b6595c56ef18f76735735d9163481addc6917de17a444357db5d0ea93242756b7c1356ed9c48b7e06" },
                { "ru", "20fbd1c307272bdc0f5d9a79f437671dc28f49620585da8dcc9675949a53eff6b0e32564f443a8028f89afc29ff30a11aefd8a0b267dcb9c5630e98e1702dfcb" },
                { "sat", "779ce771a52489749d34fabcd7e47849381634faa2ab661a8c44c577ae7282eb560ed3736a860c1aa7d80e59c980595e823aef3c1ce9f9b20a490cc16ff7cd8d" },
                { "sc", "6408a9debe680b452213f1b0d91d4b8c6d15ae9fb3a4f8bf3307c0ee31b2c06603e993c62fb2a1190bb5295ec3de5f8fb76ccfe9118859fadd4b7cd6467f095b" },
                { "sco", "cbe9f06dbec6a01eac7403e113c21c663126225702179868f06e8e268878e0ce8ec45469541653ccbd9d151d1dbb0499cdabf96eb4fc8895878d4131db1fbd7f" },
                { "si", "fa0429fb264facf42f8e4d28c44215f5fdd46fdfb29a6c6501c923172186e5fc6556e8fdaf732cb97c1e80b3ec09eabf88d15768b732883fd03f2aaed9271067" },
                { "sk", "3135a19ec50f8bdaa43b51134068aca1ffa301871f7d5348b8be5bf6ad03dda94cb08627f17b54186ed63a44dbe07c9b32e49fabfed1fcffa528c876413c8661" },
                { "skr", "0abbd4ec1deac46754937f758923e078a715552f242b6030959e24aa3d13fa15e264ea32d9bc8f2e4f89b6dabf7eacf025da082d5dd06e8894453d3315d37016" },
                { "sl", "ea8fbebbd31d46896a0a98ee3230bbd6047132bb8a3dbbfff8b74430b15301b82848da910c196ccbbd272ac039febacbf09fc24b79e0bf3a8d06b55b00b95480" },
                { "son", "9dd1a5b1d04cca633f97cfcb11ad93effcab8901d4bece33ca61a75265f7f84ab81e54404993cf110e5e799a80f0700aeb2f50104c40a729095887c1a2b3412f" },
                { "sq", "c1ee63d1879243ee4b516762ffd1856ffbd20f0a1db19978765fa430d9699fb9aa19d43c04c4b763148d5ffb4c578486e7b9eac4063008651a15b590b039d7f6" },
                { "sr", "211291d2a7ff3232c646a6d4effbbb32fc75838c579ab8ced91c62faabf617822a5c5a5ca7e27dff1ce0faa4ac74cc9813c2ee527287a9406d199a79fe78b4de" },
                { "sv-SE", "76268e0529b003c90621964f9dfc1f709c9231745d38f5e1adbd90afec7b13c0527e412444c373ab1c6b71fba26a192302cf6d5d342305fad67334df2039f741" },
                { "szl", "a571a5da9992a9f945883e3783eae2012dd79041dcadb03ea4729f08282e95a84980e2558ab58e45a0ab9ed29ea6f6458fb2ef4576270b006c4fa35e21440f31" },
                { "ta", "e8ba65f36358417033e544bc746049ad7c787c48bd12b031266bf805c20af02c5bd9ed25359f64c2b60c5b63bfbe06bb8de4c75186fa31d00f39c69888492c8a" },
                { "te", "735d8c367cdc9bc11994a80e2f0745633e61ebc9c97f088f964ba7b518a8d1af2ce7d02f110090c727d948126f037c5f70e63ac8a705a63a76659cbdb1989354" },
                { "tg", "78f88dfe58fa0a3e78478744aab77574c0e87dce0ab48fc4c4ed12ac0b941c9508c93d217517951a719b008a8820abbc2b26225b564e39dd07b18f69d530a526" },
                { "th", "e0af2cc8a2b413073dbc88bb8180ebde79d7e2beff27f913584ef899dd68583b9b047787203ed8e2e14060562bc38d75cd40b42e5d6b6193728eec295f9e6f84" },
                { "tl", "320127cebfe97e42291ee7176be0a445c0c3ef0246059913634e519d0d1178c60ea0067c1d6f0d817bac96aa878814b55c0d20910f337c875daf5f16ff28935d" },
                { "tr", "988297a572242f74fbe336ad0d7188628f0a5786924b2af1edc2522df3addb6b796253b11f7ce434eb855cf10289075980d1ef221729ca71c2552a75591f4aa1" },
                { "trs", "f5781fb6b3dbcc5f3fad57ad0d24ea48337390e93116d86c60e60f363662cb529b852226110eb466e8cab6190b3926cb7b5287c53ef4bc4ebe212368f94f77f9" },
                { "uk", "a7052a094a13e33db0f0c1b1f43e76f38f587bad4322e576c90f73a7d39511bc8a9737983ef2777346780fa559e8840992508616df5eea7f731bd3d2a6509795" },
                { "ur", "d564b9016f5a8ebcc3348bf1555f2a6935a18d4faae8c51a957a502df5503ff7028f8b3b17d341bd91ae69b7c0943966ab45dd14ca03003023c73282f38a9002" },
                { "uz", "132b6463df663fdce15c3b62ad02e823439200729cd2d4b161e95c2585e38e42a9c567f6462f303514796ca4bdec5e475fc1e42595de2547f9ec27da33f6b40d" },
                { "vi", "562d031d99a61884b2c568cf125c2069e0905e35baa7915a9a3f26ff5b55f972603e97be97c826bf8727d46a77d1f4b7688b71c72ce307b00a677394944dad6a" },
                { "xh", "f06520a468e5fc051edc5224ef373e73ae7eddbd9665b7cb803a90591658118e34d90b7ea983ee8f5fd119119c412231741ffdf716e180501d0a672fc088c6f1" },
                { "zh-CN", "3f3b233bc86a605111402f09caf728f7a351970eeffdb7b4dd99cff849e5820544d1a2077a394b99eb41b176b980b991cdd8e4a153469545ad167ebfdc36b19d" },
                { "zh-TW", "9975d23e3c6b601fc71994d52c05c2dce1ce36c00ae87ac225abc2fe0e501e74609db446fd6d75e7015dcde977363004c6b6595b7827561d4e32b2d641696eb6" }
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
                // 32-bit installer
                new InstallInfoExe(
                    // URL is formed like "https://ftp.mozilla.org/pub/devedition/releases/60.0b9/win32/en-GB/Firefox%20Setup%2060.0b9.exe".
                    "https://ftp.mozilla.org/pub/devedition/releases/" + currentVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + currentVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64-bit installer
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
        /// <returns>Returns a string array containing the checksums for 32-bit and 64-bit (in that order), if successful.
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
                    // look for lines with language code and version for 32-bit
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
                    // look for line with the correct language code and version for 64-bit
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
        /// Determines whether the method searchForNewer() is implemented.
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
        /// checksum for the 32-bit installer
        /// </summary>
        private readonly string checksum32Bit;


        /// <summary>
        /// checksum for the 64-bit installer
        /// </summary>
        private readonly string checksum64Bit;


        /// <summary>
        /// static variable that contains the text from the checksums file
        /// </summary>
        private static string checksumsText = null;

        /// <summary>
        /// dictionary of known checksums for 32-bit versions (key: language code; value: checksum)
        /// </summary>
        private static SortedDictionary<string, string> cs32 = null;

        /// <summary>
        /// dictionary of known checksums for 64-bit version (key: language code; value: checksum)
        /// </summary>
        private static SortedDictionary<string, string> cs64 = null;
    } // class
} // namespace
