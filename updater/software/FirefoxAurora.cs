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
        private const string currentVersion = "119.0b6";

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
            // https://ftp.mozilla.org/pub/devedition/releases/119.0b6/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "f2c2a1e491429e1aad2a9fe34d1038b1b1b5498470854bfb7b0431c35a79eaec89b38a5f7e79d1ea6c91c82962238a3f41f548adbe34edbe31f2fc6bb24cd8ed" },
                { "af", "f67021584d4569e3994af4847cf66f4fb807bf718095c8e3efc32ba6c2ada45e32004590bcffd628850f916f0a9a8f0089badf7dd500fec80ef4b6d0c48aec13" },
                { "an", "9b57531b428cd1a192588646322ed63bdd334481ec9735a4129b84e2dcade67777aacdfb38f75d43c2698bb6c7afdef0ce38e86f07bd3af188e22be8600f4d51" },
                { "ar", "4b69abc30e9ebeefa4ba89fcef69d57a60bd2740faab84abacce693cecab44e94312da06fa010616f6f55ff181fd23250a4980a2891ff8e7a9f0fab6fd6a559b" },
                { "ast", "a3b625832aae99d43b1b056afd1b6929b60c7e1c9588565be7158328e89ba83b64df0de0142dcab16124ceb8df83b81803362106108f9d5616f59a0042421656" },
                { "az", "869fb5d8c611fca8e94fd5a7f927b757bd59ba0d99373d0e66b53af611100f1aee26f0ea1b827868b24da6eccff5178e6539ed651671839f99f896f2c0f7e08b" },
                { "be", "a2137903e11ad2e986c9bbefa4e9ab0773b3ee0a5212734413321561b76c99b99ca8b0ce9f5bcf2cb502f41301cb0d426ae16f856d35e31b1dbb642857d6ba7a" },
                { "bg", "ad479359a25cbf046f5c4e3059868a22cbf11f53af88c3359fba8646a6c3fe87586db467c267787a2b3bc02b1c8e0eae0fd09e4706a9f5afa49207da5576fb8f" },
                { "bn", "607f6c994d692e7687c427535b92b5593e8746ee403552f8f2caa1acde6a4aa513e68b4c548af4c2206c984394027d7ffd607211d23529fdc2bea53f2485464f" },
                { "br", "4d39aa29e859e069ad13e1657024c4d70929eca87a7aaaa243094ac022e4aab2117416e536a3b789b845cfb3d800b0cb67fb4cbebfd4515ebbc294623132d32b" },
                { "bs", "56babf8c50f37e1417f9cea495aa2caa1c5a40ec9889a79c5b13b5ebacedf430c64acc811bbc81d051bf289b6c58f59035e7e1b6720fe2415f9f5d5a513fcd4d" },
                { "ca", "2c57b488c6348d5519d3e0ef414e5e5383eb3ab758cacd337605b9a38b51a62f001747469bd18e37f3426f82b709bdf2fb3d7e2c8d19331c6d337cf60a495856" },
                { "cak", "777b7e64a5101ebde2e9ee54b99eb0c3ef91f49c1570a87c57394fc9b929596c7b8a3184a445a73b3e1d3dbc0ca1be7a1a3eeef89ed10b441d37dc4dcc44b868" },
                { "cs", "f58a9d10b21be9e6522bfcb68e21dbe8d6349cd0536d152a52410133db452670e8818006f9a573a003597b8ab61a498b25a57ab035da8706cb9e8ceae3dba738" },
                { "cy", "f40337b43ccf1f515176c9b4513ac52980b56b2d63fe0c36374b192c802286fe3f9193f6439687c52584492c6cafb604093f5dab6f99da809d7fc46e83890e0e" },
                { "da", "3b5b49a03335b491406462d2564a2cad10fa2864cede566aec59f5ec1fc5b93f26ebbebe19df902c9b526fb0e326b80599c08e7909357fc82af0685679b89bd4" },
                { "de", "dd9e0c12b88d0af050bde9d4ce325907891b9d59da39d51160d3bd3c5367e256bbb82ccc9db8f6c53c3e032922e7d8ba898730b4f1f4b13b4d7746c42beaa3bb" },
                { "dsb", "2e3888c35ae89845c53799c3acc2cc46e697ae3b4f44bc8e2f2d33829136738d1a9da40ec4f5449bc9a6456579c30962540791d70f5b34d9d47fb51cf062a331" },
                { "el", "81104d869781177a0e8a282ec5ebd27c8cdfb6b42aa54d2fa11d15db1480e149ed9f71cda75c8a7ca75a9f296b9d21ab072feddb6593ff18015da1895c30944f" },
                { "en-CA", "7cad1eba87b0548574323c2a95f80014f0b910c121c4e5f23de4f069a7519a024b55836c096deed278b307235237b887a76e70ad1badc37321732f90a434902d" },
                { "en-GB", "844675128c35f4ab45dbbad96929a8cc472d388aa08a263a3d054486ad060a27f497601a0d2c1bd698c16165b3a84742582c844e72a9e10eab282bfdadb2bd7c" },
                { "en-US", "05ef481d661f4a38ab538ba25641daa6965c4bf8640a7386214395c35cc8d3bc6f91261596d7de9b50b76cd5ae810c0e81b6e6da8f1b57a55b58b9fb45d1351a" },
                { "eo", "b38ab80c51ce594505be818fb0db9c5480cf54dbd84bda3447f335c7be613d122150247a3cb5c17fea254b7f7909e566cbe6e584200853b0c8eae9e78b93e248" },
                { "es-AR", "4ff174e7444ba3ed376393d356a0b7c5a0847795d5a18be23d43982b46e8a2b62a4be408b9b5067a4f8170cceac9b084637021db615113085d35a7787e06ff75" },
                { "es-CL", "3c0ea2d0080743597c7cf4f8290a0fbd7ec329c3c94fc3798c2a29b8f50e437ce53f68f000a69c8a01396a052348cf81a137c7748afb956592a72451140b1f15" },
                { "es-ES", "d97311138318f7f297a8507b3dc92a35f90b8ff52119b5db1898231ba8b2c0f582d106fd6cd5d679e3c6845b6b9e0f9852a6bd54acd00cc44e00b8483babf3ec" },
                { "es-MX", "5887af79e55e2d2c101729e4cfd4de8fd4c3a71ddd5a6e38355dfc1cb096bc091d6b08fd8f06b699606a65753c3c9382b9f75ff40fffee07d32350d4019d64a6" },
                { "et", "a409646d828bd40fc85393eea781e57511b58d9bff48c6dd42183d5df58551065125abc6bda0a66eb5114c662d5e10800f315a5c4133a94b5c8600da0809e1ba" },
                { "eu", "f670e3e86685fe80b436afebdd6946c196664f07b7b7e69a7ec7dd600ff5f9382e66ac909ecdb0231688e72bea6917faf5fbce1af30ae63e21601138db858e09" },
                { "fa", "f2463035d1c3363263d071425fcb98d1e1c950a5e0d55d916ae0f6795925dd8fdcb9cab64a96d283f674ddf431c9bccf9f11e01384fe284e82379413855fffd7" },
                { "ff", "8d5a30e311d0ffb6df52ef0928735fb16628bc569ff8b4c0e924d738f73172420e6a1dad595466335348f3ce8c6862aca5ae37b0798b0a664446da578171fbaf" },
                { "fi", "ca83df8b5a63f7b1bdc38f6e6be95686b7d50a64d8c82199e0d3e8c554894ef396c5bacf767f6b7a2a1d8703d92bb9a6fdf8a8353d617eb06ff58e72a029ba92" },
                { "fr", "42411b443fc5432f3c39ddb0bc41ef751d9d01c7f9e5e8f00c045a633e4bb79fcd512e9a3230bbd8a470d8159578659e652923009410b0e2ad7f6ef5637efdb8" },
                { "fur", "71468413549b897d5db2f0b35b911d22d6001a3b154683a71f92390e87b19f2d7248eb53f6a9424b361e3ed7601af70d8cf19171a0d9ca484c32e771144aecb6" },
                { "fy-NL", "838679fecf2e217217f5ff1188b9b7633355cc32e683e624ed119de1822ffcf19ab52e990d9fba42010ccadf74c326814ba14517ceca48ba6be30a40ccbe69cd" },
                { "ga-IE", "db545a85fbd6a2cc0c5c08727bb89f77cd375f8793381a9667a318bf2744cbbe13cc8e058125d51aa00dda0cb92b54faf070108b49c3272f4436815babb44ae8" },
                { "gd", "2fbabf3557483c542187e6db538c1f78ab15dcd5dbd4f1fe50493b758b9a1e3da96655568ee97040fcf496d7d1d625739c2d30e1eec0fdc46e0bf35faa024886" },
                { "gl", "fb6907ff39aa3a449735d905c1005b0dc5537fe6d752fea4d924baafd62b7f2245151fe2c7de810285d7c1691c07751e400c5f155a1b79e95055935482e6713b" },
                { "gn", "88c730592fc77ec25e3271623b4f15db149b7ebf88a3a79a101c5c3bb9037d62fd96af50e584a23e0af3da4d0f15de6bf973b56db101d4c22c100dbc62a35fe5" },
                { "gu-IN", "a4a427ce698686f179f45b0c1afbed1894edb66a0f0371a588690f9048035486d103fde269b700943d3134643f7a0adf958829a7fe89066f60b706b9447e431a" },
                { "he", "5b098e41deb65cf908ca734868444a00e2cab10d3c738265069a9dd920a8c0441811d336d5f0c24183577d6e087bcac54b649cdfc0272c25efe20193a6b7d79c" },
                { "hi-IN", "1d420a314b02cf7d35a2061dd5914749ae83d84920f1d2c0927c18aefdaab864e6675eec7ca4bec4855dce6edaf6346c4e2883a1a326314987dedcd9059be1dd" },
                { "hr", "28009020c94b5615743dc8dcaab65a42608fac728d3706980cd859ad8222f8119dba2777c68ef2ae18b5fe0ef7b0e0ffc29fbeeec610b066c7532f31e8bd777f" },
                { "hsb", "940d31d7279c3357abfe9eb014ed5cddffa210a3a2baab8e19dbf2686f93ab74a2d8fdab99562215a73e1030a5d1077a774cf64686297b0e9959f9e63a2b9ac7" },
                { "hu", "0ebeb7664c4e53215d5036e3c502b3fcd7b3b4fdc0c4accfc332f18afdd8384a25b8bd06591ebd875e0845b46bb2e5596616f784631e127991aab45475659c34" },
                { "hy-AM", "6846b51be7229ff536993f82e2859c4b69c3da7faecb6cd9f1dde7874d804b5ed83abe97be88b85b30dc07990203293db1090dcae555b05217fb1ae4bef13cc4" },
                { "ia", "fc2c97d37c71a2ad8074690df6043eddfdbcc8fb70ab90c34f73efb21593f2532d2500dd4e08f1151e8b3322915e206c1776453ad3582f98e5aaada72e3b4e8f" },
                { "id", "1bf74923a8678df72e2d12f875a2f5ce76c22ff16571243a0fa4b1160a4c9983ab18724a195934ea7868eab14f1d15677e96d4fa9089613e579f8d84617f3e2a" },
                { "is", "0fdea8503e81dce590c0021e6ef640a58165ef96035df267707e50aa68eb687c5dea09eb70f30baa0db55345983a5cda550120289d1f9a52530557ec6f26d83e" },
                { "it", "0e2613e0a0dc0c60bf073d96a81a6dedc776612f7f059e6febf44ab49a65a328cac85c2349856cec71d6cf7053f86ba4d8ee25f9aa98610c6176fe0312cf44de" },
                { "ja", "2ce054f6fd6fc307cf8f7b55bc88213b5302d75aaae1a4ed1011075869b9a04b03dc47a66235232e8df82920829b408125bdf825b22e67ccf28d3f54f7ea9bd1" },
                { "ka", "f3ccbbe3f31c06ada027c9c78dd9b694ae0f6bff32b56d7f36bef89202be526a6f7fa25f05bf09e58eb31edce4ae033c31e5ba3dea2f95cf04594bf2d9b05d6b" },
                { "kab", "82c274c655b5b4ed539aed1c5e64ac1d2d47362998693d494a6f72c4effa435f33c126c0150bc482c7ebf9528c101430f23d810875c0de26fdfe5915297d6fcb" },
                { "kk", "02b81564da4dab00b551f114d7c40d250a1aab8556f6891ddcf9b3904bfbfd403f13f757be4580d2355c27c712124eb515fe1de0359dbba3204fe569469a0ee4" },
                { "km", "9f00aef7de15775b5341c2eb9eaa794cb6f13807582995da208ca378d00b6900430b86daf903cb64424c5746eb888884ce191428acb451a56b603549c8df00a2" },
                { "kn", "9ca58bb3d72840b6fb822b49394adca4325f54b02c8431084d396f77d49979ffabf4842b6f55ab1bff0b492970cd7870001b0abb3de5777d2e657c51c5922b82" },
                { "ko", "e1e23a274dd047999f0731bdb46c661825ab304ab7ccb7ceee6e6ab1cf917e0c0b430e6dc0584b903f98e906052acdff5bc31db41a80967c009abdcbe09c35b7" },
                { "lij", "d66fc3ecc34ec7818a00d3de5ecedc7b79066730a26c588a788b971df1b5217d3e6e710f767724e35d5a7fa644957f709d56fcf1b521a433552a3ffd4d238ad8" },
                { "lt", "31df3e2e62d4963bb602cc6a924bcbf160d76575d5554614e0d542e20bbafdb70be040368a6c002ed672e9c55e892cdf1be57dcbad78f47bcfb5ea39ba23fb9b" },
                { "lv", "b9a247de2829ab69e8fdc0fece7fe59833eb3366e6d534b84ee470e48e3de9fc51fbc3cee68bdc13e2f76dac3dc8f12708af6794b2151ae5933fa66f4d7dbf39" },
                { "mk", "3483546378fad42efe9e7a2df0d7f26ba00cfb8b6ef4340cf7df89a56091b4922a72eadd16bc8e186627f7744df1d4d48d65af306dd742dc09cdb5a95bbd5e38" },
                { "mr", "79885c10ea4f1a390d54a81ecd612acfccec7ed38369755f9271d5bdcf1054612e427bc083d632c81eef7a182655e5c7a93d608a6ed7ff1df7ec090e1548be0f" },
                { "ms", "ce1bda53237221ab5f7736e397fb0cc7ff0024d1cccb02e190fb1e6bd43493b95f879ec37876f933898e81c7d20f4a3fa6ec90a588bd6e40208bd0378d024256" },
                { "my", "814cc8a495ac7650166ca31ead5cb2e6cb90287e3e4c74017bb656d02d744ac3b421ca2c565bf21599fb99f10d6f59a1a6e4ec692ef10e934532e49b7fb13cb9" },
                { "nb-NO", "3e3c9795f253a7c479cf02079296d1df83ef9d868a0031258a15b85b190fc0d65a58b75d8e30e62bf620453dae799e8bf4b42c944bcd5abdf1bfd09f15f5f320" },
                { "ne-NP", "79833572c52820e749ed116926821e6f0cf3d9a8fd128543f318c01423764deda1123da283b03dc1cce18f93d43ce8c526c056d2c75c7f56683d917b79b7603b" },
                { "nl", "b6fcddfd793181fcf3a82b5d358e158f1922506da49d472c02c1cb5670c7725172cb750b0456a8be195d51e277ca86c8bb50d833e2dbe0586ad218342845dab4" },
                { "nn-NO", "74918126bbc2e620cb6ba848b02f0eb05ec983241ae384740a4a797fd265c7deef2537251fef23da123b46d0e1becea8a742143ac02284a5f8ca39ff780e0b18" },
                { "oc", "7d91b9934f4343f67ca57f2e3d32756fb8f8deccb8715b7725cf80a0ed439e97da193a0103f7913094ccdce2a60ff089500203f77a7cdea871ed09de9f131bac" },
                { "pa-IN", "bf00e39cc59c08b2ad659f9de5bb049a76be0ff274784d68f2ae0a8623e43b235f71aa475f096a79ba199fc1aedafe570f858bb6f6a17bca472e69f65ee3e1d1" },
                { "pl", "be04e514b69bfccc206599ae7fc08896b0df62f13451331f7357e92c4c57c62cb37deefe3450f51f2f344fba69ef2f8c94d89774edf32b7dab31f70a7236e3cf" },
                { "pt-BR", "e815a0fa236c6b872153a2dca2874c21e7d878a0b2499f800bb2e5edcb0b9c78c5f010f1c538269713c660fed0f2a632c6fa53efb8adb85c74191c0ad47c49b0" },
                { "pt-PT", "158dd661bde12634e269b1c6e23d146fc5232c9e5f6f38f9c9d037306c471e0de40b64d43a57111972257e6e24a6fb794f0508d92731dfa547ca24c6cb4ac7a0" },
                { "rm", "99d718ee5fec9a3a106988a8d4b98c190a8b8f4509c453d160c7688b7dbc6e4e8f278ab4298a7051934bfa48ec4a015b5f8960a2e3d919a97b7af25f51fc4e2e" },
                { "ro", "f732ff8114ada5c9e2d48a7123977b8783984fc3874dfdc33b03890e20d6b55357b3c7ed4694340134ae0f3243c68b8145c0c300c63b2ba0796f3fe3244d3482" },
                { "ru", "4be42978bba058aedd67fcb2ad6be0cd71b044c87f89d9a3f15a04db3ef6b5043351e9a5099f590ecf022f1dd1d8b9d405f088b004c7ca5c7fafcc248d196ec9" },
                { "sat", "7458e27b469210d578d98680045b541de3eaaee75bf262d9d0e4e1c36f4bf881294d650cb7aa1c55b2cc036aa86cd118fcae64b12a47c426afc463f94269d193" },
                { "sc", "a3d3cf8e77444ffe499b7364acf9f6ca6832c4d1dd30fdf73d1782837a1200f9e7af016b1d9f0318c4b4beab7a9fc428593f00e5d3063698c55c176d5a5014ee" },
                { "sco", "c1df0caf62d2158fe85d02ae944df0818a06fdd79514dac5fc4b4229be571f7d606fcfeee145093799ffee8f4b4127622929b1f36e716107a31089b0ccc6f17d" },
                { "si", "1c627d230ff9fa452dd5c67c1e1ea37f9558bf0352e551fdba638124b7fe3db9dc5a383fa60cf12385eea50aa83318c667da5dcdb46a01756f0bed842b67a75b" },
                { "sk", "4495b3c0c18bd2978f78f9431509384ad851725b6d075a7ba901979299b58ae3dc933efba1392849d22fd2e9bfcb12fad45f7fd22b09d8a6885832c79adcbb4c" },
                { "sl", "98f9626d3474dbaac319c690ed97b8eea43479e7b58bfe34188a170125c76d2cb6dfda5ac69d85f3fc5823abc1fed5046362cb9ea4ed805ae4f6a067daa835f3" },
                { "son", "7c1678f8dcf09e021cfbc497c1fe407fa621962b6665bf3a7d7bc84302e31779c19c5fb1f56a24e52e85a551d7fdac642747b1eb0486c05d94602c4b68d64bb3" },
                { "sq", "5c002acbecfaf6335647ba8e6a0f4da5750b20388a44a208560a77e9a951b493b23c078250c482fe8ee0ee44a40ba0667f56ff2740ce15eb74ddfe537c25ae57" },
                { "sr", "d12fa9dd1cf11f183b7f13240533c54a63a758152acd171bb7ce2337fdb54f4401a02160a9b1fd1e288ba8ffcfaa3082c6de3d732b48948af5fea8a027d394fe" },
                { "sv-SE", "551a89a472b31cff471c757c19196e0dea93752ce9032cbb013cebfa4ab0d45e2d6f228df0314b257a9aef6d6ae51510fb9e58e9a98d4dab2e99ed6db0dc34d0" },
                { "szl", "71e4e01ab1cfef7a2bb8760bba4dad5b572a0f146fea457f7cb4f1ee5f4d950869a49623fe4bcf706a1783a8662bc671498eafdb225dc9c93dfe45da0d0f012d" },
                { "ta", "592c9b7427014a592a8759cfbe657e06a3d3ee43d06875693c0ddc26dab670175f26aa383ca4a09f6ca73c275cef3abb69dddf3c9ba1faf15bbdca65b428710e" },
                { "te", "b399a5bbb312450b5736b26498fb5069c0b9feed8e1a565fe0521997ad6a988f897a62df1971bb06aa1cad200d6af844be6c0d78c82f39934c99b0344029083c" },
                { "tg", "1e846f02a24051d32b280ee9f6ce412d44c101f5dcdce378b8b7c41e28f46ec4a2867df97a30f97e8d149ecc6298d93f0dd425dd40190d394fd8eb0d868b743e" },
                { "th", "f0e65bdb8bedae2c3ace7893fa15864ba21b0e2da600f1bb14b74b069489654c6f2eede348c0a77373dfd66b1f5634481e6789812c0e8963cac48ba01c847de8" },
                { "tl", "6090b7939e9722e8bfbf4c5d441fb46490bcce2745e8d5dd5c15e53376369a9af7b6ea119777d57540c3768e696de15a3e9d6b1f4fea9214b81cabda8cd8755c" },
                { "tr", "91bf1cf2f3c578430bb92f40075180fdd5f6db3e44142bcf71fcde06c8284b11f22d5db36513be041105c00ff0c8757e9e7db8010413c6eb2aa1079a5fe21047" },
                { "trs", "dd7ba60d477dd02f8f173280c1ff75f3274fab70d17415a8ca1e64e3a4070c0be22eca65092bbd61cfb2655f78b487144a11ddfcc7e1c0d2609f50a720cc172e" },
                { "uk", "2e58c461f09c9429c0795023c27cbe9abde3f8ffd0ae4919574d55b37deb5d22dea03c5839a282f2762ae34d9d06888602624935ce54b6caef346bf212975d6b" },
                { "ur", "2932083d3eaf7094ba8d53f652215875593e57a018790fc5d9862285fc63261f31fe849328292ab6d3b998915ec64302a377f7797847a82e70c8c5d8558a2ceb" },
                { "uz", "6cbc9be71bc93fcd8322af34f35896eed2364502d1ad82f890bc2075035157d85efc77bbc48fd5a2be4e172acb01ca4b0eabf861b03af627c5a4300aa15153ae" },
                { "vi", "6b56e9438b56f58fc0fdf58742074921629b02644a3e8a0de00029f96ed85078972483e027cfd79cb2b240eb1336292b9e1cb8a369f7845806927aded9f441b7" },
                { "xh", "9cc003b33ad8db86a396fcc98970001690fed0412c8d9044744765981fa76c915a4c7e1fed026e8091e3a3b4e3a4b96fb1428d169579e3f1aaa1e4e57da2a96b" },
                { "zh-CN", "3796ffbd5363f1c2fb25ddff907f8b1da6fd2ef55d46f8b5a5363352f12063d50cc25999fdd453f977642401ed0043fa3ce10d337927faa55c7246d6853dfae8" },
                { "zh-TW", "b6854aecd57bb354b44732b1de933912c7cbacbd3cc36e5dfa5e11e07ac96ad26b7a16000519a4d6ffd68350947aed75263410baa50c4cbc9a50d05f47a352e0" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/119.0b6/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "a158da63fb6a3f81f58785e4273b085ba51d163c6537f84a4352e8c9b6481e88b5f35c0b9191f36ad1190335238c677965cb7019a891011a454d3bda40b5b61d" },
                { "af", "935c4f70f3c5c220ddf3ecb4cb38d05f349a0cc7e87ca2fe9d05bd04feb2a9e50257c8c3a492ecf38c2398533cb0b30e141c9c7fb92f36d70caef1c75ff3248e" },
                { "an", "87ebf953eef8936e0a821a28c41632f51e14f7ef13dfece5a34ea8cbb74a08b913c519c81171e1d9f996d82e16178b9851028db8f0477eb7207c665c8000ad88" },
                { "ar", "04bc4a68639ca256ca9ca9056a1fc1fc3ef0d6a22112969ed075698d124b88e33314b2bebba8e8c7576f0e5de32aa0e0933f22bc2e357f7cb44320172b14367d" },
                { "ast", "404bbabe133e18e7bdd6a671108f71bdce24acca979c40f875a1a07fe156c0586725e11b89623f8ed2578f3be10c0e8081f1825384d006f44a6ec5ad340d850e" },
                { "az", "8d43e062d0573f39b913aa26076f21aa7430e9ba1340564eb855ba061204bd0a99ed87be90609153d001e7bbffae432b7e837975b12bf65d7069844262e03e85" },
                { "be", "5df3350897f6d03074caf8ceddf873afb00e869e5f880d7b24144eb8dfa026377fd8ceccd72799d6481e18a3d2ddf21161111f4bb74ec9e1f94ba7fa17bcff49" },
                { "bg", "603a31f399765becdab5f475449cc3fc8f2dcdd0216c0f3e2c8124afc76c926192ae52ab78f8831f0611d338a82b24bf9a1e7bf64720078c21cc94d0fd3b4311" },
                { "bn", "9f16736dd51bf6fc258eb352c20dc60ae1d919121c5098c27cac9de7ee720cbfe44bf5d69326db81e1884c6ba8e1e048a0bd4982e81653c213e78fc95a500872" },
                { "br", "9e065703f275351a29c62f76ba17f9f09c36de3d20676abea193a366e0b2168758d02eb0701e38dc11f28c42ff38bd09d8aad6af68ce049d0627f4ea72fb8365" },
                { "bs", "6ecedb07d207aa10b437a4c5aa9c75efc4865718f660d0488211cac50df2d63805b01cab1d840cc11bc01ae2c8d7d2a56b5367eb14202eddca4e5d466250a074" },
                { "ca", "248aed999c2a4cc7f7bec4109b08be4d5b932260d056e760bee902292680f62f61acf58d90a0e29c5995f4cdc6e0a84bf94d86029525f4f9a7396d8091bd4238" },
                { "cak", "fc13f9149861a4c2f78ca8a9bfcc9622de64966c2db4f5d62cec90acf895e2ed8d46e94397b1036836466cb74b94c2850dd4e575fa605b3e5a92bd725c07d369" },
                { "cs", "6ad9a2213810ba9ecb370adb58a66538eebea9245395ceb9ea054cb1d828308dadae6d22a9ecae8d729f9e8f887c01aa1f11be95a865d899aa5ddb9e7a88f11f" },
                { "cy", "6e26e32907feb610b1ee38c99c330de4bee69c13f169dbd0047697b1d5cf8d95b238c8e10b1deaded161fd8c496e097dbac48704ef98b7be6961d17bd20ef28a" },
                { "da", "7e515216b1ccc397af76885808ab083642d03ea574391acd47a944e1d82aed880a2876f73160277a783ae78c9ae67b6a4ec472b448c93deb1f78ad636ded8efd" },
                { "de", "3968362f17f86d6b5903f00a50992a8173aa6ceb6c29cad89a33acf4e169f2cdd89bfc82dc56b2fdde50d2d091471de5aab6ecd75610e6805679d77796843a4c" },
                { "dsb", "d348ca414ecb35160399367568d9314cd34fdab970352ce4a51f3243b77caf02ae5735c06a78c9d40c1cb5aaa6b7317348fc18f55ab84664a4515bb583b6642f" },
                { "el", "0ec2eaf10b66cd5113fb4f2d28d6bbbf712696dde465879d95dc617d0268cbcfc778d74ab418b5962623e8bb3b7bd7f47de978001f00ad84276cd5651c156a5f" },
                { "en-CA", "9b3086f7ab3ea3fdb175c654cae5f97204131e665c8ef1b9984d81141dadc9e5edfa367dc48dd980ae7ff035aad0b1c703a9659ae70110d3b5d0917edd3cad78" },
                { "en-GB", "590cfd3435d6a3d5bcbc74143c6778d66ac75de3f471ccdebdb38d0d086c9cf564fd80d24f5215ad3cdf64c5e36470332ff862d4b7f0fd9b46ffb3b471e35b78" },
                { "en-US", "cf27ec09b19b2cbcfae7b9323751a8a92036154f28f8684ca1891c1910e473a6eb65c83bd96d718d3ba70cae52d20852112fc48e2e3f2e6cbcfab30989274545" },
                { "eo", "89069a36b12c34461aa89fae036961a1b13881a28e58bcf4875dcc4f1820ac1a1ca2c7370b21fc19011148ddf129ffc7debf0fe3113a9cb208d45a71c03fa225" },
                { "es-AR", "d82985fa76f56f80d988deac76837317b1464bba0d2f54926034310ed944a32051eb0407becfe95d93226d6ab838048ae1fa098b3c12b33e515de5456164f17b" },
                { "es-CL", "6208352a596d4c2ca322aadc863f50139284e536ae25d30d3276eb1b2f85e24f70e58e333cb3b417150e7e76237177ad92f962576ae28905919e137df0f143f3" },
                { "es-ES", "db06f12ae558fbbc0475a2a0e4882f6d4deeee1bd296e661fe20080dfa018dd599494e9d7a8ead262285eb61fe8fea066a8ad34d59424ea30d1ed4b02f771154" },
                { "es-MX", "6677c324346eb2e7dad3413b8dc4f844538c18b0e7dfa4063e20dbbb54d1d90ab1aa6aacb3eb34319324773d7eac57a63f68dacabb9af4467bb17a97d7b4b435" },
                { "et", "20ee5c162b7f3b5dec652865d9360fa92f691919030570523729ce12c1f30ee389ba0d004a89fc5c82949ba47a98263b50f6400d0e188a60ba76b70da413952d" },
                { "eu", "50054f6d9273b718568dc0648936bcb2b410b31ffba6237858f139f0ac858526520ecc86fdad5b9ca8763ccc2811af88f66353c1d744b51c32cd32053ddf5dae" },
                { "fa", "0a373ec2900c03c4803e6f01868e06f030b8a316a2288ad10d268c932801d759147b64cab39d6609a6745f0dd48d90e7f004254da669138735af63eaf8ad7008" },
                { "ff", "6e45cc61a02c0b120e8788c034b00618a6f71527fc6b2a48c12404c402b45aff0c43a82710183895a7de6b7c3b6780a3a8ceeaf3bb90be77aa5edb6a7e4023e7" },
                { "fi", "f7ad8a75ad28a8cc0a2112e4feb2fc50b4f0407c13ab9f0ff4e8f0f58361b07eea1dbfc3df9c271c8e476e0909fd8489e86afb3997a17d85e808f26367caee4e" },
                { "fr", "91c127cd7d630d520899028aed7d39e7c8e896355f28d80f73f6450a6d4a0c8da205acba50e94d80f13ab9a4fc22cdd2206b5bc82a9b31291fdbfc074fd6d402" },
                { "fur", "798831aa199bf5e3634af4c652919889781242a4ac4418ef50cc78398be54bdab84d0df9e45a34121463c42254586a52eab6d9fbf276cd29057c7b79fa4117f5" },
                { "fy-NL", "c2d6c600c792f1bc8fdefb8d5d55a53be2b7f453f77e9a78d88d05a64cc0845c591a80c574d05e99600300687d76a9ec38af94073cdbe11d4e55221160dcee1a" },
                { "ga-IE", "95a7ea22431c1d7b59f8b96e64c8e841e7746dffabffa0fdf0273cb2284cf00aee0313052fa2afcba7409f31419a5730f41550e225c94f6681616ee4f80ff432" },
                { "gd", "840a8dc76aefdaebeb47d008e246569ff6016447f80b4fb18b3f26b5302c9cbf30ca6ce14f043bdff9be0d1f65573cf3686ad67db37e224e90ea152db82c8a1d" },
                { "gl", "384bd1aba82599b1cbc8ce99569628826810d75351c4ce122a953d20e0b3be234c6b3c563427bc6796765dae28215b6150d801bb5a732d27fe914a3bf4a7cb8d" },
                { "gn", "336ad257ff9518e367321fe3fe73465e6c76805d620cf42e834674d8540cfc214b0e3d748b966270bfc604bc5d09a0fc080201e67530f45f3cce42fe32edf669" },
                { "gu-IN", "53b7a4555bd257bc820073bac69a34aa5f8d03822070f8d18d3800a550342c6b658d9960019da30240b0d35b0e001b577ad0a63cb67f5920c04940202e0d74dc" },
                { "he", "7bdfbc46f4a4cd75534beccacf3a78e1881a9a2e3eeb1f8e64690b75023911e0402c5c92f7169455d58e4d9fd4eb3ec7faacccdd286375c1dd49facf570df286" },
                { "hi-IN", "d6bfea0b0180158a060a520d07fd25a53847204eeb1ff009bb20bae30848f5573b0a3998fa5abe8832851584813b2b1e4a8c085973e01784780baab80efa9d4a" },
                { "hr", "7aedce9cb6941f3a9f462a11abbbaabbcc390c02a7a9d000ce8ba22a3803f3fb998db248bfd407a2cadefd0f510102ecc41f7857f8916f5d213827a6e8f10715" },
                { "hsb", "a35fd2703598227e6c938e7fea8d6a98078f4699f14ddb83f9a153d2905933898d0da4eeb643bae9fad47b9acc8471bdf2e6171f75b977bcc0e29fcc26de6f7e" },
                { "hu", "7a9fb0da8901821478439f8ee9c0e0d88cf661baf5dd948d08ccf66f559c59820376d94d4117fc64e9c0a7270df5e9eed0cbcb2bbd6cf40d4c59ba79a9d05ff5" },
                { "hy-AM", "09a8ef98a4868cca71594437a04ca12b7e303affbd2f4b11e5c72e255383fd6ed89c4c2aafb3a11118a1064086fd91831a2530fa3059739d484e6621acce10b1" },
                { "ia", "c380859cbae0d501c34bcd3384d984e7e1cb016622540acbd89a4ca0680497a60f6d67d6c61e4a697258d8a45bb73e62c1654bea6e86a096b6014be679b5a6d7" },
                { "id", "d55d7a41ff7294c9b21a4e5e405c30f8195ecca652d4c8f14a9c235f50ffd8500fcf275c5330fd1b6b2f34ac3f9beb1fe968b605b0d6719cdccaeea62713428c" },
                { "is", "09c214ebe7afff1d7d9457879ec16af54cc6acb034aec6afbeeb29aa59499f70ddc5795d5ee899b85556901b6ce34c6bb16330d0d62ffb9e1e6e56fc29584140" },
                { "it", "8f37b40835b0a389f1e8b3fb9cb2669e43982dc6d6419d5fcb7833b421b1d7d67e12cfa67d5be1a548f7c5a12bc7bed76d2ffb92843eaa908fba48befe5d26f0" },
                { "ja", "baa787566bf5f62f1fcadc613ca2ab2993e02eb203051c14f0be33e63b4b2958761806a1515fe33c47acc7f87538d96713b83cb8ca80e000706e331ea2ba2fde" },
                { "ka", "0004b910574ea8fec4f163d8df9bacb756e10f5876b2c8d704d6e4cc47013a66019ed0f8b67d1e2ce36ca9961c9b5a95e693ff75305a5140fb1dbd818b3164ec" },
                { "kab", "338f3d9ac2414e24933e2dca0ae6687c961aa59448883f39c53da5cddb241200fceb1d6471963be77a2c37bb474130facb08be40cc38fd718100484142083f22" },
                { "kk", "e7aa4b1024478f10f2f0e2349a682c06dda3fd81f96cd288f1dc115688afbcbfd8780897d02e8c6a20087a9fd8e486b731adf489ebce7bd32cba70bd72289ad4" },
                { "km", "577faa5350649c29653a742407335649d3154c1b6b61223225c4b2518a516fae83d699faf2b210d8cacdf8c09330b90a4c1a0efd438d660cf71a62e1352dcdc0" },
                { "kn", "a35242c0f0cf84b4b18e74fd7235f93d0ca48c074e75f3f60dbe79eed245b40ebb5971d0d86577c3225da1d9b2f373910d37795fe1c381a84d3e69d3d14da399" },
                { "ko", "80f7f5d49e31f71729dfbff8eca6fcaa605b94c4280a950a030bae0d962fe2baf3550044be38cdba513ddf6011b4fe62bdbafbed9a8d0ff34ddaae49874740c5" },
                { "lij", "97273e412e3df8f0d496755b322ecdb4a14438391d249499b2818b7c26224b7ebd99f3a25f842b745d43b746892561f210079a818b524deebf564c48967f8ebf" },
                { "lt", "66543dc824afc8cfdb1f6e7c63ac492c93c08fdbd25af3933e82dc2c3b3c611132683c820b259fe2ca2b99d340ec3b5fe552041c8a1685745e5968dd32ca618f" },
                { "lv", "23f7fde63023e7c30647ec8f85d70256a928c0ec0b5472c41e41c2ddf13e90d905981ff9d4abe621c2016f00dc7e6f385c59ad3ac20ef5fcaffa145466e053c4" },
                { "mk", "03e86d89b7b82f5da8e0cb39f09df6298872f70b095ed4cf0588066b86a0391a4f4f84df3bc2c496ed67d176786722b370a319d88be9a99109e532fc7f7bddc8" },
                { "mr", "38624221cffe160fcbcd84c5ad0976b07bc3e04ad01eed23a13b68f653f7e5ebf667a626ca8d3b1f26298448ae5512fc762e1a8d884f5046f44125b2ed99def9" },
                { "ms", "d4679537cb1e4d24737fa77b60e7e14af642bc6890a2cf0896bce4f77da6adb4e01b1fb87d5f6bbcf4ce6b92fc23c3be702526ff4c71cf3db369042554da256a" },
                { "my", "f39882db102cf4c5cc2ec761e6e612922699bad01632dbd5416e7b7801ec9aa09c3bb259c2bfce63f535f696a3a6665c76b3ef8e3d8412a9d24c7d3d6db7c77b" },
                { "nb-NO", "b5fb701b4c3f035e296c5e258c3d583c6172c2f1fd743997d9b4525c3b585c3e8f0995cb9be0338dcdbb94e5cf03a3eefe281ee7510f1d63a4d7e536c8766932" },
                { "ne-NP", "8e6f0c477bea8f1747b90feffcbcb1f420babc9621c99b864a85ea1c1a28dfd18a86b0d6b92198c7fc49beb3f262dfeaac795b0507a58bc0e21de011b002b627" },
                { "nl", "afbc559e3bb380c726bd881f37d971f3a36c03d37512396f7cbf54f40aaaffb1b0f3f0d90a90a9b9825352db5bb12302c19b5d7f84ca11e29991ece115fa942b" },
                { "nn-NO", "bcb04ff6080adc0b826d09d27ce3b4e44ec326bc60faba23af4f2b6c69918fc87d7cebd8c42b20117d2557d3e5c428b5b63ff578a8a36593cd1089a5cd412648" },
                { "oc", "fcadf432830c3b739643ff0ad765f3de1efc7e48cae29d561ca971a1a97260de416ced7aa35f887b0d09ab12d92de3c310b6743ddbf352ba4afb16f6f6f311b8" },
                { "pa-IN", "c3cef9867a18b407744109a1ed1e17d7ce0a8f0b9a68a1c808b920b9df989a34bb46f0ba9018fa871eaffa0535859f6e98f1438bbdddc17eeb80f20d8f345a4c" },
                { "pl", "58edddc1921d38eb6258eaf305154e785156c2c4967f66f9300dc6a9320341c12a5db06426f02483e0fb9d271d9f44795e348a7985735ea54034b6be43e4fc72" },
                { "pt-BR", "da6b54393d64764ee16d8fa366af7d1a03634ae26c7a1ec2d3d1d2cbce95242253f70042062278995f8eb4b59e90148dd451bf06b257a88245e6d55a8548b418" },
                { "pt-PT", "b77797f38d9df450130c0de2c2d7f0f31659ff8723e94154dde0323faa0419a09e2a6f084dfac33faf3d2d6f1c9ab774965e7da578c5dc83d921dc7a1f638e8c" },
                { "rm", "576f56b2fe99b3546f35360c82b9c43678aaad0cc2a10f059efac75b3b408cdb05edf0902ef0ca557eec0394fafc3d64ba899017d914fb93cb73524d6280db62" },
                { "ro", "6a39c2f250cc53748af3d663f6603b3cea5f2e7820d66e43513ba42874a25beb802b16e7a501b4d8700bd1e844145ace8ab761c696bd5cbe7a262b4ebb10a36b" },
                { "ru", "2a9451cf06165dfae8c3d82f339fa1c0410d2a02068c122ab453c13f4bd0c92ef376ef3f74fe3ec4c53089b663196e7a5c0ceb6dc4fe2c78021dcf084ba81b09" },
                { "sat", "b829389e2ec6d2b673fe53d3f21a94310c043e79fc128a07d4680734c84d775643e45fa922b95d21a957ec93f4fa244692759e8603295b20953bc8e63099ead7" },
                { "sc", "5010bb0e55ecae688b0d9341653327291ce0edffd36786fea409caaea91708a26ed448877ff407613b19ac60ab6f08cb828eff6abcb83d29c41327a6a24fbc11" },
                { "sco", "0f98f37300e86c94d6323d98f9d36c348737bbb69f66dc077b15fa38535626b55ac894465ce1987f8ac64b3d73921b66706e52d5ca90974d694ce3fbd36ea18b" },
                { "si", "f04643206c286255b18713fcc039c0aa3f7f8c321e958600a4a7bc34042f95a65e37b0c3d5ffe3511b8bffa431c01e8537ab334ace2fe0b4c595278c4532249a" },
                { "sk", "2456a8ed5c859c177efbe6de62697cc5b837c2a5c6b51c429cc278c5daf46bc54f769cd4b49e55360722a6dba5256e5c36b4774475381eba646e263dcf92ae02" },
                { "sl", "dfb12486a698030da20a71a9ad3e2f471e50bc7625cfc6cac9fc2d5a9da2f5aa4dca19b3cbe38c6453db65785da1b721bd85c681225adf432e9e2c2356bdf261" },
                { "son", "ad8970379410bed70e3ffe946ce569c195d16a22c33b7bdf56ba4b8ac56e932922735ad6ecb2ff20080f7defbc4df5ac2bbfd088c418beca0942d8847c266682" },
                { "sq", "95c6dc30ca63237c8ce2713eabb22a36451d05fdabb1a2904bd832328d543adcc632f8315a8f4917de0d7179262d45b9ff5a3a58de7bbcd733cd174266ba0bda" },
                { "sr", "884724a33ad980cd487951f320e39ad2443d0c786f092387a4220a1fcac83b4d0cd4cad28b4e0506c125dabcbd0b5eed4a48d2cef20e42c23eeedb368a9f4afa" },
                { "sv-SE", "5bbbf3ff39ebf1be48470c0fb7de6652cfd8b8c363e7257f786b88fabc17760a6362666c94ae75a51dda97eab34163c491761e980933e1a689de45283985dfe0" },
                { "szl", "2ea5b1b59cda82b5ee30914c77ad1fdd21bbdde2e8ea36ca99c75e37c53ab7e55be7b57590384789d366a4f158724d95fba21bd1e0bed888f71f1608f4e97bdb" },
                { "ta", "c583c6c4641ca4d335d2c2243529ac5f21b41fa080b019ea219debf0eca60e4bf8ecbe7d4b11b16be1f48600fde7b37ce2a1b0f9a1babe4e014499a4474c8170" },
                { "te", "7bd3f591a24e74969494acd520aa56e837a0d090b56397600d91d42839b115f1f9831dd238e45254b6be55c4b3d09e592de9b90e8dcd85e9b1303a80b59df9ff" },
                { "tg", "e0da8e541ad7a4d1a4877db50cff488e3f810bd440d9cf01220dc9a348253c94dd0c55f6a21d1bab0ab6bf5ec87b2fd8085107188bb1bcdc007ca888c93436e7" },
                { "th", "866561ace7b85cc39583ed7aa8c6b4ea7c65968a59386c0fd1ea5e44647f03365211e3799f1c9d60a805b2b56b3cb3f07daadedefef3d9ec284e11fa8d1dfb8d" },
                { "tl", "759fd13042384e09317236d8aa19820358ddb817e72ab4f0a550d013256570f52f0d7e461d9b931ad87394b991d1d2b1b9089ab6458671af9c387a29095b41a7" },
                { "tr", "6f60f8bb9e3bc16917c61b7e851a13b884323ef7a0d494e1d66e1e7f69653a88ed6088a9ba88a92a6f96083240d28647cdb864c09dc98e19a287d322c724b9fa" },
                { "trs", "ce3308a7328a313307681479a7686f8acf6fecc6bbb96ba4472035c36271e494e84ed281fec726851324b313692913dfc9732ac76f57a5f4f8e04f176d1d1c05" },
                { "uk", "2cc673d34ce3468b4d6b03ebde66c86a8e83061cd868cd486fb34e97aea7f2cc78113fe8ccf30bc7ffd01ac83189d07392b948f5f0470a8e38920cc33a0714c3" },
                { "ur", "97fbab85a3a193fbc690afce445e25cd8799f2a48288c3c33e200a74e89dc4b83f116c79c42a28ef90ab46a7b2e90da4c65d922a1a2fba929c4597b3d2306490" },
                { "uz", "917274e35affe71e3d636e1e362bcbdeb3048c99c5abaeaa6a99862505e8a5492597c818b827bea71e33ff1de3aff536d752535454aa8d50ca64e1b3a1f9cf20" },
                { "vi", "c2573a372cb56d5b84e0785285c4d094908795cb837fc7d287f470942c71aa11f171535cda5a10fd2c72546fe8c4242a2ea7e3390cbd7dea8a9d0a3883df38e5" },
                { "xh", "ba155ece61a79fc3db6fe8ddab3123c2f0e15a09bba30c7d3351d8fcabad440f07d477ec883b585d0b15c3c8a4835e848c3add5c7a8c061a2c03b6d290e0ec4c" },
                { "zh-CN", "ff645d73989a17bd8cb7aec61386c8219de5cbbaa98bd966027e00f1d65b098e377230d7322ef7391c73d76af4fe31acbc55a7521e8f25325c5eca4523d68d51" },
                { "zh-TW", "87dc0e0d83a0957a0084512917b4c44dd7f7de90b14f02a7a1f105306a33ae65083d6f1ba5df688a7ed89fca6054352ce8213aada562d685357879a7fb4f7698" }
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
