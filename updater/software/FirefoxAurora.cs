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
        private const string currentVersion = "129.0b5";

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
            // https://ftp.mozilla.org/pub/devedition/releases/129.0b5/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "da4690b3bb3012807af43147f745006501fe25362600690e4f541cf248bfc290f6296dda48a113c2ec2b8c0e1d0fb9ffd2d4b9de80b2d4071d5d1da0ed6b150d" },
                { "af", "5ed869b12834e4cd502a05a4a01c42ca70a4737b7c97969f1632e721416741c27ad82715bf6621f7780589b991b8474a972cd5253cd832e01fc743d3c53ad7ac" },
                { "an", "0a5e896b08f98a86ff3d6085b92669e67fecf112fc7c77eaba47617de7076fde4a2fc7c83a21601e5b46286c74356447da1513e08c723a187527087995e9b2ef" },
                { "ar", "ff53c4b7e70f313085a07c9fb8d30e21f54e9ace8845e61c69951cf529663a8ab468e458e592772d75846c7046c5f11df768e7d1204e12a3df49bc17426d2382" },
                { "ast", "fc8567aec99addcab7badb9fa501d956694c917d13947ffb019259963aceff5a3e5aa0a5d4edbcc1e6b326644e42d53ba131c2368df9ac28642480ac1f7e77e2" },
                { "az", "6d19a495302648b5d48cf072aacc785b03f7af7046e2c39d3c7d6b92b3da24263e992222c28fadc5f67b9850b6bf9e0b8dfd2d78ead4b71c916c1dac3406df94" },
                { "be", "1d999eecaea0c03fc9a7363112b64c9575f8bb56216dd50e548919b43eb66f9baafe75ebd0c3bec6e88a672aa9e25ebdc993694ad235d0b29014677c6975a33e" },
                { "bg", "30a96367b6ad7787c35ea0fdcf3daa2fc94720c3754d303fdf1328628041cb4ebb26c0d895b61f965cb9259349c0b863508ced46b3a9a503344fd53c5a277582" },
                { "bn", "8c00b2fb151c9d24653a961372f7c9a02c9cc35b74535c7251e5b3ed3f5e709488afa05ebdaba966667f4579639c3c431df17d9f1f6568b5eabda42cfde85136" },
                { "br", "8d5edc4fd6c929c210867782a1fda88e235812c786e6f36f5737d37e44036252d4332bf2fb6df13a1d78c45f6c940c6602bbc279eca83b887ea2a2761d811a11" },
                { "bs", "e64e260047406ef0951b4d99ce848492057a9974fef2bb33f97e40f3f81ae3ec449de9d951c3173de12a077c0a2db7f056362b4d2e428443bbd28b0d99db381c" },
                { "ca", "cafab8336e72a93923f0865468ec760ac406aa8f746acd0df2bf78f0cc05b79e148132b81659de61c6aa33dcf61e71424e3650d787ca2ff428e3bf1ed654b12e" },
                { "cak", "2b9ff7e6050a33937494dbee93cb3112d6c82608610ef533801da978f8c9bc39fefcdd51d82dd1876fb53d5eb0505b5438e6505ced721bf77fb51449315cb152" },
                { "cs", "409a745896ec00c5af8b145fdd6469906770d496331ec42dafb8500ff4056030353acb61b021099835a8cc598c78304ae70249d83e780f1d4eacdc968610c081" },
                { "cy", "099ffaded1f7e17f81a8e54e9af7b4965258210ce1e07e78af71ca3f2ad446aacf6c9ccb6971a57f6dfc6aeaea4654323fd9e61888aa1c5fbadd6e5cd507d6e1" },
                { "da", "010ca2005641a0a131513d34e117293cc39d5468c9982675d709b49b95a4b073475ba950a35e953b0c7ebf3302f9b47b52a8bcb44da90a340e7aaafa2ecea591" },
                { "de", "df1aacc62c13434be5e3456f5554a572dc95f2e29b11ce17ec46b44ea7432fcd415a13f95742d9622b44ee6e0c98c6734db6f92bd84a34bd456f8adfa21ded87" },
                { "dsb", "9cfc8324713384a7ef1749c1d982c07208e0f7f5601436fabc3ecda8a4698c69db340dd74cf533fe986e1902ced5eb76d4993902291507090d34998d31d7cffb" },
                { "el", "c5d11c345e140d9a7010a93165d254d8cc854d64de489bbf8a605af9b280909a1a40e79fdecd8ca84f99205d485759a947c5d9dc08082ab0f2a104cb034f295d" },
                { "en-CA", "e70a34d371021f7e8c484be4b0f01f459dbcdc4718a4e29ed8213a8382463cb7572362454ad1adfb6c62f4a0e4bcdddfd175b4c62396dff7dd66a974ebfe4233" },
                { "en-GB", "3b2a6d10f394e7be1ad30c6bd70c4a2ea2ed75415bc30d7478a05b2cd110fa1f0276ac0b24900ad41b187f147883fc5d29d4b688a590885223ced0188fd9050b" },
                { "en-US", "70fd2a074d13651e9199aceb5166b8694570a3d28d7460d2d9a69deb1b36dab80d8973de52140b64c6a0db7496dc448a926ec8c64465586326be75ded7116a94" },
                { "eo", "db2b84083f2113b02b7209361f40fac913d608c3c46d4b7e1c348763f9c3023e9cbd094efb8949144183818988a76e8d70378b2abd6e7260509a91a719d3cb25" },
                { "es-AR", "e084ab17a2407737a2aaf12ad082280a7e62e33b41db26b405ea7512ed9dca32133b4af512365f6001dc35120bfdc92c31f54b2dc4ebac6070d303ebadaf294e" },
                { "es-CL", "7fadd529e40c9d6962061f5d13f7023cbaedc8328068485c327fa8f93876b74882a7bd552444be368cd6d77db313cc63f050786a0114b03ae1f1ac43f2d64cd9" },
                { "es-ES", "d23e23bfcc650f24c297e64e3ab29f0c9c5123a19fc91ff4571527ad9a552880d0e4cc1b7b51360bd34e48a59d47c705c39f12fae440b61d4e7546047bd2558c" },
                { "es-MX", "22b17fcf39dcb984176ffb761da7742e0e5c8adda46750681239ee3ff2ce34070f6de6d4bd5a5d4028eb7fd7620c516ce59f6e484bb9394caaeb9a10e2ee1ddd" },
                { "et", "4ee0187d05311bda9d8d2caab74f2d4536fe01579338d51f10b31cb56fa337fd35befc934f579be0ddfdac6fb3c704d890fbab57308346cd3bd72c22a71da816" },
                { "eu", "af3dd4ae16e4814dc35e38b5e62fa552352335dc34837a00029e367168bfc807516f4d5baab69d5991fd32d09d430fe5d72a856e2ba7c70b0445c90f4f1fac3a" },
                { "fa", "63db5900f3e1fcc7bb0ad4c1b66bd26e48aa7ad754a2acba2ca4b4bea07e455a74d212648bbcb4d74bf2f0fb66065af822d7d0acc685d5723414891cff26dba0" },
                { "ff", "2aa14006de1499947c8fd442ed99af39cfcebc5b70607f18dbab73fe785c039c72396617c2eb68223e005d07d9ba510ca5a5065e40fa1caffd8571630251e783" },
                { "fi", "793bf37c8cf34628975909a4b84ae377ca7c411d6d57300256a26b2bdd70ac1e60bf2c116d54f215edb3bf621ed369bf08f1a758fd71eb2de9c32e8f1a3ee90b" },
                { "fr", "ef7e8c72d18f72e08d666230be25a642b6224ee895a763d830bba6fbd9f07e0ae85293cd5008ce8566a9bef7ea6291b1fa549194909bb8e48cf492750964cc41" },
                { "fur", "bf6f0d5e0198f3b3074ccf9b134f67d687bd8e418108a8edafe560d0f58555b5476da3d49025ae6232091041f72eb43c27ab47613f73b86efca4305133248310" },
                { "fy-NL", "66b92261f9747e4b0c8687e605cea8ca2e98b5c6ad766009fadb005f86b5c8b83f3e1a073ec29f380018203cef985c96ca449339b2690643a01280075bf04fa4" },
                { "ga-IE", "e4ee8597a8a5c95b6957f53fd0d33210c8c8a95139d4176997eeffe5404e4c2c97d7edb7898fe4bf4c03f2014aea791904c9747faca16c2f43fa53e015ef3b2e" },
                { "gd", "86827a2e699f93388e3cb71599cd5324423029c52a153dcd28924400da5e4b2cb3f3af49cd5f66164dcbb10292fdd7011b178fc8d758e169afc110f801870299" },
                { "gl", "0ae5bd1a2f651fe47ee8cc6c9dc39e1a761ba95ceeeb269ce9395aae7847c5fd0ef13778aed20e7082e3bec4657f0a2735eedd47135fa587d5821558c6bc5c74" },
                { "gn", "d7bd56f95ccc902f6b11bc5005667d4f7d76b10046f7c913ba9f6632624941f253507d8d3dc5b639d5f24a8a7e0de4e2cc9ee9e8c28fce0c9d1b6369e853a074" },
                { "gu-IN", "e724169cb94741829228d35a1d03152c3b41977d601482eb7a5f7ddda2ed4980c3465378efa94bd1631cfdbd4d160ced9f2e7d73ed6e234e0cea1fe5e1d8e2e1" },
                { "he", "b1158bf600d5b3a65b2eb40b11ca0b3dfe77146351cf5e8186c05c53c275df629861e8847e43222ba4057b55a6bc9955aaf6e804311d63f39ab4d4bc02eef15c" },
                { "hi-IN", "8d35e608960a7639cd71c41f2c693b8b6b039155d4912466740d3bf78bf83add1137554c8adaf2cdf5ae2988c02a8dc0bc0ef889e76fd7f4f4c3ffeee8477410" },
                { "hr", "7b42f4d6acffa0c0b30ac771f29c78b883725799e437df53a0e4578c83b9b9a526d7919dfe6acece7a444250ee2419aa1ccc8acf033f64b0611bb929c3d14bda" },
                { "hsb", "f3cbaaa937d083a97b22a2b1c55db12f9ead41d57df84486aa457a920921584fa7f7a4a0b5952a5055d56b2bc663e3fe08dc8f7841bcb104253f24b8028c9325" },
                { "hu", "e807e78766c37abb8b52706ae6bf3122fbaf43af26bd0bb2c844aa7336b12b11a4cfac77a987a87edfa0289cd987d08d73f666f1c011c3d79fd6288ed39ec68b" },
                { "hy-AM", "2f4690a30dff1eb8caf401db3163ec4328bdeacdb2d52a843fbaeaa74f716b91db88049ca9d36c5fcb2272d52f407dc74727100f43555d24477af5bc3479c5b0" },
                { "ia", "0f9b80a804188c55294fdfdfe1fe8c3068cf5dbb463991a319ab8467fc40efd012daf4923fbd257e06cab89b05e267530d6001ff1580c77ccc52d51ee2cdc45f" },
                { "id", "804fb5d2087f8e65d56e2fb2e78397c29e5e0e3e441f9ee45765501bcb6e994b407c78256bd43e9b554058d7ea4146730f9b17e2a16bbbb8e88fa86aec1913a1" },
                { "is", "b854c7f1f3740690a1ea5fdb45aabb965e2d9b673eb8f591397aa184db1eb262921fef75bec49a2f34cf30cddfd07adacef6ff4cfa03afd72ff56f452e6b3c60" },
                { "it", "db8fc57bdf9c510b4e8e6169375d913d24208f57917d29520d4a114a4fe6cd647a15837ac984547b91bb1535ce4d4d1ad287e1fe82733b93135b2530b0fd912c" },
                { "ja", "83f35f959235646605c6a0664a638c309b4a9e010546d4e30f6b44c26a5fbc71235fc9b38ab026c9173601053fff55e035d6a863e1dcbeb90a1ddd735c7b946a" },
                { "ka", "637950faf9b76be3758feb22201288265c1d86e2c83c20454be9fcfc2b6e3f846de9ee5128b451ff6442adadb3d0e4e0ff9009ce6d3c7bbea2972c706ff8bfea" },
                { "kab", "b481255479dac220bae5e1d8738e912cfb71df3d3577368f4bdee1de1e9e6ad2a393faba6037c83029e61310652f3cd9c72e353bb60bad3a1a483c20ff96769f" },
                { "kk", "12f870e0a85aa2cdc8109349e8dff338f0a3f617a4bd62e2dbb41854e6d96f63726367e0b2ecd34a9f174da3060ecb9e5d5a7fd804b19b9f97555f6b61cd08d8" },
                { "km", "9a7c38c5c1aba2ff1624f54def9c0e9e8de92122d5a9dfa52ce4f91824813e43f88ab37251204c48caba8a06c9576e0722f5784621549695d7ee4a339fb4660e" },
                { "kn", "d820d0f9c3f1115100fc11bbf0fd0c156594d0c61c2c5e829f4dadbcf3cb225dd42d6b44282e59fba26512fad3faa3cb8e69d5b401d58e1f47b7807ef6678f36" },
                { "ko", "4844b2a764419b984eca5fbc23a76a3390dee9c42d84336c1752605471bf94df7e428c737b1ed242e396938806b61b6ca1da990006c4b9a2a18b27063123e06f" },
                { "lij", "bf5efeed8710db9b1419d2a82a1ae187743ac9d0310167e7cfd64f49d4fa4811de6f918b43740c3c9da99f2560ad6329cb00f2c9422cc4582b0985de1eb4827d" },
                { "lt", "98732a778dc3d66113077bcafef0060d94ac0756de9a381e2b062258cc893d86813ffc4f2b8b90e206484f0594515b94746424db555c8e33d984225578d09447" },
                { "lv", "66339e0aca797dcd7dc1082b7411fe2a2c9ce54477aa5f8ef194ca200b85e42dd30032a33530195fb46ca79624b351228c05186f310e04d59c689f02c41b9352" },
                { "mk", "08aaf4edd859e60d0fb316a2fe8e1719389d4619dab4bc99f274396032c32c725a8463c8f498adcaa24087006154bcd3bcba1311943c9256378bfc856b30fc1c" },
                { "mr", "b85b8e1342d4ff8080bf65b9f51d2f3a6e35da90eb6cafa1e252a73aa2dd004c2eac3f094f6391115205fb427ebfbecb1072d1169c21b77c063ae77c5163cade" },
                { "ms", "fbf6fa5c9c53dc0769ce1527a655ea52678594c85d5f06efb057733f08975fafcbe3f5d280255311c6583943622d2a86066a3c78c1f774191bb2bf61e8a42e42" },
                { "my", "f298b6b0307cfa8a3902922b4dbfb451cb4c751a1ef13faa01abedd81f02b5ce835949b35682124f9c2e5ecc39fd59d561260d53b723be27309a96baaf2dbd9e" },
                { "nb-NO", "35b9c68c7f51716497b145ac462de5846c03712c7d1a4506b5fe3931712f4e2103b7c5cae23096a013d8ce091a2db1972ab257b028f97a306d134ac82ba392b1" },
                { "ne-NP", "b8ae5277aff1293b2ab30fd6feed5147fb5d42f9b80e11b6dbce72b1b63f11417b3a6eccc6fbb3c3e136c6eff0c2d02ff40c76263978b9cc48b17473d4ebc6f4" },
                { "nl", "7bc1971d4a06fe5515a2cbb75ce6e3a65a61933028e2f6a28f5758303acaa910fac7d16a36f1dc88750a85460d473ad8ec23c54850f8ff38618e8b3d94fd6c95" },
                { "nn-NO", "434564b90d1d3b5ab207f65d5072e3e9a14401d124abf416497f89b4f330f32a393a2519ecb058645ba9d6168b72ecf43e120602714b89ed21026554a300b5a8" },
                { "oc", "e269d6090fab478a5ea56577e84c8b7d93e6785c37e13be8c8e51de26f6075551e3e6370bb2f6e08bf23d793e4b7f0aa3c7bf28146933f47d22d30651ee44c08" },
                { "pa-IN", "26f8bfe5a734d85ccf72d3c05bb23fea86e42c2ea4358f10079494650b91eb01b935b2c4cda780eeaa03e6201b8eff567d5f9cd20cbff89764af8c82847b31a5" },
                { "pl", "b636adfd5e7e09b936c6fee4a2f111e42a9ab34e737211978b0e01b8535bc21f5815afdbc8c0baf86c25c828e79086646979d04143c5580ffd1198dfa6e6f216" },
                { "pt-BR", "f9b35d87594f951a87efe809f6459942c7e1259a1cccfbdaa8e28f551d09b8a52ae5622f4a090d3af812d9e87ee94cea8a24c136a98d59e64f7f266a0c85938f" },
                { "pt-PT", "d78b0f50bee1c52ea4118639d9abe5ad741261679ce742062845e3da7141e1d78c31aca9ce7444f9e37333b8bf04fdb15553bfd80a578d9ceab0bf52964b83df" },
                { "rm", "2f9374bec19b117a5994b71f070df997386569a5e4cca22740c8aa7d0e5a82e2b77ea022ef6bf769ccd2d74b42ef8f92310a6bd0131aa7e2e22e738bdec505aa" },
                { "ro", "70b45d165998bf732aee47ae7190f0929fa6a59523ba012cba8353924ed75794644e985eb184710664f8e1f4aacc5098a4fe457dc1b9ef1fb60e15483a3002ae" },
                { "ru", "c917c83c994369bcc4ebaa5a8d759c86ad94b12c8f0da87089f8fc7170527af382cd8a523135f2559dd8a03d20b8b934fff1fd4cb36163102a674ea0763e3d20" },
                { "sat", "9440f08be1c303ac9ccfccad18fca130c85f8ca56637343391d224a9491aa73e887026630e228b413b8f059beba136031858e3060046a9557d37c8a233bb3391" },
                { "sc", "1e6faf931855e1fc7cea6a5dd8ce024e2b54b8d5a212400e92425c184fabe384829694f84d5724540415bff70e2f5ca30221b98d851741531f815ee998ca2149" },
                { "sco", "d1b540652c480c9fd29ac818df875e06b5660ad03304a9e7f9da6e7f9a230afb8344d6caa0d23703bd8ffd0986849f7a70816b0dd142027d518293c13510a746" },
                { "si", "5a9782e3978be1dd2e7580ca896f58cb13807958e906cb569410d13c8c2902e188b28f75fa322ee74c567d4ca0533a8d95f1c37746fb0c7c4ae8d16568164063" },
                { "sk", "da021355846cc6a126fa170ddbae43e958d7077ccedf231eed9a4d5332ac1560d5b6be2800f8b1ce337bec0903e8d7902ddd23911a1919339bd863a32683c122" },
                { "skr", "8d99100eabfa425a8dfa35d5c766c681389bf0e61bf2865e2c7bce92028f996b7cc704cbb5cfe5b3280409c06b57f47a919cf4d735d250c8800c47bfa2e29643" },
                { "sl", "beac8528c50b246c889a8ec8a4155887baff8525ec78ce3fafd1f00c89b6d83fe969a04fb6bddf80c01fca624fd4ac180d03bb131fba6e8d23dff8fe27029301" },
                { "son", "a0f93650728f260c3df6393e4b131dd81c655f70c0a9ffcbd31bb60c5f75ffdd4f4b0b833762e3f2292c0709784186c23b390456c7c7e323b28b1bf5b0767fdd" },
                { "sq", "eebd3a3792473de641c01d29905ddd531a99a8a0f83a1bec106816be4ed2db3e31bc3f2c7c7df20b9a975225765f8181364585051077f8f85ee4ea39b692a067" },
                { "sr", "b9f31f51b282939f0e53c95a82cde7e1578636a9fb2e1a16acf76ec1d4264f66e31f12b07dd40654b3850ecf965fdc67f777d62d658b209a9cec4fed0c8d28db" },
                { "sv-SE", "3817f5785a5c8580b9fa5a7a171e07152b3966d994aed90935e25618354b1a539446f1ac1f791cc9aec24db3120ca06ff3d02df5c5d21f7d7ff14041ed10d7f4" },
                { "szl", "21fda3909e41313262a575794e0dd3d3d4317a51aa79dc3a8458a0d6a6d69c25627d331e1ca71e729a6794f511f70f9a60421f0ebfd456e59b5e005422ca4055" },
                { "ta", "cea4a36d47827744d547073ef024278a054e05eb6918c39e8ab3ff056f83df8e3d6dfcc7af7afdde3b4643dc8ca41f288001f6bf3401834cb4cdcdfc9588b37c" },
                { "te", "0d3bf576d972d486b61e2f10b20855f94ce7daab52261b035fc7fa426ec367e7be708f0a1c9be8308d95e32fd966aed7f9b04578460babf7336bc905328f7d4f" },
                { "tg", "fe3439399ef62eef8f1650075b043eb119b06432fa47ea51485a708ff9d976242e7fe107bcd35275a796a8e95191cf5cc087fdbb41246e8e0f6d5dfd7dfae021" },
                { "th", "4d872453efb8c62e7740bcac93c35697229710fbc23ee54696bf202729092a77ab73ad3886c31df1cf6e7880b50298d66262ddfa020adfc7d12c290e84a18bb6" },
                { "tl", "4dc0c84824ec63e1771ba09eaa8c5f44c972365128781662fb1ef771006d12971ea47bffa51e6d92d669b2e86402ce489497da75a3f5fcc078de5e8698723de9" },
                { "tr", "ac09bee94e9cbf6422b62d46443e6632822fe3ac6cedd7644cec41d9806c2d5128acaa51c44516c48f50f320aa0e95dd3205df67cae1634ba61765197986dede" },
                { "trs", "527da9c661e7665f3770487b79d2ce6758a860b6b14d53c61054b8c3dc44829ca77d55ccbee1a4b0cb9c7e20b237cb0e29aac386874be84e33c218802e834cd8" },
                { "uk", "04354fc6005a5af0dd7f97047fd9239f6e92acb254bf1f179e3c9f7e803de5e22732333fca85d5640195fda9dacf93f60561f3d2ed905663115d123878ee28b5" },
                { "ur", "2a406bfa7ce4e0b678594c25896b0c41ba21d7377eeb4765cc72b385a9dfbaab353a8bb7f2509e8ec5c2d7156f8009ccaf2da2ef54fb57fb417a9cdc862691cc" },
                { "uz", "cf1959ad1b112e981f5dc4be1c5de41f2f59dde4d2fdc3ffafa60e69028196b36197fd66cc9fd43b1594527fd1c1f6122bb66fc1fc63c41bf4eb4bf7ec5dc086" },
                { "vi", "97f07c2ea6d55fa13e824ace10a832c05d0db447dffe1f2ba57c16c54e4211c7444705de71bda9e9bf8d16e1a1c3f1ced411d7ba862a43fe153a2c08b03dec1a" },
                { "xh", "bf21fa0b5e4e3bcf0e3d9e6adcb5a42cbd4ebfb5f87ea0ba36b1f17cbe451b6a10bb5c2289e6cd19eccfb17f48b76a4ed219242c56f5968945cc9e1c90c376f1" },
                { "zh-CN", "a9d28649850e6924b4f821629ddb299bfd8e9c299c7138f3fe65ddff673e328b6de9de949814298acdd0c6eb5df959e857ca326512cf0f92d804fc8716f7ec3b" },
                { "zh-TW", "5a72150ea732f6a1f7d36984197d2bfcb4c52c5417399146680e72a77f325585b7541b011a68f9f6a71c893fe03da8a6a3848c9345cdc060ee5d0a0e0c5bd653" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64-bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/129.0b5/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "3118874f4b0c54075036e0c7176419fd819725fd9224edea5a7202fec3e18c9853c7cfe34e8d96dd1776024368385d986d79054b012187211e5877f97ea1e95c" },
                { "af", "7061e5fb38848c5f348b5aae6526afb5af347ce3329ed9795d690674844055c4ccf2390ee1624b108602e8130ed2e21123f94a6e72167c95b7e6896bd86e5f70" },
                { "an", "855adc64713f561fa63559b879afa38a3ec61338b8bab9ae25f9b456f81cbe5e3a6d7e7c958fb152a225b0134a2101243e41e8f6c535c6396a72cbeecf10b904" },
                { "ar", "1e63151a7a37e90a9178dff564e1b79da755a01fe01d4d5ff4a63ad7811f330b0b8bcb01a5d1763b7477c460864ed75f05616023ff14d8fb9039480f57d2a088" },
                { "ast", "6282b73d3fdec49b6a5dd0ddbbfb09fbbd19c3b40696744f2cd213a60245c2866f101fb8c695340bf6572962ab6895a6d012b47b2f8637715537e702e739a3c2" },
                { "az", "1f75441554d5985e209335739608995ba46b6388949b14ae2dae7730b2b5b02ec897f13c6465107adb9b6d8d236cb9f5f6ffd61ccec31ed42b5e831af5f2ca4c" },
                { "be", "205200c50d2e34afd19a9cec454a53a6d11c2c7c98f2436d3b62a3e79be893f1b4185adb5025ef706f735a7f538d2231c873d1c62559d1d79b37d1df980af162" },
                { "bg", "e9b21d59e72e5e77e6e4447d2b19dcc9119ebb1a1c2c8718335a79c40ef38f98949cb73f8739ee682dd09dd68a8e5649f98928bc8349347d244d0297bb10183c" },
                { "bn", "4613aa6d8eec7af0401a1393f700a6232ce192eefc2e2003eb6dd5a9630d1556d7a49febe5c87c9dcb4133687666f8ffdeb619786ed207a11f084b99f874e4f7" },
                { "br", "0555874c73a8a66e1f13cd220bd91301f4b9588163e471b946162ca6765875c2cdddae71aa108c14490389fc0e76cdce61bb128356228fa1bc5f44cd62b0bd48" },
                { "bs", "7251046ce08f0a6b19482dfbaadc1be04b219dc8a4b312273b7cbd00fef9242cbf7b38567bd4461f8e7a428e561e85e2cf25edc52539f623b9326627fec73dac" },
                { "ca", "256a835a3df0bd965eab2a7644b4731ec60f3be21629a66e59a00d8a8832beb93c894e8090daceb3da1f5891615cff41f04ebf8e940e7b1dfdd0b8e50771d006" },
                { "cak", "dcdd78e5182a50617eaa2ac34cbb1430a5dcd21a04d0e5c4504b134aacbd1e7945d076185dd301933897cf93b7c9faf02eeeb56ecd3565199e9117632594dd50" },
                { "cs", "4cb2733f60efad2e14aa5e955f0d18c85d5f34a0d27f1b007fee2be7863bfd557fb7916d5dfbbcdbc5e081ae62c8964b3ae591af5751d1e0ed7c9327e73e012b" },
                { "cy", "288e008c7da82469a871b802b77d205f705c2bcddde37cebed6a6e7c76dd64ae65a7b508e0ca3ed07c361398c3256758d2110fa2a6b615b1688dc3c41ffad259" },
                { "da", "bb95416e7aa47f2105225eed0c114237523fcde6e71199ae0d02baf469ac4102d27ad644a550e066a6e98d1b1731e15cededddf5c8e1e0dcb38a60e2492b5ff6" },
                { "de", "ab9974807380ce10ac5db9e60ba4f5a8e4668f3e36277c6c339ac1ac52e66bd124c0e66f8f95c6b5212826bf1c423b63b76c361a689153fbf3edc75c960bf2ed" },
                { "dsb", "5cfbba0156232c3b72f8721fe2929989217c7a78421296032f6b406d34c3779daf2b1f0aea07fc415ef1af87f620d64d70536b2827a335f007db187e59f44a0a" },
                { "el", "93f89f268a69c78ca1080cd2359451f32f1b8377897af2520738d8980ce0c9bb72de7b04ded13d9a2925d5466b975f49ff41d7238bf19c6806234ec10c0126ec" },
                { "en-CA", "5d66ee254675a658847b654f04ebf26535bae786d34229ede9ac5a487a672b0a82afbe6d7fb9e3520f6ea5545b24efbc6f491b5d2a1195ae48b039d530a67e35" },
                { "en-GB", "14d77d96c537a0ce14d62ad976b33e3d46680b0f80ef86bc5759e6d0437dcf265421b4ff4f9aadcc8f768b1bcac4a3863b0bfc9c74868a843f2fe8bf703f4cc9" },
                { "en-US", "c5d56b507852e22956d5f52f6c56c783cdf2dd016bd400e259234d2b06c2c6d37118e89b6e0842034efedbb8e8dfc552c6eb5d4d3cd5298a95835044dda6a7ad" },
                { "eo", "bbb5927cd50497e2e5b3f50342cbe97ccc2236f3b2c07db823a7b789facdb80075d2b193c99295b9dd12191c769263b4d69d69e996111816068a43f55f03ac8d" },
                { "es-AR", "002843816063c74ec3bef23243c82babae7dae1b9e872974152cafbf87dd29be9cad2d4f93b4948ceebf7e6ed92dca7a015cc4f4dd2383c86e6f00e0ad89fe6b" },
                { "es-CL", "7855dba50b1a89758d9aabe5a98984ef1a0505d861ce6e9cccbec5ea45c38fd9dd1b7ef8bb9a737112f03299dd4b38fdae050d79ed88a0437fd3b41dde52395b" },
                { "es-ES", "520f9f75cf843765a1c7eca9cf03846e92e5ce9fc6eead51a23303203a7860306fc1e20a6b167b94e25df7f5a16565f931915c9eea93e82b765b7672a07fa181" },
                { "es-MX", "d308432f94f8ccc32d92558aac986240920cbf6af2e9d81bc30f84b4b3d724f67e48d97c7252e15fb92dcf823a37cfbe42e39418af14917880b41c68d34a5856" },
                { "et", "73df8faf7ff297fb72b1dc816330a3c5d6f49636c4d6531db22365d3ef4e8cf6e415c00367c6c40a2fa093a50c5e38672e9866cc8a849644968fb29bf83d1841" },
                { "eu", "0718c83e0194ff47a6a6b42ae436ba5c2588e7e31fdcbf3895d4796d98557c5f3698f6ba84db9322f0d101141305ac3ac3b05884d5826886cad817172b4afde1" },
                { "fa", "f81526e5b8c11b3b7b1f32d3f434f6c5691327272cbb70edf2fea94d68c33056f0b2b3bcdc9a94af8d1d19bd18f0ccbb997b554f134821bb82597d7e7c48347b" },
                { "ff", "d6b340969f4dd1fe31016ceae237bc35cf6db053c25856a77a1e5d69f29aeb0cf86a062dcca14b35c87fa6125f11c2dd537f5b95c76c281aa43ae2b9af9fd2b0" },
                { "fi", "b020def186615dd76f8bad124b2c208c3c024780b18c78dd70f6da926d639c8fd333819cc9510e49e4e003ce70d596d2751ba953f7437414bf8eeb6906bc9a62" },
                { "fr", "9c77d0e7c8db3a47cc147d7bed33279868361c47642a1d9b3ab335d79b56f7700d264346cc3d1999532b7cf850d602a37d8b2016968ea8c412fce369cdfeb758" },
                { "fur", "3190fcd5a56bb0ab46b9daf86fd94e33df0ae018d15907a03e9b2098dd8170c5db585111bbde895193f50652fba9dcee6f10e7d38080118cffcd09c1ce722710" },
                { "fy-NL", "de5d0ba6109a5588744176aef6e7f1da666cca3ebebb6918002009952472b0582eb1c33bca30fd94cbc1a03a8c8d471362d74dcbde0609ed5bb2194c63ee32d4" },
                { "ga-IE", "9894e5c959d90d9abb90ed97b1d33f3652ad6d9497e45b6d538dae27b6841d741779e9875893ee901c3f3c32eb044f7ac3ff94e214f1afb5c89c409c70bae576" },
                { "gd", "537907c89b040c55d0fb25d5f6e9a221f4d36595f191cf95cb3eb9802a02ba586fa43ac7bb24951c7918dd38c643ac5bcf990194bbf220e25b6e4b456c596d8e" },
                { "gl", "c91ce1ec7d4dde233a91feb399dc40298ed35abccdf543ae99db9ceb9e55779c91e16a0a20e9ee473cc2f493b9cca4bb5ecb649fc4e816223f14e45e92c63738" },
                { "gn", "99ef93288fe80d4b47e3e658eeb25863bb6ac3b99551ecf2c38f536356e09903199f04d83ad90300ffcca187e98d03ac35eb25358ec2e47c7ed108908682d9a6" },
                { "gu-IN", "bcd73dfa6c71523b33e7a135124178d8477f11b41c5516a158278d7d2afa7e5d74a0a89bfe125249c97bf8b24c7aa06ceff21471708f19eee9b9eddfc0c4d099" },
                { "he", "b17cca9b80af33aba6df2cf737ca13aed5348de9837018b786809bf2ac11215a6a0d8ce004bf6a0a1b8a7b59cb4549b7305ba9db2ab8d733d32cc90496a464ef" },
                { "hi-IN", "b94ada92f21484957ad942dec5d8c90c0cd887abdc82b0c3c01dc255a8cb9f9275c989f07ce0463132fb65aea72cfd126e67fd851d347d181781921cf8d95cdb" },
                { "hr", "d29828a4dc2f6a2d55afb402c143b7172f20fe246453fd2faeacf70064b8d88b109aeb5506c095012771903fa7ff36cb50e758392cbca51a298fca4c5d9c89b3" },
                { "hsb", "d0fdedff4c9a7eb3c0fea0e45ca43541385c005784a9f49c48bfe8172837ebad148d302154ad2c7caa1d91c8ebfe1ccb75757f8282e7a8f1c8a90ea436ae246e" },
                { "hu", "57c7882801616959077f7455759498117860da02877f0b7f5e5d99b7e6a878fb0b7c0b195ab6b8f7f0bee5e198a6b7ca5740c3d1aa716faa32762c8e662130fd" },
                { "hy-AM", "b0b12e3538bd40745c49779bec5adb025c651348977c73e192c699941c793482c3258033757281959230f664a1cb8db4fb5a3becf125bf3609d555cbd0b44bb7" },
                { "ia", "412aef3c7fc2060a44b0e93d2c4865cf42bcbdee3ea532db9032d0e9a5b735c32bd5d7b2148158c484dff9061d8493849223a0a2752fc71c75960c97f7ddbe8b" },
                { "id", "d0753aaac0ebc0a30ce394fcb96d9432f75fd32595b255ade67b7345d221787aa1a8bd43da85a24b41e30c4cef38a69599bbfa512fe028711592161c2da58618" },
                { "is", "913cc58ca3568f274e49c4d4889da59a83dfadcbb7dc2dc96d279ed713afd3464fa917cd43c44b4c592e8bd69f2696a4e95e12cdcfac7e6843c6c4f5b0a4b665" },
                { "it", "53984c2cfbfe6f7d6e0f563a040c0a7eff7f7f250a53b49332b352090ee3effc8c00cb3153d259369bf899cf818e73659b8c0cc59394ef44eccc3e3f9436648a" },
                { "ja", "5b3c81df6b321a9087686769e11ba06d798a748deb4bd29cc8c7db9350310f4264518fec7dc3cbb6b0dd886e7697e44be426ee3d1334008c25f1afd5b3e635ee" },
                { "ka", "14f96ef350b83aa4a74c9148a454702adaa70b520e4b92e59dcc2570b7da0b2f96761e897987c3f262b54b1a4caf55a9d3442526d0f6f87810375e9f691602e4" },
                { "kab", "7821d9217e63a6e168d7cc647ca10936485b56d0308d6ccc33b8731671e9894a5e52e206899a44d28b63142fca45cce8a53dc931e1dd440562322709461ec261" },
                { "kk", "9785b7f740203f425f0d5435078b003399db83581afc391f9be1ee2fbc27c0e07a38d035c6c7b6c3ee8259467e8bff2d758cba9c58f788fff3627173251bc6c2" },
                { "km", "b2c1b939f1874f4bf16bae15f0b9dec14a4fee6d4df10e0e9dfe906e99be4ab7dd28598d42d4b3e466f2ad00f00b895ff19bb141c930975310120e99b6808d51" },
                { "kn", "4df0cd040ba7a7d74d9f1aedab2161007828ec97488b5084dc23eceeafadd898db594f39032fcc0c55f7ed4719e1cdcaa1a18074c4d7e1e6fa1fecbc1bf29703" },
                { "ko", "3080107777a04a3025d64beb4199914717034e24f9ef67019f7cdba2c586020dfa14ef4c29c24e426f1f353f8e9a15d97a00595c7efe73709c6e08c30113bdb7" },
                { "lij", "1b53bac05d1e19b1332ec4b4a3394e798f7dd22cb190578ce2a93db81ac34c8a75823cb29477e4ec589f57a88034fbd8402e1f3edeac64992262862d66ec72fd" },
                { "lt", "482f3ebf33c78ca044b70cb72807242ea595c25448828ae53d2d1c337a681fa2d39faf5c1791d1e74fdf38747f2db6f6bdb1b1a023cb88b8ad24c1520b019718" },
                { "lv", "070a3fed842ed217795688ea3b3fef87ed854239cc92cde07048be074d89753c739eb79278b443482d840e84caf18a8abe01139bac10a460857cd82130bc9766" },
                { "mk", "2d36a5a3127e6d7e04f90e0db51accbf3023de62b3308869028ec6ad3ed7c65632be9cd1a8e8e8128c854ee8e66790f6249c03980342a5e9ccf263615eca6c37" },
                { "mr", "1af37d99aba359edddd68b0b13922966702b944f32a0d064cc0932344976056ceeaf95b96021cbe760d144f4ed7354003bb2e32324ec8e210641b5e08bc33684" },
                { "ms", "cc5613e2bf9a6e2695332806c8367e8d9317aeeb64c8346c10568da877ca1e93c920b7615e3cc959d3f20c5f3a21f0d697e3a42afddfa919679f8164b723a06d" },
                { "my", "097c2e8dbb58dddd6e7c7b3359163a683af37f911f4bb5deb0251cee82da56538c8d95efa6733b2aaaf0d9f201f3890d147d1acb6b878a70dfb3ab0c6b5071df" },
                { "nb-NO", "a07b6eb232d39c146c4fa1fb7d86c141b13058343db1b3079d266272f70fa3ad5155a1b01953191688c93f99d3d94e8e152cb546c1d847eff2c0ec557ef465a4" },
                { "ne-NP", "76408c7f6d8a3b2ec05fb5d4ebe918ce6b277f48a2d3f36f2d4a60a661d02e6b3be31c829db060468eb52a7b09d9e5430c4ab581cf37cac798bdfe1cc94191cd" },
                { "nl", "714485a35c9a05aa4f98d423e9051a81790e50a2703b03eb0b9330e357d58b17f2dff0a839f6c80d88a9838d5bb8c74e4fee9110d7fe6d83d9b992198396f8bc" },
                { "nn-NO", "6070c06ef7e40a1bbb46b62cca631746c900faf3dfd945976561e597879c47c85d6db0c82168035436c212cf97b68c7c14a4f6e7f01fe6b13e8681f17daf0a26" },
                { "oc", "a2827fadca96b8115f4ce7c626dac5fa8e5ce5c61c05202ca8fdb1a44fe0d4d8d32255edd55b6fbed6c1e92edeebb02d7a8ad843d131be8b743e3d6b6889f857" },
                { "pa-IN", "095d8e8e02c61d37e890a7782f4150f9b62269667bccbb40397d4f32b83a6004ba631df195ae3eb2e8c88d4a2b11f123679df7161f26c45e291b044434cd0896" },
                { "pl", "038856aed9322baf665e1d5a644ac8b8c46d53eeb648715dae788120aad8e646f8d78f33a26e8edc22556f62bd6a8320bb5174795308f75ec25c2df14dae3512" },
                { "pt-BR", "e0a774dbeec2f37dc15452cd59f926339dab100a1502a88958fd34ee2bc9707cad706f7b81d90ad112a42c9a3e90aba51f292b6ac2204824688a12bac93fc7bd" },
                { "pt-PT", "599ba37f0d22f0270c1ddd4941844868b42efda573d264682f90e5dd0a8cd7142e6ff0ad821142e9859db32c4ea8647fc6377359ac006891628e7a8fb4ceb893" },
                { "rm", "0e69ce5ba936060b737bd55b11d591533b0e902dbeb53b297f260dff7776cd2b1f495fba3cdcf253404a7bf0e61171c407adf147d51266d49a2a1f209d6b5d4a" },
                { "ro", "8f3cfdbca88c3ad8df3d070ee22adb328a5da04d394e294be9cce277dc5a38366fd9b04d8dd9d2a1572c39b317961f421ca89ec7bfc28d11be36fd952e01e5f8" },
                { "ru", "2b9c0bc36938a2903c189c01e3c024bc514795f2a2752d5979cb7e8d026fc4796cd9676ff43035bc7466a4348de5319294a8fea906774fb03f03b41ae7c4fcad" },
                { "sat", "3b18b1870e98c249237c58aea40f49ce49130a9b40a99505e7a3e8219b72f3d22312903b3cc450edc1603227605ef779a6969ab68b3d75f6b760fc86c868b9d0" },
                { "sc", "147f5100a7e3e582b8e56261cec87509e977f6a2963d0e91a2a57c7f97704f1a81e50879b378ccae984c18a45328c9e99914529901521c4b27a988b10a6f52a8" },
                { "sco", "723b9242ea5f24b458d97953821ffbb00f9997cbb7f49c48e25fca7576cb5dfd1b5a264750f87a0b5b78fc84109da80a19d6626a17bd096b655079671f6377c8" },
                { "si", "e742c4562273426a7c14dd2e51a97ab48fc525aede63e4152b713200402dcdf846f9fff7ff0c04ec40232619ed8fae0277e0f430c744332ae3ea881369f091c6" },
                { "sk", "3cc01866d0eb71f4376944da32d05bc146a6ba6924c9625d6971fc4df7ae9673e99335c2986f2c6615cf2b7a9a8e8ab52784fc469b5d09933d8078bebd14c2cc" },
                { "skr", "29cbf1d57720fbeb4398168f069757c99b6ba35adcd32d4deb23a85d791d0fa411e14eaebc31d40ee4039c7a1f7e59e54711bd5d3e356146d981f84c4cce1685" },
                { "sl", "8a940b0cf524c75017c38e2705b451af0d887813556999bb0d3434023cbc34478ea39bce1d24025446e9fc28a90479c9847ec43d19893a40c44e1f77bfac3eca" },
                { "son", "ee007f49e65cc88770a431f625436e7bed8b055b1cc2a3a7b1d33ec10fa668f3a6ca006a8daac0e83364064e03cf36ac5191b735ed31d1bcd877201509802d33" },
                { "sq", "c93a34e55ea492eb7fb5ae186457ad8be6f6bd857c8404c22678169eef906f8917d50e43e518538e59f5024e57bf7d8c65c8f8851c6ec69449ccb54f83189ee4" },
                { "sr", "bccbd314766fb2d5247a6214e0eee019f405bb6ab234a3d88315a08d83bc544cceb55934f8b841ff7e96ac4b00f3d6d9a86d28b20f83e2168b057704517c12e8" },
                { "sv-SE", "bffec7064aa3f67e0c72868b1ad5f183354f28269b73aee22b133a6603827598ebd4d90bf03423cb51a22a17aba5d68e5b5d7a2684535c1268f138e90ca90af0" },
                { "szl", "18a7c3845d857fd6da98395561960add662b5bc17a563147014671c85d344fd144ef619fb1ba3cb254a5c73bd6f893551dcec5a68a96e08fa580953f2b5d6eef" },
                { "ta", "992194b0f9f4a46b9a573742c1b2ba9975d9e15babca9c61efdbedfaffd29c1c950c9bf8ec5f1eae8731354633c78f33738acb8e62768c90dc862293e3bb5180" },
                { "te", "89657bf8b5a521467ae0824c860e0eb0fb5cc436158f51cab6c22b30e2121a994a238c73d73c0597512c9b2c4f29ecff1a7621967bab882a76c3e73a5dda988b" },
                { "tg", "8a2768283fa0adaf3fef3e6dfc60b0388ddbdd2ecdcf8d254888b3b3487725066793e8e29db686ddf2c54c2f6975ca53ea3d99b3bae72b82536198fb657d7cbc" },
                { "th", "492a16beb3b0ded6495c326e2f56bbd8c0f1bf85da6b4f671c873416e7800e135ce83f209e9f1081a1b48ebdfc2d1e716558e1695548e6b1e4b6215a2b65aa28" },
                { "tl", "2fe5929a39da7805267521cdc46f0b992fc10c4813fc7925c1068ed3f15f1b2bee5a1dfbdf92a85d0d4f0d94a7ca2a05512f857aa615aaf96334711ce43f13a3" },
                { "tr", "ca63f123e49e22b1240a041a1a8c989884976ee6b93dd14a660393cf6a5e5f552123fa4409a3abddc8d5f67b7e240bfb3554e2e2b5d17a3177d5b19f8d8a8b87" },
                { "trs", "7cc73d5dbc998229c9b1998ae0957d985b240d59062505d9dd4c21d7fce18487a96d9603d967732b1098a99967cb1d4bfc2fadc2ae60ba61618f2c01e5f82f17" },
                { "uk", "e981c4ae6d1827da8fe8218829ebcb735db7973b7abc49cac4da4b45cd873dce80c37c7494939206656993edace3b8a0378aed607706830b12f7f60b8c5ad0ca" },
                { "ur", "78ccd9641cfc33ca48c40e2a77fa7caa8bd460cd2c1a82eda22d01deaac28f3cbaabca2dfcd185c300e29561c2a3fa707c76272c3db13759cd8063f2fdc0458a" },
                { "uz", "d47df9feb94dfa2cbc5d5bd1b8c2346659fa2a6693b3acb3103bd4c5b1ccf26ef8d715ddf444830d9c31bfe9c6c12a616d83733855aef504be1da51e344294d1" },
                { "vi", "26e88de5ebb79ae80f56843fb0af914ae9ab1eaa4839902c02bf674ffdb6ad3c9d2e1023a1334c2289e90a6e29be17bdb57be21094231332e50bc0947d39a94e" },
                { "xh", "3c509c311b64a89eab2f88be444e0cd87ce9bd1deee71f048398738b6b3603f21992efab7d3a1e9a57dc42d7ae8cfe70ccdbc9f52cb60aa787b60298b848300f" },
                { "zh-CN", "b3e94cfd3225cf9025250433582206cbb9b71346faf54976178c0b8db2cde621604c422eea3e9520c914113ae1360f6200e69728cac917e44610d4d281e65126" },
                { "zh-TW", "314d5f01bfac3f56ab379d29c2fe10337a77f5bf60355742aabd22c70036aa1239140085307e7557e3d41d35d74a7953fd8b92b659793f513e8f5573bf12f3ef" }
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
