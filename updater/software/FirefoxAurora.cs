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
        private const string currentVersion = "126.0b8";

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
            // https://ftp.mozilla.org/pub/devedition/releases/126.0b8/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "89abc17fcfc43cf5d29ace08793f3c6c78c8f16f6522d0c5411cedb3a9920636e40e2fcfc45374a21052470b4cb388db24a41243abccb5b0a6febc4306d26472" },
                { "af", "7cdb29d1f1ff4a7f34822502318c33129d59e583ba58dc81f55a6d996d80d0b32af71ef1d32e99a277169f5b410d16aedfab161e3a614277b4d2e942a7999625" },
                { "an", "2177ac5541c9fa387f1fdbeed10c3b58fc66e9f642a2bea8a43b068fb20e8428af3b15c249b5a57033413de30789133b6e5d750cb44e46f2c95e2edb6f1f93e4" },
                { "ar", "3e75a0f4f5b20d55a5bd9b3ccab18a57800cc10b8c6d4ed7d3944b3a3df896065eff2326477943e0be5ffedeee49030020c16a4748a28f65d26ff1127c6446a6" },
                { "ast", "84f437add31253af9e41fefef85e691767e67345b2fb70c04a82350c93f77ef4b34bf15aee4a4c0f8a00dbaed9927c47ad6cd35ebdc32bf8309b26e1da5b8b7f" },
                { "az", "2610662b4e7286ac5f5609fc7fa618301905b279cc59bad2175ddb2b65401a6180615f6621603d794ce01ab389241a848e2e9d2f44e749bd3ce6943b34ebded9" },
                { "be", "aa85f5bfa9fe41f59d6aa35714182c1af51d73def9754e0cd6a8fc5a03a0dd0fc8b312313f51744bf9c56579d447f2596e16209f0754afc56a28f71af4328a9e" },
                { "bg", "ef64467c7e69bab5e424145de216f892961738953b078a011dcac9e39d9016d4d4f1d6510c92e17d8281f8f331925cc928c598b717c368eda1c71e6b12d32e59" },
                { "bn", "95027667d33f06386a81b997538f31bd81c9b08dfa10bab23f795e4bd40fb0c27512c0e5a71bb588c68a040b4899122fd6382d470343dd67b4074cca0926f017" },
                { "br", "5831362e5c71f0ae5621d778289c64372ffcf8c8138dc4a32a576965e8956ff152b6062cbc273c4cf0b8da06b32cedede1e8bc8da0f2ce88876314745d0881f3" },
                { "bs", "caece4489cbffcb5cc1883943374b79b85588c56c00cf8990831abbef4c85bdfe6ef3ee261448fe96d59f34b013c8093b60147d5a861a12a4716dcdb16bb132d" },
                { "ca", "5a8dbc3677aae98ab09f1947379f9660d4a86ffa571b83f4bc4866080b6469e2e92e472cbf87343536b28e8cdc0b836a669d3bc328fbd06da6a32da39344a538" },
                { "cak", "c6b8b0a35723b1a51b8ee9947d29775a4a5d62b6cf569afb59b8aac363b4b45c00c760a295d44f84d0ea3e801299e467c93c4f03f8abaf3a309723e54f53a3a2" },
                { "cs", "5bdca3bf928909d2a7581a94bc4e51402b1a8c1836928ea7550c9db2b30871d806feafa41a678739b9d21de6e0b10c058d4549dfd997bb6a1a4ac59c27c5008d" },
                { "cy", "414d6a5dae41fe2abc749ac892cc5bc01d63b5fab9188c1549f166d72dd8f3fb20c4182ffb1cf3c632a95362c45f93c702249db4487016a4d5e4d2b8855fec5a" },
                { "da", "6b6f68b5b24ddbad0849a01497399a019b50e976fcd59fa66f380f4b004b21d95ddf62b38f19cecf838c7c6beecff86b4f285ecd09bf2f26e2736d7c9c993e48" },
                { "de", "7830ab00ec6bacf71e4919774b25ecb5d29c39ebba36597c236bd7616d8780951bc3e3046804bbe1a892a9b1b8c75a6fb406a9f6f9f08d4c4f532100c6307bb5" },
                { "dsb", "aa784a364fec3ca8255f386aa3a7990365c84754a9ac81db4079a5b119532bd4474d01e60304c229c4e40563fa23ed656f3265c7fa8597c6daad4aa46f67163c" },
                { "el", "f271f4bb9ffbf0534a8d5bc5d4b8f3d1a5e83502f1625e8bd0e70539bb9e43b70a2b878c6a6c57b44c0130e62bc7ad78f8916e226f56b138440e3e8c2fd6f075" },
                { "en-CA", "4c72f93e9fa711f52091b74e8367ee7dd12221d96dce4a01692400a4e0f99bf9fc3f1e788f4faa5e5029af16ef044666535bc37b648009df9fb2415abe1972a1" },
                { "en-GB", "33e69efb9ccf67c76e777d9ea61b053608e5ef6b4c12230df090c885ca6edaeb37b0b84f464db27e741a1b977da40c669481939440d392f4befd208c5131da16" },
                { "en-US", "e223c956e72d4906a755b317e2a3643b8c80223f70994fbf82cad716efafc2387e203d4d2c1e76e378aeac8a781f656c90376bbe410da33afc0a6b8fadd7aebc" },
                { "eo", "5c85633ea97d95a6e472fc6c55851f1f89a729d6380ed4c190dba740e2b4779f4b79c7efea9b78029bc6800e3f028814464a1ce685e6b0493fa45d41940d996c" },
                { "es-AR", "d678dd1e427a6aaf8292374a2e23d57c482a3f0ef58b58ecbd47aa313e1b64c9daf6ee43a78c18809d897ef8f7d039d732a6bd0e9a303daf909b42b6370434f0" },
                { "es-CL", "3299570857894360c0235b515b924d102928800b1f8512cfd0b58222d386e69ff76a43924ffd9cb6aa9fefb8c8997edf4c1f7af126343462f7175c3c56a22f6d" },
                { "es-ES", "74c05339bd8bae3303c4270ffca30518328557a792eb677253ae4852004771467c5dedbe20c8942df59cb5774e1e1c284afe1a8ae9cde71164b2ba30af94379e" },
                { "es-MX", "6f8ca0fe4f3b6415acf8a0f61338a789bfa7b8914d143a52e7664094963a19adefefe235265a05a216e0db6249bba6170cd7820a3cfaf8da1b0d5c861ffed4b8" },
                { "et", "9388780ce86437f4450b803f3cee8f3715f8957361da1159526737c8745ec61bd9c8fec8bd91f5c865455fa215b6910dd401486c26c672b387ec607c2ee984f3" },
                { "eu", "2fc5912caf4b1883923a27b1d61c0a726a1cb051a4c4b2e442db218d47252a7c64058bdf1c910c6d6b2565298968ecf3fd34e7f2ce5c2ce07ceea3bee08dbca2" },
                { "fa", "260830ff3be44c7c8415c7aa15344ed917c2358b34e55d5571b62f645dfab0820f8564e4b6a38a93c065d8d890bbd94105b2710a4c52b8a754dde9cfdd791943" },
                { "ff", "1857e27209bad27d1fa316bf62ea0bf3e82704589d37ddbda8ef851fd33050c0be5ef83ddc803687bf87668073f2ad622147f0ba2ac844c5e6170e79accf1c55" },
                { "fi", "85c3beba6cfa0e71fe4c01bbc2dae57b68edbe03709e428a79fad82b6a8f73cc002142204ef0ffd41d62eb2b6d64dfd23634c6884623006903aed1848f423249" },
                { "fr", "169a18621fb220fcfc2aa3b8c36c38e32085bde2caf1ed61b61caa59098584eb906e946be941e237937db26aae54d7c3328d659c7ceaa0a4f408f356506a86b0" },
                { "fur", "e4c06a0b2e08eac6ee11abc575414a5e61bb4ea81a1ab71c43ab978f1ceda787e629a824460eca289ace097852c850ec08d45ad543aa1526f42a6455babae97c" },
                { "fy-NL", "96fb31c93747eb179ecdfbc18b5cbf50e804579b4fd444022470fc7bfb7f2aa92ace5f4c1f69060a2063325cb0efeb4eabdcda2cf9e458043fc5baaf8c9d342c" },
                { "ga-IE", "fcc6db7aa42acc425490cf37df4bf92f5c0c9d28f4dc8a42555285995f61c906632da9616cca7623b395e689fbe4e8800cab31451ca7f3a93d35e0323f877aee" },
                { "gd", "f610e539ce0431ff91456f5daa9599e6fdc01cc240be0ac63fcad11f2b858f32d38db75460e29f9771dbbe7cff0744987a7e5e02baa71c609e15ba2912ae3a32" },
                { "gl", "65d58b4b983e952f8eb0c2fbdf80b102b15b6661b2c8994978c23ad29d07f08b034ef19b2c3573fcb0592862847abc5c270badccafa14426fe9c9e4c62eaf0bd" },
                { "gn", "69bf37ba80fae75aa445e4c675f1c6b4f0309365dc74ebbc44fcc38b6626cb9bdabff1beb19506b760257dd713140f90ccdb7f0e4d92c36fcbf1bd4c47d009da" },
                { "gu-IN", "ee72319aef1f1c188989d64f9412f0c47c4070f17194686b01166d88157e487df7cf73fca9746a955b5d39cb833c82dec64f7075ece48dde04c61d13a74d6590" },
                { "he", "0c4573f1f2a847636ff1035ebc9995449d9a850256f6c6c9e772c35033298b2672b2b8ca4f44bdd7e23bf2a85303689abcbcf0169a6485ccbc00971ab53e45c5" },
                { "hi-IN", "0b0c3f0c38dd82364942812d30f571c5fd196f5eb738c1e9f9e30518797d01de1e033195e089036f7121d8f03413d43580b9bbf1537ab02ff5e906a017a91d42" },
                { "hr", "aa8c31b48e3f7378cfc7ce52a451f82201876bf6c4a5f9c203d0a144912378ec2e68767da935959c8b84d782eaad2f16ad78f94c7b69625685ff6358e33a0581" },
                { "hsb", "169366851cf7b62967b23dbf5554c025ba0c0692efda69b6de86003646f0f689613634c3046e40e572b3c1c5d0546228be6110e6f4edb9e34e00f19a8c4e8a48" },
                { "hu", "96b1977444a749b47171249c91819157fa49f531a4d35b3d9be9b6d553fd6ffdb3a9c5f7e39e59cb3447f8437eefa19dc9c97101dd06f5866c0beb801ab26c40" },
                { "hy-AM", "3902f176af3030daa6b5927eca43b689dc2d5423a50ccf4720cddd5d9ddf4eed57f5f5cff838455b014f4e19d86e5fccbabdcd34b915b499991a6dd9cc93c7bd" },
                { "ia", "31518f7d409cf02567ccc3304f7d18d4cfb7e428a83169e78029690a0be896e6d2adff9b3613f53a0006ca74c246e279e74c012d1fc1058631cffed0522dea32" },
                { "id", "339fd0010de3c0b14dddb2414ca3344165c0a79cb6a36b37bf2f36924c430f1befad8ab485f915c550b52d810529b123f8c8df6683f2b9a7a91cf7407ac87ffa" },
                { "is", "61233e3f320a79481df34b4c57cccc93508d42a7b7e7fbb0f4f0bb23d66c7f3a12487ea6601bf54940dbcf99f0124531b510b141ff29ac482c291334811b281e" },
                { "it", "c86344d6c8e6faf24f03be0d434bebce88c7d252a461573f87891973c15871551f945d1a44c45da76cee1e5c9829e09b37e0a9b9929f87b1f8478115c1ef35a7" },
                { "ja", "577277e910036d1cec93687c6d935d111bfd0944a3151e888ca927d6074ef494d0c233080e51435145f4edeb3ba4a5158ada2f703f00b93fb9152e2b15dec81c" },
                { "ka", "8597003fa0791e18a1a329fecc2a9453dea4861e3677f627219cd8914b1ea554238ec4093c7201abd5096685676deb3b26dab559bc4fa4d8cb02ffca70748f96" },
                { "kab", "5bde3a9d2162f92be7e6da7b0afd176e2f986f03daa9001ac53b7155c6becc1406cabc6155b1a30b0815aa3afab50169fe6d7681e2cb45a040e4dd866a7a5c0f" },
                { "kk", "6ae893d4ce63d913f7a63d6b05d35db8245608099ef6bc9c0f525512aab3e6ce5fdf45197e5ee199d86dd62376effae18c01636a6cc1758a26758f6a0adb1906" },
                { "km", "0ca060cb5690bcb9d4b2a7c28f22fb127da25bfb68c9b9f630836daf749e80eb37399a6795907ec6879d198486d8304339e6a10d29792cde2541a443833bd5a1" },
                { "kn", "e19a247d64a4ccd48eb26848eb6d42c000f92f7e400e6cd9e1807a10e93cbee92896e80ecc94824d8b1525d36523362172738550616fb36d8dbb5f7272c0f4b8" },
                { "ko", "2f192dc8be11bf8b4becbb9ea4caddea6eb01927e4b04359bf248eb97aa1a0ee90abec0db2966e66545907ec20f8c8c59b44d9d4aae98df966b9cbc1ef4fde36" },
                { "lij", "f7cefc2138b8a677216c830e9203a0cd08aecb5eb2e72177e9dfddbc83d93a045040ece2502180d68e8add5e43dc94188340fae44e5b7fc00f0e630f582db091" },
                { "lt", "520aef30d74ed31b13d5d50359a286afb26a1b8d2766cffe4bd6a695a4b7f5aa60beb90866e0e193ca6a1236db037d46a1d4f44866d2c274c2edd157194efb71" },
                { "lv", "cb1761bf762fcbf1cdd21660702598735b5fdc805b7cd78f3d452566945860690bcefdb0739c2df8cb46966e44a6eabb50fe565da05905a815d5a7d7e60c1512" },
                { "mk", "87a734612e32b3ddb9a40d0f0d0d05e96d3b76eaf4f7bb03f01638bb857fcd20e24236ad7eee279ff8e9653e16bc9fd2b77e546cda319f27f6b6700c39a4dd83" },
                { "mr", "3326e2ae0cbd1ecb8c1d3bfed88d315961cc9c1b879c14e2fb41022429c4013054364c665e7e38ad0062a85561a04a2335a5766fc17d2996e1b3ea3a5513e74d" },
                { "ms", "e7c720ec4b6ba8d66696a90d22a883289df6a05d16d8a6df7e2029c237edc0707456b451d600d0969a531bf50bfe9a85da264b2543640451ffdfecb3d700f91b" },
                { "my", "3b8d49b42635eedc457a7274b78f252f2a48f7dc75b77113f1141a5c0507efc83fd7e83aacc0c25806739e009b84fdf50ee1167d2be651a2662a2fb405b0272a" },
                { "nb-NO", "7a65bf7754b17afeb5deca2ddff5e30039e548e34b922a65f6bc0ed316590574c7157f0a9855b1d4c95cdf0d3a43c58217fcec204322458a353fdeab10cc8f08" },
                { "ne-NP", "4e34153bfe63916439bd6fc566d8a2f6ad24d204e28d721ca02082c16b818c8c39a30e9a649b9e541639d66dc9bfcf16fcf8db897127a33fb7837d7c26b8c6a1" },
                { "nl", "76ddc57819ea72d697c358006a13bd2fca78e727c9c2ffcae3b07a2de259202496030a89e07745cd32f43c13d00d6abebe839de78d446a8816c1deb957b36bcb" },
                { "nn-NO", "de6d1545eccdb23f4f0f4d02121938cd3acdb4bcd922ff26fca9085133e67a750b06b08b7d057bc18a5ddd6d07e50cd023db28f37705fea067f23b91e71afc6a" },
                { "oc", "ec35b3588c551c8d0bba60d40c97c1fc0c616513ca2cc96c5c6cb16c3527a278e4d3f64623260775e4d30819760915da8d5a3bd1f50150db07df49261e6afcab" },
                { "pa-IN", "58cc5541b3c896c55d2072dd6ae36210e0672f38b0cd14b7ed4a4c5284435b6be45ce4c9a45247a584ad66e6fa1cb2cc51f5145683c43ede1c01d9e1046941b5" },
                { "pl", "faf54ca1c262a40d0a6e668d948727cc578504697c1433f590c472bcfb5a8db1b1d165f39a905c5d707dfbe511e20103220373bff7947d04f462b3ca13d2ebbe" },
                { "pt-BR", "f5f62a93451fd3d021577543827a1f285f4e95f114417868f153ff2f15e230817fdd6f015cdcde6b4220618b7829a2f5861cbed7d8f2f11e201ce2ae85716ea6" },
                { "pt-PT", "ec9b09c01337a24018f4f4d464d8aeaeaefe378c6fa1d846855f224b859f41ed553df4a907bf39f93b904fba84b324400d102e16bb35aadfcfcfa29fa990c0a4" },
                { "rm", "370210fcc67804a741cacd1c8aeebae30b54be5405e53836642f94e06edb2aaefccf039f5413ff41d6dcf990fc2ad3eeede34817cdb92028e13935ef09a2fdcc" },
                { "ro", "2b0f66d7240cd9f04ad71d26d072826aca8daa0c80ab195a9dd100988e1d0dd2078ca929cc778b1f60e334c8a00a3b9103346e34765c704e3e726dda8cf5f044" },
                { "ru", "3a370444660c26445a5c0adf935d3f179ba28c9a6d53c24b6b2e528d97ee0a238b8274f0e4b114f87174356dbd6b4f5abae5ed67d2becb27f8f6e4bc3f52f69e" },
                { "sat", "d4bdc7eb462aeea9b5f5f81d4007b456fd26ebcaf3fc7dd1a8b9902f6835f7d50b67d09400c37c7cd30eb93d22d9a95f539422bf6235725821b5dc80a0abdebd" },
                { "sc", "dfff2d963c1bfe50a717fbd65fdc66d59738c4727da261bfb78c185c000a60b025e9963d6b273f630401b6dcba17d770fef22d132a9452f3cd99a3033f1bf196" },
                { "sco", "717b93ed96e4c412f8dad0a6487051964e77eebc6c2f515b57207700e127cd5382610626118155a8e236717f200b3ca1b98b9d57ea6ece9adbb66e86dd38013d" },
                { "si", "e9c02be0ac91f58fc29a729d77af9907f9129f5ae58ad1e4c97b410f4fa47a7f83ee557ca3afb108c0c6f4f81e4d091ad80046199279a38fe2fdeec3a1e8a95b" },
                { "sk", "0eaa1311acbddf0c5c5af1834f160aaa0dfa5f9293652e43f9268375e17df91a7e718b253c368d4262ad513b1310433490dd0d29a4fd5481faea698e519a5d01" },
                { "sl", "893923d93368f7d51ebcf88e111aaafbe0fc820670b647389f77b2dddccc95738613a446cb1fc6841691a5d0ae55612753134be4b77d4e4c27373ce49cbb7d17" },
                { "son", "4c5ed4cfdf33b33e0c8894eb9aed25f6bd1d1d82cb7db69b4a4eee660b896147de1690ca791d39b7a3a7d2a2c5cf0f3e2e3f36796138b5aa712d4ad065608f1a" },
                { "sq", "c33498047b21fdb7160b801a85323abdc222acb5ac5ad5efbf083e424878f34b581dd78ecac50b5add9f00d093d6c2d7d197cb2eb37c4687957414023bfd14b4" },
                { "sr", "c1f4638e485ab564b1658e269e23de31679bf58eff523fcd0792a888c58b98ddfcb5ff5a83e225b61d72654817d3276a063b656f2e88c8524cc3469d44d1fc8c" },
                { "sv-SE", "961daa965a9cd069312b6c09d5e23cbc61c21bb2977d429dfa8fabb3c071a14e575754fe3a9e507befe0044f77cc3e8ea7747a5bbf5391dfdb7641dec94eed96" },
                { "szl", "219fa0c91b9303125c78256aead72764f1338c86404b1caed34510d76e975c9e4040702c2ac321c20056943d82b91dcfb8a3314834fd0aa84005237737a1198b" },
                { "ta", "e35095b93522192c41eab5660deadef899dae2de28eb269e87847d29d55ec5f88d6d1b3ba4c8a3fbcc60d7b7e46f63683f07eb09500a1b5dd264c39d1d6e5349" },
                { "te", "9c6b55c7bed03a93387502652291094a315b6fa4aa53e99a6a1207377eb810f2341ed28becf82f28b370625c16c84f4748637e2ea671d314b21a486205fe7778" },
                { "tg", "7879e22cdb2d94b772c84b2764dc7d05d32244b7b396471ae626e8697fafb1db70e80ecb340a28a12a1cb412f93a80f3c343b895e73495fd9e41d63b39b2a830" },
                { "th", "ea6c7ea8e5ff0c0547426df5d6c5ccb13989e39eb8567644360fc76954f024ddd9f5d5746cd441c69393452f59b81f9b80b117a8f38d7f96c4a4cea4a9642374" },
                { "tl", "c3726b6d3c6fd217a9f57fc7ff240f90fd4bb99bc1738891a82adceeecba8421b47a17de27473623a8cb7e9a14a2d0c547805cc062c418c40f98c65ace87ed04" },
                { "tr", "9559f6fac40a6c7d791a082aaa03aa49ad8ce01c1761701e5317a81326462e0557dec11479afbd8e3ad0325552d90599c70504503ee22471c079f278d85e595f" },
                { "trs", "fa2f36ba3d64ca73cf9b2fc8f61713e1ec26786f97dd7f144cd101d9b05e49419f87433ff329fe0175b509bd4ee1d4e82b6ee8bc003e9810a2a8c8812cdae885" },
                { "uk", "a903c7d2f110bfd0c600e313d5956ae7a57511e9d279702257690bc2d622f2219bf3c989e3652eb337946354de19e39358a1a9dc8f80ce879039819d4b2d1ac3" },
                { "ur", "029d51c415e3fc51c7f044211054b614412da85479a9b1558c6d967456426aea4201301cfc098b419a32fd6afaa25c424e27e6c9c2f68a3cc47744c97a56d7ba" },
                { "uz", "7206d76c2daba79ccd167f9c9740c0dd164a87d728e9e6f8697a6044dcb8cf49635c7b61f52509b2a80cc1a103029b573f122cc177f9c235f44bf21a9ece0467" },
                { "vi", "f6e65bdbfd079663149910763e19886f936cbcf8d509f3a385433956c3b125b69143a0d59d92eab3340baf308589ab7f51a59fd185c153121abb4c0923bcc08d" },
                { "xh", "11606ad5ec30f68e8debb8eb0d82daea899062766e3f1a8a082ab0f442b10e23d02b463560f31977ed2aa635bacf0894ea03624a087f8f1586c81f028dec4746" },
                { "zh-CN", "a755d9729d0c7537d33c7b954004f6f94cf598bebc8974828ef9d695410dd87ce28afb8f905d637bd0fada2ac5ce13fb170d67462f566fce99e46cbae6052bda" },
                { "zh-TW", "2055f889f1ce761d0836d266d2971834de87c13ae653ab2a0505230bc8ebdad1067d26ae13f1016cec6d4f8737ebf15c54b3c406f4a4ca6bea7efd574e69bdaf" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/126.0b8/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "6fa7818fccecf1637f47636438e7da0d4432384dacf3aebbe0a5c6d41bf2c6f37e218dc527c1f5c4263a613f9df5147df81900d68737ea0900a79e664ece5eb8" },
                { "af", "84e04cb2dd971e9d919e622e427c1849b1d4d10463c5e4e12aad1ae57ad274bee2ff1449f9ab7b2a902970573699745e598781fbb51936d3c1217406bf4af25a" },
                { "an", "9e5caeb62c6dd7e80386493d26498c3bd7f07f043d59d1cf4d50eae0a2960e37e480d9b782f539179722c78411e40e60964d76c2f1d9acc6b312342200f559b8" },
                { "ar", "0a945c413c963570305c6cf071adfedafa0e8a244887789d84219206f059a744f8141cf24f78898767fc694c3b67ba1d7b7e2f5f02782051307b1b0eb7fc101f" },
                { "ast", "6c168c5e3dd1929f1cfd2b11d8bf19f22a345e99235dd151c3784acdc10dbff6501c4539e5236207ac88bc257b8c893b59ce66385a9db262437ed746ff291907" },
                { "az", "23f79bfe449261b33fd98e063b346378ef207f778927e0f436c39423187bea749ebcd9c950e3ed24f3ce68ea658ada769fbd3510802399c3002e59c2d8e8b6fc" },
                { "be", "85285b462b0f50e84ed36ed860b39158e918cf8f5387c6c5ff9180c958a3ba851dbf492e1439a5bb396a183081848992ba5440f4bc8ba9cbe38a5f71bc0dc01b" },
                { "bg", "4ec38c4b75cb6452cdb9814aecebc06ea8712fedaaa80923387b4c0dedb2d886e0e9aff723af75d7a1b157eaa0be2dc75dff487c73790e950cf4e6a4a23e6a1d" },
                { "bn", "512dc0f92cfc25cf4a929370faa1e5291cb10a2339b6a1e90732654ba5c5f3533611e699f756cb40c127f2900b0f02bcffed968d96b92bad48f75f214fab3fbb" },
                { "br", "bca74e50411ca5b92189310192fe5972d32840f6acea146ceb748ca68475242313a163b7a27428559fedd2e728b89701a835a54026298d949fc7c1af9b30ed82" },
                { "bs", "1a8ad3d9eb58992b0ef6ef004ecfa885c99abdc780e3a0e7c804f9d9e63e8deb183713b92852daf3342416f5934af3f0196766a8016a6d56b43ee220747ceb98" },
                { "ca", "20d3728d3b67845f9a225a9bb4e2663931f8ee514a040fcadd61bdf1fd0d2c96f8b9b0f13e9f20b258558f10ee77f2b19c58e63011f7b616dca2e1576479ff5c" },
                { "cak", "a2edf4a89b77b66eb729d24169c63a971ba788ecadc719f8ba77c7b54a84f54eab1cd58e456e433e889f3a65e502667a6ca9511e497c973c666c52fd29b0b737" },
                { "cs", "d14cbdec8843ffeb9154777d9eed12313f620fad7e4cf77c4c906309111cd7c104fd05661ac29d566eaee16a34b25702c744cb4af48bb4a6a0c7adecde9d0a80" },
                { "cy", "a28ae40c54718bed28119c847aaad894df04eee44a5289a33a7a22b8bb85313d1d74af602a4273205404a8b6a599c9cc856c12f0dd6bb716034427a14d4db1e6" },
                { "da", "e566ad9c23fe2b0d47252696d0aa8f3b6dc4194eb841b49c690618f0d070f1b7ce3323c6e06cc20a4f5729efe9d0e795b141a8e73f12bcd59b5197817c71f457" },
                { "de", "343c7088942e01a31b7faabf6f3be1a615da4fb08fe2bb27c244c07e23d8b8e9bf6ded394443da98edf0609fd75b852c279ef6a5e4b483968d6756db6ea76ec6" },
                { "dsb", "efb8b3b15693cd8e74abd73ea3c75d983dfc07ef668dac5d2dc52270d97b863d2f1448dfa123caebc3e5ffd29d72761e6bc0390dbd0dfcaaba9dc106309c9c29" },
                { "el", "6f4f45b009b88a185164faf461c3b92e19cbcd50081923e7e14d5e56b0cef31fe9bb9518c3e6414079215d718931c78e34ebaf6cafb488a418c084ad21d1df38" },
                { "en-CA", "075aeeec0a6180a6bc760864e349c87dfa45539a768421bfe7b5382a2cf5c7de6b44162a012481400aeee0b6c72ca23112f38559fcb48a0ecbe9cce706a39075" },
                { "en-GB", "e52830976b231f72cbdd88bb3fad91e97c74fce3ea7f1f1120c4f830efac2cc9e736a459232828f3fc7f6865edb7f27712e6bcdd928ecca5cf947636e7e304ee" },
                { "en-US", "b22ac4b3f18b83238f8b5b8dd1359a835187c4619f898c5b8ec2f665a694531b0a173be640dd4d53d3db1cb0bba90fd070ac67b998722b7485453c422446eda4" },
                { "eo", "5fab421663f6774c0ccfa3261d62a494900a6546102062a46894271e607a437772319d2e674369973afe3238bac105e2bdc7a4c1d67fe39534c4d542a1ac3ee9" },
                { "es-AR", "5cfca53b058ab28173c80c6257d526f10c76c52ca990d2836dbb4249df37af0c8e199ea50fa636b13711651447cb4eff659167714d14510d3de26268c2d9ac72" },
                { "es-CL", "25b9ba4f098d2c3ed7b8562d300ad89e452f80f28688d5d3491dca1e924e90f4c77f402532b6289844eefa430502adcd9b669f462a84d047a437f45b2822f34d" },
                { "es-ES", "b05b3e2c6119a64d4335242d8bb2993695feb7617170bf7e1f1ad98c1b4faa74e0567e1f0d753e8dade027561c52abc94d44cfe7c916974da20d6b0dc4d6fef5" },
                { "es-MX", "304d373a2f34b4b003042631db48b154951cade8091d6958ca5f8b49afac96670c7b438776215e87588364eb2a7877d1569789d1eadad725695048c617617b6f" },
                { "et", "06ebdadd8c28c379ced99d3cee68a4d23b661a31b52bddac6037f53542f7542d047ad876f1158ba9a0f43b652e037aa91ed9a8833a2491359b6a9ac28c187e73" },
                { "eu", "2addcc9266b82a18bdc7b08e0298952dbb50e883144f2894fe8e75312d3d0b471b959c7dbac57282d1ad6d6bbe0df4a04035ca726e3acf1f994a73f39b047657" },
                { "fa", "c352d3d7c7ea759f1302230c42a203e582de08212b1000a89a59902bf7e4f73fd40728a98af94d1a57895855cd92c1c47db6dc53fd8b514ea2f6c67cb2d15f63" },
                { "ff", "ffe8c0bc578b846a7c9566e4678c463c0a9462f022b9a0337844eebfe9efd7b620cab7d28d61a1e1cfda89ccec0da80396fef1c13b384bb79ab9c611cd21b693" },
                { "fi", "af7e8807faf4056921dd487d2c575294c796cc3c2e035b64e959fa57b8d11fca200282aa0ce362de92992aadf8aeb494af0954f06c4c6e52c4bc64ccc2358764" },
                { "fr", "644dc2b44ab983a9467bb660667627208d76db4ef246ca762793bfd20d7311c2d339c06b12abb7b2d2fbd7c13b8ae69ee622cc9f13c722f82488fe7633fa25a7" },
                { "fur", "a6287a1024a4eb126953b6b75703e2a7d55217970007816d1516b7601bf2c50e32201ec1770e18e9aa56d99c5296faaa2687dbd5ce338537a58bf30f821b3f8d" },
                { "fy-NL", "5486f0b024954232a6b908e78242abfe6cca336d36338517b63ac0a905d2ede9f3688062be887e935d060f6458b23a68eb1fe4556b562d1eb169f519b1d4a456" },
                { "ga-IE", "c04648b360b54e86f34991a8976495fe605d2f19b08178b1bbc31f13a8ad89c7bfbb8fea91645c25b68059bcbf9a76e64717eb8323b681a2f13890dd8e2ca7d2" },
                { "gd", "4468e2ba0576e0ee79a4c5840901f48c7bb7d220a10e1be63cd80417e269105fad45b2951d8e2da90863d56bfd84b1acc0c2faeeac22cfa5019015832dde1fbc" },
                { "gl", "718bc5f2ad09cffeb15f352f3436ed169cb31aaa0271d909c663ed5ab549645d85239b7cc774f723ec97fce763a29d966ec5bb9a84c56b522d33c7f5723cc978" },
                { "gn", "adc1572c9289e9e3b203a6d5159465361f8e0624c2569a9a34c06b306e0a79a09c11251f6a840e9e17ba8261ac7cf50b6dcc6dfc4fe540d062ab26ec0faa4d0d" },
                { "gu-IN", "55f7d862122c4813d6af187b853f525fb27aacf72cba70a798145b885481b91c1b8528fc64e35aa357c7b4baab7d7ce0136ba4f5cacca121a1507172c5d69312" },
                { "he", "79f541129dcf7528f882427e8c90eff4afc7ab22d747658dcd42f348839d29ad4804a2a9f58ec2551d4cddf40820de31d8d98ce5bed351dd28182759fa6310a9" },
                { "hi-IN", "97a02de182e4eb962babfef1fe2a590014bfb0197b47925c14697135c16b95d4b0d6e9c77a06fe8f4fdf7ebcd8b78af1b74aac41796eb7a5785a249a12587beb" },
                { "hr", "22bdeb94cf9109309155887fbda1cafab3373d52d0c9c57ec01cb29d24b08562d620d3f6d133344a429c497d8731507666d435d856a0f09dc2a2e135d636be92" },
                { "hsb", "95e81e79a10f05366ecb6671cdbc715df57dcb781f9c2a44c5b056b62cfad50464da53051a9f92f9f781b2844cbcc8ca78bd8140256c700412b125ff94cf457b" },
                { "hu", "878149359b3d73544e47f0e102487b6f4c43c64e0971f6c619698aeff51cda1eb25667e9d17b527e5dd852569a6a0b64229792439aabe93dd1f77bb569cbfad2" },
                { "hy-AM", "fdca9b18c5f55d6d8b4dfe72225136b552abc81930d1748f1b758af0aa02d29edca7b4ecf44bded2c54b3d59613a938e1f7f495bf1dda2bddc04ae9de8455fd3" },
                { "ia", "ca99af224e94fc2f33eb3c828f3ca731f3ef41f7e8fe40bc78ed2730179b70c5d2855df3f2d2afabc61428e12e9dcd1e5952bbc5cd0bed1b8e76201d53e65a2c" },
                { "id", "4fa5bfdfdd63a5f0c7525530fbb3a478b1176f57d7b4b3abdb2ded8095d8b384ed9351192e10f8ae8937612731e02ae651f4af921e757ffaed83708a5b1f1011" },
                { "is", "1aabebb65416796f40ee5cff84d16c1617132f2b140782fdceb200b10f7f6bdbdf6921a2bc1678e52ba5667b0731d277374ae7a042297260d14361fc87e316fd" },
                { "it", "b968f871d5b5f841687b9e74730f4bbd88037d995f17ef2ef9b954e1a44e9a436d996893eab71ac91f0d97876f44e2b6436537cad3efb05a7cf95eea7c33f9e4" },
                { "ja", "75546989592cf0c50c0ea6ae1171ecef34480f82c3cc92aa3c936c8b6902f40d7438963596c5ae86015cff91ec87ca5ab21118ddf6a8bf833405691a5987d807" },
                { "ka", "77a8b61a36761bcac25bf014db2d2fdf477e838d3d4b072fd148cf11294860cf3f8e2175c82f3bd6b0ccf19c1913d7c8f6e50e8e9d5f8bee43ddf558e8429dd8" },
                { "kab", "8debc02e726a7c8f82164a52dad5102707de1630ed394267c0b7dbd7c410c5e38957dc9bd501efce3f2309cdbe5f4558200b2cb3aad26dc7c4583cfcd601fdab" },
                { "kk", "83e1ffb364b2eb5372ec254d58309881046d72e0e5eca538d29d9accf80f1819fad32c48b08c39cb5e954dcdbc03d16fb36aa75c96bc9b0bace51903c32461e8" },
                { "km", "41b7fa5b4d6a47f9e657b159cf2e1574087e3de103e21560fa3200ae59e21509b36053ecd1eb7aca4972d1dc14e0d9a83178fb018d0aa79c1b1bb16cd40d4325" },
                { "kn", "3081c096856cae16f89e523ce5a7b65341eb6688b6abe37887c5b551cc723cca42788be9688a4bd4dd012b663e84e99781c3730bb667f5e1c94055f1bf040c64" },
                { "ko", "8bb0509b0ed18891b27554b4bfd78bfe2b9757ae1e1239801bcafc5f3e2466d570ec21e56fcce51d496d3d79f572514fcb39392bbb3a6f929d9e1969d3fb8589" },
                { "lij", "28d83c349861c1a211de53885a9fab8b18065f40ad512053596f9fd5dca6c64c2907c683231c4467db96ab8afabb578b5894a20d9de1b67fe4636f1e1fc91944" },
                { "lt", "2c18ab781a0164f341fcc55a5769758dbb7058f796fce02216132aae5ad4e109760f559f1892ba62994dc284ebf6a610624f2b0c8b087a72c91559ad374f2014" },
                { "lv", "c10deb4a359081616f5bd978248ba848d2087ab0f1a6f4c8bc4def645c2a8522d41cd73d29f27704b1bf21617fe8499fbece6f981a291a64554f6c2f95491f96" },
                { "mk", "66db5d3cb855e8337706373247cc87cb03e6284315512a45a64d52ef92fdf35aced9a9842444124ea21e0f5d707da612b370eb255a6e46ad820a86e357dcb5c3" },
                { "mr", "ce337b82969a140dc255394868bf4cdb7befe70aae8a19dec881d6d019281fbfef6524450bf75e9e21adf99e9b3ca73902e9b15150a0bc8cf28c0b51eb9015cc" },
                { "ms", "1c0a15a56d14f6fb32114147e4ed951468a0d66e3c9c55f354e1678e72bc4329e74f27437ba78dc3151519d222b3b730c57b7b32799db68a8708b032fb5c7bab" },
                { "my", "775beea02e1391bee1c1985b8f14dca4c1b25d06e9eab4394694d7050896c0d23654b0ff925b2867ef624876a2808ec85cddd9f35eb7ed8d37f96874749ba53a" },
                { "nb-NO", "164922685efed76db6612a79157279abe6ab8188addda136d599677bf17f8281ee9d1f0bfafca93aebffb72db08e283bfb6f8c30d7cf15093238dbf23fab339c" },
                { "ne-NP", "21f4e5a9f114bc75585d8f5bb807dcab3c944f15e0ceb1bd4784fa23c896bdbf635782c21b035a2de18dab53a135c00398b8932ff93a66a671511cd4b87f3e62" },
                { "nl", "50b8abaa32cee31c0c04533048697d1468b45afef1bae10a22611af117fe9c7aa3e856bd88e3998f80e5266b6f6a75b006b0c484e164b8a840e42e3a7aaf695b" },
                { "nn-NO", "58337b9398e127cfce92fe8f2b2871ce9ccebc7499daf2846c27510ced7e21965cea760b58707d8dac406fcf26edd846bd7c92f8b58ce88ddfc626340cd77101" },
                { "oc", "587d21d5eaa22791c9f6c127d8414229b6546d612b570f0ef99ba7bdaa0331777a6ec8ca65ac68828a83c3340c8723d608754331b14d98f4e957738208c32910" },
                { "pa-IN", "ffa2528aed5c3a120481b07279e52bd0528d27953c0a4048a3c52ff0a85a2d601150f14edecd8326cc271e267f22eaa712443b3396690780a0201d32af7db755" },
                { "pl", "921c826f879c3339536f622924c2d3dfb4818166c2f134486a23b090eb2692fe8d19ceb0f755a3fb78886b8254391998a7ff4c862002a3ebc85d6edcb5b70bed" },
                { "pt-BR", "f08083464ce8e28b4d47f7736cc9bc60266d945e3e5247c543a6c23a73dd3f8dafc13744d270c84650f751ffc1b1da3161c69becf8af4803559bbbd6fd216bcd" },
                { "pt-PT", "d7e389d1d6c757a3bacec173cde8cdcb11890e7bd19d2c8352ae4997ddb744ccdf86f11cca73d62cfa4aacdc7776ed905442d4d76c9c80999a8b5e25d2338c40" },
                { "rm", "b1be4bf071eb7c989f0284b41ad150434c5d10255eef44624bff4263cd993b4e632104bd8aae9e9f2e69d24612ae0cc4a9ec54102f08fd905a1a6b203be409d4" },
                { "ro", "c37ff6b3d96a0b178044de3b2846a125566ea589a5c81f4719b3d039e2e639855a1c566a7d7594d7fc4d860fb72639de0f4ec22bf08beb869f306a3989660bfd" },
                { "ru", "fcbcce76307f0672102786394ef4f0ce40e8a82c5549de2fffd82d2f6e5043ab8585c10cb2adf19d4da8b0e1f1bad9371c4ad16627819e073b13df8625701906" },
                { "sat", "3f85bb5f3971b8b889289703b71e3bdd2c38bf7ddc92aef4662751e77e25f148a4ffdeacd7bb70b100a7b77e02db3e0ab053bba838a8fa31bfe11e0ed4dbb2c7" },
                { "sc", "b0b592e51bafc3c2dfa6fdf1fcad750a3f1cadb3566806d2ca16a6bfa8659ee51aa51367b08aeebd397f0972c9c61fb4f91f7f60cbd572d328769cb28f01aa6b" },
                { "sco", "dea39afa4d7fc5cd18086dd6831b3bde76b0f816343688cae444c59b8bb9f05092f1237efd9a63af032d1053cf76aedd3e935148278395b12d9b1753d04265e1" },
                { "si", "573b4d4aea6a96a8bd258946cfc87f386dc971e6c0c9a38c80d4f244596abb1a2614794d1f0ef2c8444ed6c8858d076b0fb494152fcbe852a13c3ff0a99e37bb" },
                { "sk", "c5b4956b3e323dad691466ec1dcc7d5b891107bcdc0c42de1511c60a6dc707d52505576565615b68574825c4e9019081f0cbe82a20c7ccb376d5e308d5b69a09" },
                { "sl", "b29942c81a682e46ff1072ff7a7dc15dedb478d5624b8014734782f5d597382f9b636730a8dc22914d54d1ac7d9f3f4363b59b9779dcb5f97b6a432c50899d00" },
                { "son", "2637a94a53ce5ad5a4e269144fdde052bb57a0808242644f48a8e668046c467f7607a1d704b4dce0530b41979aafe4e331debf584e8f2ab384ebdd8d2e88a8ea" },
                { "sq", "6ae412176187509764fc7417051b821266b24a19102b4dbafdc10e6754f7bf95907429f68b3b5d198dfce74bc0d06b008db91f55c8d443a02f2412f1e9d9c8ac" },
                { "sr", "b159584c74c9a42f25e4378e3207f56fc66e12705064d48ef34373ae20386afc93229ad946f3dde9dfd54fab40ca0ef8fcc62c29aa2ad7b1f3ba0c9856f0900d" },
                { "sv-SE", "fab7df6ab14587a4ba0fbff5cd328f03cea58095bb4cee30930037e570f251cadbaaf49b5413d004e255f31572a192903130f1e969d262957669cf25ce82d0c4" },
                { "szl", "d5e6efd871b7e450f6b3359757e60b202de1455b01a32ac56eb6e83af9c56bcdd11c0cc1c99810bdf106e49d10e81943b6baa4d62ad9cc245cdd5d81260a4004" },
                { "ta", "f9a48f51d3e4247bcd2c0ccf4936a709ccf202895d493042b69d5859c8bbb2bd532ce85df818aa280058965c55e4ae371c6c2234c55133ef53d8d83edfe965c9" },
                { "te", "6ad079d84ea92495e61ba6b9b56280318ee0abf7a53c2af2590cdcb049b3dd204620ea3d0e6a8295cfd580483317ac87d9665276f133577bb01eaef08b4bddc5" },
                { "tg", "1a0b0ede899dbce9d7b39ffbd178299284f705da13adc982b1e330ffd7450a6b7a4fe287fc7663311141271c6e1902935b17ff0cb48745915327b1cb6e23f8a7" },
                { "th", "ec01ad27247a7e9449fd2ba555b3537b57e8db9ae15423c7396a25998d45e8b4bf711284e9bcca4b7cf353d150595b24761ac88e30ce82f9b4ee3fbf537f79d9" },
                { "tl", "8159f07ced0b22594dee1234fedc05aa126e1da258366a1215d3673270c9cc3b78bbd1a6f7a54c1bc58ca026cb5347c977377abdf3ee2babd9941bcf87a3c476" },
                { "tr", "9ace13d7c2080cc7315d87317624fb9915d82d4f1831b6bd5d19392de1ba40f2fb0fa41a293c1b006851622f1601c953326df6e0bdbb8b8c69ca4d092abda3b2" },
                { "trs", "3d5ac7079a77369c71a371b253d4a06f37dd80907e0ee637bbbede3dcd8218a162f2728226608d0ee9efdce26c112f07ee9a5962f2bad5fd6fe20d9a1340e5a3" },
                { "uk", "43c69c39a1aba107842c0bc81c184eea98127b6dfde2c28c07a96a5bed93b7872f9423039a27d0d0de8a5d2f3db0a00e3a30d577a960b9ae17696e8f4b7403bd" },
                { "ur", "6b744dfa95c502f7cd8cac4ec1c6900b32d9586840d6cb6dfb4e5081d034ebba2e5be3158b1baf10886d91c6f44e852a9937d1e63b5d5a4d9017e8f4e160f4a3" },
                { "uz", "52dc62efde653e60a4b56cf6dd28cba8460d7253cbdc7254cf1554d36167bc12e94d4ebceac51da83401e3429b28681c90fc1eebc6c3234830c5310073081f23" },
                { "vi", "c242807f493a50899d38b478befa1f4a13bec118bad8eb422c2aba4a0bc13684671a0984a0a81812ed517c1ac39563b26810cd8a88e6c2efe56957be7dd98e84" },
                { "xh", "da45f1cb8ab8c8a2b3bc8007e00b31a23a85646bf8a14e96f8914021acadb880a051a05c3e1eedf0582a27631a23fc07b36737259698eb0bc31e9a358a4d501a" },
                { "zh-CN", "82893ad7a2cd37cb47d27ffd620508eeba758d6a9919ae20d038b0d6829973ec92f117f5161b28ac276225d09c251c88192926328e37f5709b2707c045388c51" },
                { "zh-TW", "4d0fda00c7ebc8d5ac6602a22efdec75e30f58d53f2f79cf8fcba03cc2d60330411af0dd5294c754c163586568511a204c1146a278d5b1d2eed04e235099986f" }
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
