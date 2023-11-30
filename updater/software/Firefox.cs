﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2020, 2021, 2022, 2023  Dirk Stolle

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
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using updater.data;

namespace updater.software
{
    /// <summary>
    /// Firefox, release channel
    /// </summary>
    public class Firefox : NoPreUpdateProcessSoftware
    {
        /// <summary>
        /// NLog.Logger for Firefox class
        /// </summary>
        private static readonly NLog.Logger logger = NLog.LogManager.GetLogger(typeof(Firefox).FullName);


        /// <summary>
        /// publisher name for signed executables of Firefox ESR
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2024, 6, 19, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// constructor with language code
        /// </summary>
        /// <param name="langCode">the language code for the Firefox software,
        /// e.g. "de" for German,  "en-GB" for British English, "fr" for French, etc.</param>
        /// <param name="autoGetNewer">whether to automatically get
        /// newer information about the software when calling the info() method</param>
        public Firefox(string langCode, bool autoGetNewer)
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
            if (!d32.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
            if (!d64.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
            checksum32Bit = d32[languageCode];
            checksum64Bit = d64[languageCode];
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32 bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/120.0.1/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "bd860b5eb3b25fd2fd2a8fc927e039c7a4051e51035cdea72af65530717fe185eb716ec7d8c63c6c5790161dcac6e9f53ef9811a19315b81af822312115d2d3d" },
                { "af", "a4e03033633ecb1956ce884832d8975ed2809647593f7550d7716d924fb397c1b318bdd4471cfe54dcd54130b895cdc467a93950e449b1b495e6b73a357b322f" },
                { "an", "ad37e05228a2f56d0156b5492f65a189de09267add8ab9231e739c3144fc1d4c24e41245596a501cbbc3933d36be82df1c2163c9f28871a2866b8793114e3028" },
                { "ar", "0fccee1067210dbf85e43e81ceaecf01c0ab56b3717b45e4475945697c1e7e3834691279b89c75410961cc8edb6f2b96d80f531141f5d72a555c538d5283fcaa" },
                { "ast", "be92860de4a89988736b73300f1629b641708db52a96c48c2460f4a197b4f5c46b7efca5a2c45ea75460d94506be7ab338dd17d50e35c6dc0d258cb9bfaefa87" },
                { "az", "2376b0cbf167e7ed65ee27672b72e2d6f0dcf6024324d24d66cfbb0c08e3a79aaac6b49acff1a08d164478801f447cb9c41eeb0a0df50347f163ebcb86fff1f9" },
                { "be", "0a07674772bcc6f04fdd058678a88bdf9fad60b38f6d9bc0cb4bc8faae8dbf954754e1ba36b954a3f5474345ee803db9052a2a99bcf253d9431e5f58dfd32c09" },
                { "bg", "202a4b201f270a707d7c1de36916a0f67bebacd005dc0262d87ae552d8be77471f2b5887c909b62fd9903dc78b9679459e648583d41c460a978e655bc18f005f" },
                { "bn", "5d675780cc99dfc90e7af8554a0d38cba588becf8af4e838a63bf6249a886de8fc0084ebee94c688776b49062aa4411a829f46d373068b8b0a9385ae523ca8e0" },
                { "br", "5c06f1bab56c03c72a9e56f26d5ad2e2c5371f123f5ddd672f7fcefd0938e3b9adb0dfe4ad0c19ceb75eec0f37bc2597b1127af0cb9d63c7cfafe23b8a7d0afe" },
                { "bs", "2e799188b9f040f01faeb026f053272d0ec1577777edf783f9976cae2c7bb8e7c1c36de25de1700bcb0079473a5042b7fbb99840e15e55b9a4729f6b9c1fd677" },
                { "ca", "0384d1d5edfbb85179f2bdf1a19576f056161ce9a4693b39a3e0946927bae51c1eb54f99b5a17dcdb4e51d5aeb40dd2b78f275549b3324e5dd057613015e1d37" },
                { "cak", "e25be1bab8f1b561e4934a1f8513c0fb2942ee35d68ba27dc568aa268baaff8c712cc31e87355f9a7445558e82569124921d3527dd8f4283046e1eeb6b24169e" },
                { "cs", "a0384076932fc4609e68867266b7b51463d8be855a90a5fb352fc058117cd755b29738ac029255c34a42158a2262f988991bfe8289e88f378c92ae6905dba4b4" },
                { "cy", "62cda32b25510f7233bf722fa568325f294677d7f29397689b87a8e2484632d1a3eb6031938f8d1c91c668decfbedd3fd6bf2fae95de59854bf8b33cd9cf7634" },
                { "da", "a3207d6684bae8cc2e1794f7b0f2bd4011dcff193c3a7c6f3f9583662450cca34084f79d978d3cb4f4bab82a65110c737c0a918a3e71caec97735fbb60df3422" },
                { "de", "555e572b386c01be96bb6542413ef79716b54b4db3c10f519512941b6fc30c96c00ef54422bcc55c995d097b4793d0d9015f59399e30c0a23c34f6e396d02a91" },
                { "dsb", "ef992769218240173d37c163747dcdf02deae03a53c5bead1290d9a308089ed37eece5388f1949b53d3b4b153cb7812a9b2f07c799cfaf914831fa129738f671" },
                { "el", "94432ea8ea4962b14b58be286ced3da8f9875297ea0984bc26b55a9278cee3a8b15a28d4c3ac50d65d4ebb3868d15ae92291b98c36b4cb3afc05c01cd9a302c8" },
                { "en-CA", "40e63000132c93876de649b5ade2413582324c4d27d01101ebfe9adf16ac7faaf8e2eb7347022c714cef0e35ebf6dc2f07eeb6db7214d71c3437cd4a1ab0e229" },
                { "en-GB", "b72b17684748e81ac8220a5951fbb12a6771d36adc01fea2609dad901fa52022e69c221eff745ba1c56c389cb7e679e38fc7324a670dab8c4a1559080e88b633" },
                { "en-US", "75ec7e2ba49d82b92291d9d56456befae530ff856b5e18d0e881d6452ce365c19b0e8b48e35bc96b3f727028be8ec13dbc0d6aa3fd1a69edd4e0da69af6cb7f3" },
                { "eo", "e52bda29680fb669e1305fa46d13d5429202249feff35c589a12d35355dbb62c78025d092fa4e301841507a667ab874b084346d1b1c9eec6fa65ad1ccd88e671" },
                { "es-AR", "6ed60cafcc5bb6011e7edcab0a5c3fd784ba7069442ca6df06778aa569c3a09f54efa7a3f2446914ad415080f226bdcec5240a376659d9e8076e227626019d80" },
                { "es-CL", "e3f0cea30c3c310c22b9c3e5c35bcf7a7d6bfae1d40087529e2cf551689bdcfb3b084acaadfec7ca42011612be58f274ae10701c2295b41606279127ac2efac5" },
                { "es-ES", "6fec23eb8384bed588145b6ed3855c37773f3554b54fd855076bb086c80e9bbe10141f14e092c8c0081343d642180123d9ef9c172ffc60d11560339f60e3d4a2" },
                { "es-MX", "a90c3ae41b9908b7916288ca9baef6103a1a663979d8c8e0cdf7768df2439b805c684d854b76e57f1f68c7d906fd0b9d29acbb6a0d904a2a041055cafcce34f6" },
                { "et", "93e0944926267b83ae96970562f032225df9cc66098a5ff5e832e8ef7617002991b443559320e59e300cbe789c2b99320e7fb4c3a353ebea9011eca1c2da8dad" },
                { "eu", "aa98e07b6a722d9d0fcd5c5da7ed50e2b1f4647dad8f99fba331d85de0127d836fbd83aa861377d1b50472fdedf05c68d791a62291fcef1c8beb989a5aeb8fbb" },
                { "fa", "16c9dbc5a98fe6c7c5f9efade90e205a36d603b0292bd06e53f255973f9e42cf3ce60cc32e077b7a8d52cac84ea6f162a39070e36f18a9ac5f2ef87539ac46c0" },
                { "ff", "f87658c6a3ca28049f03e57a584788cd1ed87fd0cef3a7ad9850c292177f54e2eca29844ef954dd450e191737e95385c120629aa6c6b2812c122241ff7797c47" },
                { "fi", "1f2c65aa4d5e216f0175544b94bfa3bfa4517e5700345b88979f3ce5e83625b8a685c5355e3acc9ecdf09cd476ccf82a3eda910cf53be25237c500c37533a86d" },
                { "fr", "6a653de84b5094ed06632b0b943d816cf8d52462c791f5da688b48550cfd29cb75c1168e08ed58b9eeb6a4b446394728ddc1660338631edca3e89c53679be8cc" },
                { "fur", "1824c45db2d1b022a2ca236c40d681f978c8e386e0a563994f237bb75ba91132d74dbbfb32e17a3054978b58545301f6b7e188391de93def0bc8a85908b1a4da" },
                { "fy-NL", "3d559ae082f88267184bebb1fdbee5f997abc11b3765fb314030f1ae3ac58c0769f6a0b06f0036932a754cc066be58c588ee0418ca57bfdb027842060064b011" },
                { "ga-IE", "72d38b8a20193289ca68f2a4e7b00d4bc985375536a83cd28789c796991247baa80d49f05d7870510e7b14ce28c2b09874cb3e5df956bb056adc30186bc8a607" },
                { "gd", "4971d751ba794413bfd5e957f640eac1f9edb006d94c6b5fde0894fc86863a8f747f2c4dc34157013b2a54c30d02a3f685a867e88cba46cc2cda0e967595d8fa" },
                { "gl", "57e9c333306043a51255744148ae9e73a6976c790a3cdd4c4ce0a8d15a2015ef28b4e2179e1ab7961a703cf315f07c605d25bbc2bf9c21a144acf8e4acfb1993" },
                { "gn", "22b66f49e07d25a7beaaaf261f04305df47c9135d5a085ae314bc47b0a54932b6398052c694bee8fca0928475da4d9c0de15bb2b653e070d9e563336a6a204f5" },
                { "gu-IN", "7bab762f19ed0121cbea8886ea5e23e1b856da3254efe68b651a6bd8ecefd7fd096c9e9f90422602b1135493f1d04d0e0d5b360cc5df5c42a4e4b79e6a335700" },
                { "he", "f25c1b8817c829d01fe4d157c1a53dfd24d7086e65467e69509825b46ff4ef500da63df4db0e31e4e501385fd9951eb88fd2e81fb03630a1ada141542148204a" },
                { "hi-IN", "15a5f04ba4dde4a514c3f67892c91eef9f9e0213ec2494e454cafea41f3fdaa0ed37efa9dfe2146906f47483c4052479a8f2ce46ab9c17da5a78a36991a841d7" },
                { "hr", "775db8ba7433223b06253523f61c920bb91dbb0c606909b70af51f1ab6d403b6d6b6d32a8f942be758b5624e49b6acaaf6fe6c3e6691edbd220adad578fed484" },
                { "hsb", "5f1a07f4d88a361d5e5b426733e1aa32368b1fb8fc623500412d01be342c903444a8dcfcee9c6dc4861235e09c873d9f3ec9e3d8d3bfa2a73d306555d770cc8c" },
                { "hu", "ba693d3dabc9c2beff4cb014b4ba62728d1e5794e725619daf23736c6f925b5c087fd8d4970a5daca2c1f23a3150a4c37bfda883615c412f4a19a1663fca1090" },
                { "hy-AM", "e7a158c3d494117d712bb39c901aad1ef0f182a753e20fdd07374200e45328200181d2d2929ab3fe2604b7a7fa0a453e4b170a84c33e83b839c1f7e05c355ef9" },
                { "ia", "a5caef19f8c9c53c57631591f056425fb8dbca461f2b2ee490c87e56c81d3e197d53288d6a98ac392f7f14743becf88d4b0a14699197e620d22e0ec4d80e6dde" },
                { "id", "4d7e9e12ba6db274262dda651abffc9bb4a93788134bd2d00cfbb59a516ecc365b54eb27c555bdf1410908772ee2a58b86f618c956ee74f6148481a37223f65e" },
                { "is", "0a0741b3b23c3f8228ad61014a74e376f1958f4104c930d4d3e20f77554ea7d1152cc236b1d31cde6e837c31d68e13ecffea4c1f48f2b161b2d377f0e693660a" },
                { "it", "564ab76ce732ed20fbe06567f29a3d809e5b2bfac3122bec30162461abe184a3364ef95db29e73951e76e4ee1ba3e681cff2849f81c3038f7b24c3d68dbae1b3" },
                { "ja", "b637cb3f5ea544c2ed058568500fcb42cc10f448ebf9f8963c198a8e0591368304e4885d76c9e2db4701b6804e532b54cae00ff8360b621d1e68c4726396ae67" },
                { "ka", "9f9712356a387d05d480b93c1154232c5ccb4792eae74aeb20c2101fdef0c77a5d7d39d6fea9438b3bf63f4c24c5969686eee0c2f6fbfec299aa9c01d2ad7e84" },
                { "kab", "8e4333ededed2008987e69ef1ede620d80070e9c764f2ed39be8cac088ae0c1c13e670a8aece5acee02c9ac9b822d86b849fdc3af45c6ee6577db86ef9687e7d" },
                { "kk", "22f24c31623de5dfe5e74b355ed40a1028bef5bce1443895cf64b2ef2e2fff409eda24b5d8324460eca1b3319065146c969e3feb3cccfca75c4c10dcc4108b87" },
                { "km", "4c068ad1f651f5dd038b6762147c5923dfb4df3990999b45d0f898f6527f2de4e03aa406ea428a1171d69853009d79c209a224ea5f68f2cc7d0b586800ee727a" },
                { "kn", "275db06751f2d9d7c1bec824b29500f95241f30ae2f96f80f047ee3ae7e76392e501cac5e10884a0aa656030d52ce134b6ad792d7de335118ddc0ec6458947ca" },
                { "ko", "4ca3207a54c8bd93a896a074362470672a3928601f9507fa898e4e1e19e2e3210ce700ffb3a8df5abd0e719f28e6821a0f9e8fecd669b3631b41b602355f279d" },
                { "lij", "3709303d0fbbddf27d7337446b9e90ea5b15bbae8182817afad7c2d562ecd0e2f3583367c8dfaec09e11e3b4031ec3fac986049fca6ce08fabcc8950312bd3ea" },
                { "lt", "0ef9ff829dc58ab67c6a0cbbf7355cf26c8c77f7c1bbc19cca7dff85960e6c6c1abea1c7c1a81a1e948a26c52cb81d0aa5b53fdfb85cda17c8bac122ec34b04d" },
                { "lv", "14f8c0110b260b538e393813a169474c79ebafb4021170290add16269a3501fa83eb477931d1a1dbaefbe46194b1e83864166238a1095115ad8399dc89788b87" },
                { "mk", "7e53e7688da3ec3b5cbe777b93442e631b72fbd30b89608296084577f14a72cdde29398ecea490b79f98ce1fa56f35fda06bdd13a9c0ea6a1b3638a5037be97b" },
                { "mr", "b3bf14186cd293b8d858bf5ae3d5a2c3ee8a37d6c344e7c2a46bb4e35b703ddab3bd2428105eccf621558845393d8926830ecc6ab95a078bcf62b8fdfc7a96b6" },
                { "ms", "538024a3f320c35fe3f6b981761ccaf03d7b05a9a6253e5fee1ba10fbc23b4329eb576bab2fd1d12964b507af4b9daafa75cd9ca6ffc3baced0f7eefe683a02f" },
                { "my", "242bdcbb20fa8466bec6445560e9c802bd9093d65e0bbf7e228cacd21a8d68c2f5f13e55736d6dec8f775e6376aeb1907b9525a48c492bf7c9407b8e5c3ec58c" },
                { "nb-NO", "907c58ee01fddc2aa23aedcbcbda3984cd512eecc81fd746d23453188a54ba9e1965dca21f27b2b572a972f0cb8fd0abeb8b3cd30d09ed7bccf15fa712348244" },
                { "ne-NP", "79f791ef808274a47931273ff2fec17b1be75b7cd630d15ff790bc85afffc151e98cc83520fdffa49d39bfa9ce2950e794173ef420a4c3d5a3950efc470664af" },
                { "nl", "a89d8d3c1fba93fe89090e5683f3cde7296477e42274d70093d2d04addcec31a1409b4f9e23b9eb473f13402caf2ad78efd846bea7874450ba52fa456d0cb45b" },
                { "nn-NO", "78be54146f35521f1b0c1c3ac75a5e22836c760c688e1ad9cf67f60c422b3eeb07fbe75dc4886546b33807a26a1d270f5382190858a9792985c7335666d6489f" },
                { "oc", "7d1ee9d7ed0d36ec384ec502453fe7113c0dafd676b8268e10f14d42cc405c816430ae3e32d87d18f999f0eba9bcdf3ba5b021f1cd342e0635c547368bb947cf" },
                { "pa-IN", "5628580ab089d2db2e75fc2b3b73ee904bb7f323cee707ff4794ec559dfaf41bf772cd3146815d053b7ab3f2ef5257ca6ee98eb5610dd14c6ae4e300050e4d67" },
                { "pl", "507a95f7554905ca8f1098350885734f27493137c66cadc9a7e70d11993c826254e58210ed355b6e6fc39c331b85693175de2d8aa505227cad2f119c6102e0c6" },
                { "pt-BR", "cce220a365857ed3550bdced8d9bc3181ef702e4b7f2ad9d162c42a28466877ec81ff3c46fb0a2c2965d33a70c3798cb359dabfea409fb0fb91be23bdc533121" },
                { "pt-PT", "464da41e72f44cfb138f1bb01ac1648924966d5ab7dbe065c07213b2bdd8777dd741d8901683a28806bdf2156de019773562c9ba14f659a65c3943e3cbffa2c4" },
                { "rm", "6ce3483a6e70e598da5066bb8ffb91e3b8d00a9675f7736cc3ca2426e8daec6e98ebfefb6729be7b5c42faab47d1b7d350aabbfc84ac55c50c9b0a377225376f" },
                { "ro", "efc8bbd9d7911860999a1af7261c8f09345ee57e91c0b1ccfdf24a6a631e8b17b330937b7baf38a4c41fe3b0991f1e6e9b37ce50c3590678f4eaff606d5dd45c" },
                { "ru", "7aa601fff05a8d755b157f3c200367bc610b40de721358be9a9d05a374e948ea777275149b2eb62d20376ead2b4ded35372dc2d0fdc0cbcdc4e7ac7b3cbeef9b" },
                { "sat", "4bb95323e28f43b947a3f2acd56eedf0153288a3f45c2af94e0d4f362c99d44cb3fa907800c6c541f0590e951c75590aa19f1c3280aa86731bf6cea3a5da38c3" },
                { "sc", "f2ded659e1728297f3bf103a5e1f573cd89a0ff19e6a2f4a0fbaacd6a452a0237f0bed84f24aa595bc64b27f679690e8d9c79bbc04c94e47b3e58420487cd26e" },
                { "sco", "8b9481403d3210bf293b80f91084cd283822051113a694d84d1b4bf8e9e21cfb6782b59b8f730bed688f1338f32a7c70bfe1c45b7275cf145319a2b9f33e67a7" },
                { "si", "6fcdbfdec0259942fe1bc1e751488d192ec0d625c90efc33f479030987c4b9801001f91e8ae5abbdd440338d7d6bac43bc13cbe2bd1df4e003177c19ae68799f" },
                { "sk", "6c75d9cca76a384fbef3229e4f4513127d1ed5e8820654d6464dd4c14891586c84db5798fe2fce7324ce3960bf1160e0ae98821c6fc07ab2d394451297f11f6b" },
                { "sl", "4bbbf0794978ae1641bdbb07d8da66d1c7c0180fe09ac640fb5a7db024be571f66b4c608484126900602329e25dddf76544aea33bb3ebd02c0a9af2bb64853fb" },
                { "son", "3c01bc841d796809887bf4c659f1592a626228b1eac1cb0504136691b9688f869efbc72c45c2d083656c6b6f8f3d26ff9091ec96f4fe706036b803c594049a28" },
                { "sq", "08e61344691159336047afc4071f3ddc9b46ed8a5f80aad81b35f308b924bde75cc0fe0c8eed9c02a1218e4fdb5940672eb9e433bf8be746036d7ecbdbb2bc49" },
                { "sr", "3f8e732cddbb659288ce88575b1557095874c055aff857a85f47dbb2af5dbdbd2653212d76783ca6a4a2a75253006668ba87fac4959de7bd3450f2aa91057eff" },
                { "sv-SE", "004a8ab7bf9672e3d372267414836b74946fc2594ed9fdd63fe6a8342887afd33a9603215c9a9d7aacc33cfcc4468df481f9624bae2d7df244c2c6e39298647d" },
                { "szl", "7157798ff034e1c4996b9adc2f1336a3457481ceb079c752f372880939e8a3c03447074fb1e487ccdafcfdc49cf2108cb73cbd0e0b919f1844fa7e2295324cd3" },
                { "ta", "e744436d7e72a3ca49e1e989dd3fca26e5f6820da59b0ecf594cf2d0b22bd46370a4248c1f413bb1d6b90d4b71e6d8d8f0189e45ec6735cbed81fda74599c761" },
                { "te", "6352bb46585df49ead470354f1f18f226b470c6ca03616618a0ef8f852978c921fdded718867e8f07e42e7089d8600bb73219304207ae5ea6176ec3e82ef8295" },
                { "tg", "2fd14162a7b8562c179c07775d1ca6c8bc43933d863eeea6f13c4e90e7e8f1ff7e61594f8e1e33a9832a84901642b98c30fb1ea4c20489be65d18aa35113c141" },
                { "th", "3ba9c323b98e8c4d0c65dbd8856ad2fd0a2e0d4e1f7cc190383c4e784cf1c1da921830baaf1221e396061fda209981f5d16aec74c6441352f13d728abddfb626" },
                { "tl", "f170904597387475600317001d0e7a226bc66e6574901f10268e22ed5b64373eb02a56184265bf5724e5b8e29cb69e331000a201f0de1a7ca66bb76ab9583d9a" },
                { "tr", "df688c609b1bafd423da4b9cd13256dfd339727be6fc56ba0269723d55a79cd6f6ba2a136ec99dad0f15af63f2a1e8b05e7ae4c20fc3fa00e316e416f970dc07" },
                { "trs", "f350dafa98a022dbea733565c29f37cf6ea48e3987614a62dc8bc775c4600adbce4e4610548fb9d228cedac4e1a8d71354da7f58a32df7887826e55b25a94ad9" },
                { "uk", "7607dc011a30337413e4e0cd56a178c14dba69019df8e015f0180380b975b15a7ad451f390d958ab593fd70dcbdbb83187b452d7b5d8c90776947536bd1847bb" },
                { "ur", "8f2b032c423f747a7e5bdbea78e493925d866f0ae00ea34e16517add2bcb349c3ef2488a58967f25ebb8d781f7a1f85d1bfd8c6ccf12dd28a28cbf41da9bb041" },
                { "uz", "d440449fbf69c30485d16b282fb348ca66fbe87e43ae107ac2d629855cbd5476c9cf407d4a9354ad30f6f1a3ffe3e4c7f33e7dbc11cf32bf286a6b6a97ccb19b" },
                { "vi", "699be05574af4403eef854190612b70b482bf22b1cb9fa23d3ef0d0a0e83b1fe73c89f4d4b2b83ab64e68db19094a034e493c7421a0630c181416a84ef996982" },
                { "xh", "dc8999631680c838e75d90f67bc98c61d1638b80f87c0642a7313f11383c049f56a12f55ec4b0f5b4a5f74183966d56dbfe0eeba7f209211aeb82e83a0c911ad" },
                { "zh-CN", "8127059f2d7b5dd51a3c6d3f905d3b5e402a13614b91e770d5ebdbce9f751f8fb6fb359505221b17383be79c560c07e0ad6c2e1fb7fc544ed23b075b56550bf9" },
                { "zh-TW", "8ffd404dbddb45383970a79effda355983905700ad4ee6b8625afd3a27f2e90cf9a7c29da33f50a0ae25749995ddb99268af2b5a4129c0890436cacc9ae842bf" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/120.0.1/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "034166b1613fc4aec58587a620e56837824b7f3b8db4b2c1354e90ee8d732c6339566cb9fda033803083090e581ff54bfb66c0dc3e1ebac435eb6c9ef0f693d9" },
                { "af", "8a4eafe86288373f79aa42f83af56823e538c7cb6d15e98444760f994944220368de3e3a4747da083b433547935dcf78a6139e0ba96f9f6bc7dac486816afb79" },
                { "an", "2773f5f53932e767a8f1c771250b6af5cc8c1fa9157b9ea3334d38240f32dcc21745be6a0fe7ae6f5d6fa4816abf99f6745db453ce38b679b38e07488233ed04" },
                { "ar", "21de4839ed3a32658fb659ef9f00ca3587a925484a028f3c77f7fcc65f6e768259c7886a968ff6fae9849a56f124b93adee17532a6cac2bc708c86da0e74fc9f" },
                { "ast", "3af6d5e32a450b87936ed5f6bceb797e47f718f94c9c8c65e0cb431ccecdf0aa59073fa22f1cf5bbf481215a622fd39152c8786bd2ee31d3b75ef3b9dc5b07d3" },
                { "az", "02eab2d0b6dc7c478453238f47b691b29702086dfa541c39983c1e4882c1758a8f6736cf45912700c0f01cb8495c4ade51c2575d71e58701901320e0594366b4" },
                { "be", "a3c78b63acfd91511c190aa6a3ec3f56b3b246f2e86aff0122018893fb2376da8b6e6543768a6e73587f3cb493cfb071bed181836fb5f23ead3b407b583a780d" },
                { "bg", "762ca62d4cd70c321496311425f49b9f6dbae59b6603c089a97ba9bd72a47800cf9651bbb3ac4766fad597e19c2f3555f61f2d9d7319a9d528e60daffa303e72" },
                { "bn", "65a73070c9a24790304a523111d970d8dc64baa89296bfb36fe90ff17bd3ba7ba19b64086dd445c15882efb865576bf3ac60e2a424b934f3d658dc6255f71a46" },
                { "br", "878a7eebfe9737b1840c0d1f590d4aa64adcde4913aaab40bfe45340a100a0b903c19385062cd2b985bd1673a34752e3f6c531a2aefff7b7d1e718156f84c70b" },
                { "bs", "80f3970a886d8893c49a9c3935490c5db0c90275cc1f332c4883f975fa190ffe24e19bf74f2864b89cb2d0e457b9611f7c91e5f0c0ae06027c170039b7c87456" },
                { "ca", "33ab758f036178bd7b8e45dcd60d32b35b87330fe2c428afc32f04aed182d035880bf783e9d8f95182d097eb5d9286ba42f4643e922471f712a31220bbaf6e71" },
                { "cak", "18970b627a36baae29e428c56b1e20673f35f764620579a8d8c070846ff57a03086b24d57a8f5d9961bae085d2a9496cb3641ab9f133dd857cb3e04dc668a757" },
                { "cs", "9f60d71e795b54494ca8f123f47f3ba4bcc3bc109cf2a936a3f6152710bf1fa3dcfff54403c48b3ef23577ecf87a7c1c35bf338724741e1dcad2c4304af74703" },
                { "cy", "4336a8fe08e61a6ad28257fc40889985802dbd7bf2e583afcf438db2c2d3a154b64dbe304040840bf4ce599d3eebd09a030874a74241d4769a7f74379bc2df7b" },
                { "da", "f3e0beac7e8d63eb34b8099a4f162873398967a93f89ddd52630db246f17a32b1b17bb9283782ec79af6e10d5e0d1db9acefe0173934fa69281a6bb7680aeb6a" },
                { "de", "4f1eb2eb23c92fc905f7c401652f32f9591e85677ed98d41aae035cffd37b7700617b1946bda3cb4836ac227dbc37b6007d66bcd7f77736335206db545e30a8a" },
                { "dsb", "e72cbfaea68c8a2f88815fa72be0b521ed3e56c044159921c56e79b5dfccf0e19182a59b58e8a2e1887429b07b8f1723c4b9214405e202ccd96714b44bcbb360" },
                { "el", "150062e1c73e30d81684c9f934fa0a60291ccbee674195036822e986a158d024d9bdb99eccc50abfbf5b81cbdb7dd179bea30c33f05204c937d263e3da4ecd2f" },
                { "en-CA", "e14f85525a86e123f80e145f2d70662bb03fa209ccf9128a26ff4b760a7e1bd449900c649d9b78eb62ccc1e30cf6b093b394324e6cd442061b8abf5ec523bde9" },
                { "en-GB", "5cabe5fcecd293a952068ccec300a6872b482ccad88a2993e42d851041a2f77f78af4838418636a7b6c1f2b33c84e26f73a0402352709963ef0749c26ac7bd52" },
                { "en-US", "e3a300895a3361704694635579bf554370d11f50fea6d649a54ae23c73318013bc8338f2bf2e87adde2d7bec055a73ab2fd1f9b437e3644228fcb7887c436e80" },
                { "eo", "5cf0e6db830b6727c5a67bcd61944c851187723597611de0dd0659681b84500d6f2c488c530f181c4263eca6a4c8957d2080d3b1745cd20f61fc655ed3818068" },
                { "es-AR", "85afa189d4ff68b384812c8e890be8ddf29cdbdb2b40a666ddbfa87fb125e41c76a6717e832aa3b16a736b8969da20e85f7577b9384056ecf6f88acae51f33f7" },
                { "es-CL", "32f56db741b661824000344bb89cb142789540f67a279023ce90546593f44696764a3f8b28930f858ad8970c68559b99c026805172cfeb6850c90911532654e4" },
                { "es-ES", "81ee0d3f1374702a13d30ae8b640f7bb480b66100c87b2800ec1b7019240edb15b90c302d9174f33f4a98fa1e7039ed213e2a1ba812f1660aabe1eb022814622" },
                { "es-MX", "c3dfe32922b8a522ac472ebcc3063f7b90664885d37ac1132b8eb2158148a4a95d95d7b34b4a0115b959d3deb30da7153ad6ea08f7344523151b68066075a458" },
                { "et", "6324f0c66906a85cbcb6ca39b62c38ee0f860c6fca75320ab6601ff5347f29b001740a44a0c84a04d6e83a424b596b9ae942c64c213a2273f4623ac16b81e9fa" },
                { "eu", "d0f642e539508c8d56ba31658a9a70066aa7e8cb7f05ada74ba6753bd1f9a324266ebe213d5884e97ff72adbbea5cd41ea21f7d09ccaa9097cb081a7cfbc274b" },
                { "fa", "9b2235f1b5a4962d00091251416a0f6ba24eecbc75a1d2236b18ccf2fd60b6da77184751cf295fe6fd6a6f64c34798f69f2609dd875c08faf06443ddf7528ea5" },
                { "ff", "715d7a138634883c74e33b7dc0e95e7c6682102798354b22692b26a5b2031206cdb518e2db313af90b39a94dc83f9e206698ee0905d854271fa8b933b9f3a5a0" },
                { "fi", "6bb0883486f0e381b59ae6a2ad7812760132186ebad76f39cdbc27a2e844d2cab9ff231bd44cd8697ced01d45faa515d555cca1a16662360a9e9e86272d420ff" },
                { "fr", "28948040ae2635180709fefa9809a7a01c70de01ef008f062dcdf9f3808133feb50495b103f7a154cb17bfb8780cc3d3fe3eb7cf6a384a41114a0cd4f2e4f783" },
                { "fur", "b8cc0ccc86e48a6eee65596e72c49f4b2dc65aa19b42aaf52bcfb633b93f2ed6d351821452e5865b2679a32e92f1d74df880668eae8844a3cd727ffe6ea4be16" },
                { "fy-NL", "260f96f4903ebe5cfc8f279d88e869eb5b4375a249b2e049fcfc5070d4e30685ec089fd378ce2e775e7fda18b0ae928ec48e674535e3ee35f65fae1bbf0d6bb6" },
                { "ga-IE", "f37e83a7b2cdecce5dcb086adfb32e519fd58512f0ccab5db32a7e93ff1ac0489e8f98d809e83d66828f0784e56c91256316cf0f57baa201915dff1278fd868d" },
                { "gd", "e779c9e3ebced16c9642d6d48e1dab78469d78db1a50eeb54833d64294ad1a3073b1a897a750c22efd72a77618d545f7bc98648f55a7630a072cb0ce60d95759" },
                { "gl", "704228a44f0d2ea40ccde13a5bd61ce6e78ac780f6752e0a1ea5f65080427f97e0b4d9c2d674b6805a6250ee5e914e800edfaccb01a8e9a372bbbe2068f12dcc" },
                { "gn", "23e5bc220eb62db556d182b1d6bf0b8abd7aa7653aac19a0a686fcff7e1b6aad9282a91480d0af5e2a47b4ed884bb9557737b90d30f3534b314cd34298935c80" },
                { "gu-IN", "f3fede6306b0626ca1dfdf3bbc06a0031f423a906fc9d04b279a30e2a39f28137e673acdf82aaeb498e5af0d65e04a08e755fa512af75779625d2f0eaebe4516" },
                { "he", "b767960619e2c2e53ca6b88161a606ff4d02dbd8a5d35aceada3325d6e4aecb631c00bbe552fa85feaa596941f89415b00aec32229c66e37a95a31fbcd7fb621" },
                { "hi-IN", "52d81f559177fc603063fc64896510b8ac3c981cec857a0f37d647dae5d9ace2ee9a33f7be6c46ae94da4b0ac997629080abf144653681df7f6862fa066b6066" },
                { "hr", "923939a9032b9d2059da28c0d6183a9937620d25f2b06f53070437770a4eed590a2968785c34ed24639e203d35ec4190f3c6c2478c8d59f00c1006556a7f2d5a" },
                { "hsb", "c07babf274fea8a40528c3a074ddf8016b621872086a53b1b818c1bd356d4dd6e36cd1a61f62678382244f3be515ecbebfd8746ebdb7aeaa9967854797e98276" },
                { "hu", "fabbc8a2c3503ab6e0ee354896ec68a1e7d54d4a94151ad1a17ba28891d7c5bc5d0b44e075cb80eff24b5460feeabcb910bd086994c20d5c96e91f2c7a7f8d12" },
                { "hy-AM", "cb95d6454eead8cf7f717aa377d2e85ebe7fa7ea1431607fb6a533e5fee199c8da69d783fae8326236c0dd50d3e9976031221db30fa2f0ac56c9ebd4c61bc251" },
                { "ia", "38bd0e1751a59ae339b66770f855b1feb395d50eea1bd16f8a4a5698667523fbf47221d2ae77906f9764d1a896c063ab033bfac57d1281358b5fcb5ba40c186b" },
                { "id", "161a7b9e65c5188dc4c151487f5ae49a11225a74bf0269fb59064570326220cfcf095999f748d071a65f8a349330bce9a7012e9c01f92d0cac0f128f7be20eaf" },
                { "is", "fc8a2568b91cac6441abd64eefdb3d25a8c6040266ddc748a96f4c8baf3fff0d9c7ddec22544181b4e9a7501a5e2ba6569f184dd4476da699d907321560cdd11" },
                { "it", "9e83460e32d8a8c43e06f09c867cdda1da473bf50d47998458209880562cdacb71a44f9459633a3ab4125d5797e1c0543302a307373fe38d021433f3c4a84918" },
                { "ja", "76e1a89d0f50bd5bac5309d145ae4bb09621aaf73817ba0630d7efdba8598e03f682181b61f48cdfd9be0199f0ff35409bace7651180d0d85b376b86f2f08cb2" },
                { "ka", "ed1a5d51e4c7ac404941dfba4b96957f37cf1606d31b04a23124a354ff07f6e65ca7a5ae6f1611af85782e7633d74f0b75a798f95f5fd10eaf86901c148c33a8" },
                { "kab", "1b3e27197741ffdb0717e7c54880a9a263c9af01293dce1c45948de98bb5b381519a6c8ca98d53a278db3f739014597fdc281d431ca9a62e1e05ce50381275b7" },
                { "kk", "25155d90c790e8f717f7acf73de228423c52a81e398ebc0eb5dac62d9adf43d883da8c24fe1e9d8658154d642f420fdcd95243d12138e503a657ef67b77a2078" },
                { "km", "ecdb2ca4978ca579c3bd5b25e6da4aae960b10dcbadce44d920749e6e89a2f0443929f0b7640b824ea04d8d75c3723a79d9d79873a92eea330a30ed5b360bba9" },
                { "kn", "15b2f090d3bd3775e673371a2ee732e4c62facd7293e023888f4d6d5ecb5c71a5705e52223746583f4ccd364dd430dcecffc1719ab5c6cd4f56fa0a636b7f0db" },
                { "ko", "c0b53f663bebe25938f00a25a111909a458e25f29940df3fe76bfed1b28c8078a021a16b8617cd76de5b165b20bc7729bb3b2d8e412f7e9945830691c81639df" },
                { "lij", "9880c42688b3934e8cf7b050b2b7ff4494636d287a7f86b435aa4bbcdba72e753fbf1aa59005266a92024bec7d715dcf1c85a0951da9544bf1a0c68d328c9658" },
                { "lt", "3401d8f79e70fd433e6f97a93c9d3e491c96a11237e49ec050416f0f798d2e9fa420dd712a6deadb542fd8b1021963dcc2a103bf6547df76b70d6a10cf56da7d" },
                { "lv", "cce52e6cfdb59b3d43954b7c6c0800138dffb57eecea9f8f81c6b89125d732b726b38184dc1933efacb9df85499213849d8282c7248cd002d8bfaa11fc05ceb6" },
                { "mk", "0b0a62ae0f114316ff22374977c08b54694d26a770c447705025db4443caa38391cbd34379ef72f7e97190a4cf7d4ada56427f98963511aad6f7443ed3755fc2" },
                { "mr", "5e4cce23d8d2779ffa32689cde25f9f2c7ddc06b93693ac87fca97c7352c4135741e0d2cb89e07ffcf08cd15531533ea487de9a6e81bec6174d73c45a5d0834b" },
                { "ms", "52a5aa719252d615a8011c2a09298ef65bf47c79110e27a29037e90a34dfc6180cd474c969da7aba89e113ed64ec99f301344a3e1971a4f6c680a4c6a9a3edec" },
                { "my", "cc8185ac04d0fa6f8d84db937214d0a2a9d1bf7794706aeb892612bbada9644f406e82e12c4a0530e18919048d06f5725631e62f49b1d33e0666ff3624e26dab" },
                { "nb-NO", "e4487b3486e765c25e8652db1fe27366e1a3ac5bf2540c6739db60a89e885a18656f2ead6eb359a2646778129e48ea274d4bffa6de00bb23b16408c77e726ccf" },
                { "ne-NP", "1ad00c3de445063d3c708fc4f192ab4637722838cfdb18a403fdc1f5e78fcbab6cebd2528872f68e87ea60b05c12e0a42be7cb62d138fe2a90efc8b54c81404a" },
                { "nl", "a15525ce71fbd55f83b25022e9d2473abe564884e767e724cfabeaa67a6ffa594131563d0b56c7d5fde0ce5e43a5a31ca8430cb32d13d5011fdb588245ca87d4" },
                { "nn-NO", "ac7468a8d055fdb7ea224137500ca0613c8755d253054e600263f3f20576d500411553b2969952eeb3244db5140ad971784176f8ba001aed1bf6c9a8ee90c668" },
                { "oc", "07c28c854541610961f06d15972532dd24049e936806b785384ccce84c6c8f1d4bfc1dbe1324d331e48e16af1b854d3c4afd434c2956b66999ee281465b40ad6" },
                { "pa-IN", "3e2b4f32f653b1c090d309b7dc964087faa9f55ae6a82e0077829b90faf454b7070650fea8215e97c9d44d553c02bc793534747f3aa28f3a3da772e543692581" },
                { "pl", "028bc911593896a2b84524f1c980eb3df53747e4c2f39f9ce4d3fe0c010f2c39d84aa397f6928c0e8b89eca789708a4a17c2ad077e87ee0e712d5deef5f7bd27" },
                { "pt-BR", "54e6908283796b67f4386d0d8f2fdbe00609c1a3bc35ddfa4e0f9a625693b46b9d453b19d8061b716d3324e48cbd7f77d83b55743dcf4995d37196b83df7b4fd" },
                { "pt-PT", "7631db64c41860dd3bfbb0e29458a92fb472669f4ecaee49adfbfb7f62863fac6ddb557e317cd3ad1ff653b3206ddb85cd98c3a2ddc572b89f323c8dcfa1cdbb" },
                { "rm", "75eda17be74270da6208185d8ff8525ea4dfc9e7bad7e4e68482884c2603cf901b43e3df7628d18e7c485a6a85fa07c084b8d370f5e9fd9f63efc1b8e5710202" },
                { "ro", "6be3406e33703dcf5826e80ed67d3c9d82fbfa3e807872f571dd5366e33023be09ccf45461a85d976666088b884fdeed7b318b7141e5b342e7e257c62f528ef8" },
                { "ru", "6882dea07b600fd93a0b177c427e4e8e39dcf82d3ba7502896eec0aa66df31f36a4b98a6527201659ff3421683fc7636782a60c3ca50c20b150ad7301b365609" },
                { "sat", "10448fddd74b29a20c84f095f21752075990f0a374f0023ddbd4741c1209b74ba77b69747098282436ade9c5d4685acea3a5dd53ba76050ba004688a5059b506" },
                { "sc", "a3c036e0d0c64d914c73250645cbef79fad1ad21a237e77ba794953f246d3ec94f720c8bc6f7736311773e3053da3e65703d0025aeff0276927111da9c9fdc45" },
                { "sco", "6ea4f0d0731aae0e4ea0cc354b9bb60a2c9c06b8e8d814b0cd59d0623e53c8602cdc6fe3945ed83dfc3c9378b93bda20e17bcccd2f120e0d6894f5e8d2ff873e" },
                { "si", "7958c04e5128e145393b75f5a6e5c231c5b66dd36dcd9463785d970a97f6f97bc8c21492a4b90dc8ab31951d4b59a71c7299bc49a3808cd8cd895d99c0a5eb9f" },
                { "sk", "e19a218a4dd9ef08eadc3a813191c50113342bea42f09bb92f91c9055b7837794e87146d331c7ec32c3dedff045e304033eb8cd929442f0ded5406c05670ad6b" },
                { "sl", "7facc14caf9276bed29490c02c80abbaebaa6ca3fc4a527a9fe1940751a2058b68a9ea666e5a6110b054da4fad8978da2ac05ef62eb5a5744e9ba79d254de0d3" },
                { "son", "3f0bbe5e1d3a815a18fb63297dace1179a82d3fe88c2bafba6d65872eab59f1a47735e7f5482a1e773f5efbbd7187ff6e26c7f4ad4569e0a2cdbd132897d6593" },
                { "sq", "1f68c9014056f77843f48034275466013fa4334295612686d4300f86f079c360f6c8dbf82f3c1c3b67e1c99d1a8e6d8e08b67314cd56fb9c0180ad28cdbc8980" },
                { "sr", "38dc60d098b417b10736ccc544529ecd2893692e4db8c505e9864c9e1f18ffdbb37942c8698ba183f78e64f3f273c243b45ec8bf39fa813b8ad96d7e69b36db7" },
                { "sv-SE", "11c53c4b8db00d39c9e3e8698ff31c58a5775a4460126d3d24329406ff73765b654e4ceaf8a68eff1530a95d43ddef7b506b13975f8bf0dd8d192d2155b65529" },
                { "szl", "a3b0bba27c70d4837ab6ba0afa85eaaa728479a299d3ce52d0d470f1aecd86c78d0be9f352d70f0e4512305fc099c42ca9f72a204e90b511017c7b6dc2c92e50" },
                { "ta", "b9380a8884e0aba6e3bc83e769135099b0ac11759770978392dcf8672de4336f75a766bbcece06c2433cf4c37c03c28b788d04e6e381cfdf751cffc82049e254" },
                { "te", "9fef0cc5054aca9e6d10a6595a8605ce2999dd28bbfe60779368bce3c2b8be39cde48b6d360afb7800a860f7120675dcfb2ca608f0708e3cabee97c986886276" },
                { "tg", "0c51c4dc99c0f2726c998df3733307538b02947533b3f3f3b4de5c69886f0df5f899573069cb880fe089bd772cf0f311bd65b4e682c914981c62d4ce81fd096a" },
                { "th", "de6b81d0e24239044368f2c474d3284863c5f047bd80093d6f8933e8563f4b5da0c7c20734100bec6f797276be8be8ff9bcbb11cf9c463258722348bbc3a3b6a" },
                { "tl", "612e0541994449397fcc24c1e29b5b58b320c2e5199bb25d2a14fe261f90b977ce673f06461a0a360cea474645d1c63fc27d975980014fcaee75825adac0ed8e" },
                { "tr", "2b99ce265e7889d32c43d53f8855bb64764c9f4106533604973cf6e533c04f69344ce78e0b819d62302102d5a854102cc009ad7de403797fc4f29384832f9fc0" },
                { "trs", "bd59d8518e766fd3c6caa1c4814ebe62e4ab9c6b4ec7a5968d16de969c6b14835baebb7134f9372780e2c4c36b2beb4b5e532c099368832c0ff0ad44b5718b17" },
                { "uk", "aad6d0656a8be7dd7f527161462a25e4ab314eacb953a360e9f8b38c6d6e7f8272a956c5187f1cca3b9688a8457e9138cdc831821725b9166b45923924b85218" },
                { "ur", "31ec203ccf3ed61f668417584a797ad14927fad9d91c0a8caf3b3482d27a8e17738d7a84c809530a74a9a8a715b8d0b9022732d682e90439ef1e2bb00bd0230e" },
                { "uz", "71c7a1789d8d9ecd434dc6961d55f52121f15452f4ad66ad4283d810a4df80c7df4cdda50d8d52e847303a7e1af10088df3738556a4676e3eb6ba41f03e1ea12" },
                { "vi", "a4fc3c1b7e1b5ccdf653ccdca9196231c08a73aebe205e9568367d4a655bb44ae238c60c195261d7ddd187bebab39767a3f409f971bc0aadd7ff816c01977ab9" },
                { "xh", "ea05fbe74f6defcfb29f55000377af5d1e3e28f8cb271b28d055db9707de663324ac72444975b77f74f426a0e570bc5d9a254b25920a9d3ed344c536baf19ad2" },
                { "zh-CN", "ee11660e07ccfffaa7a69006ba6dd688b0af527a1bb3a72568475bc5e8f7ca17e49596494fdb4d2faeb62e8f4e72051e4841963b979068aba2ff59b347b07d9e" },
                { "zh-TW", "0eee4ed493437faef2f85338c50a283002f5b76852f6d700b7857f59b83bc4860fef8e853e90e760e90c320c096e2082353a7c69dec95c710bafb2c3b0e9e709" }
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
            const string knownVersion = "120.0.1";
            var signature = new Signature(publisherX509, certificateExpiration);
            return new AvailableSoftware("Mozilla Firefox (" + languageCode + ")",
                knownVersion,
                "^Mozilla Firefox ([0-9]+\\.[0-9](\\.[0-9])? )?\\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Mozilla Firefox ([0-9]+\\.[0-9](\\.[0-9])? )?\\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/firefox/releases/" + knownVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + knownVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/firefox/releases/" + knownVersion + "/win64/" + languageCode + "/Firefox%20Setup%20" + knownVersion + ".exe",
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
            return new string[] { "firefox", "firefox-" + languageCode.ToLower() };
        }


        /// <summary>
        /// Tries to find the newest version number of Firefox.
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        public string determineNewestVersion()
        {
            string url = "https://download.mozilla.org/?product=firefox-latest&os=win&lang=" + languageCode;
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
                client = null;
                var reVersion = new Regex("[0-9]{2,3}\\.[0-9](\\.[0-9])?");
                Match matchVersion = reVersion.Match(newLocation);
                if (!matchVersion.Success)
                    return null;
                string currentVersion = matchVersion.Value;

                return currentVersion;
            }
            catch (Exception ex)
            {
                logger.Warn("Error while looking for newer Firefox version: " + ex.Message);
                return null;
            }
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
             * https://ftp.mozilla.org/pub/firefox/releases/51.0.1/SHA512SUMS
             * Common lines look like
             * "02324d3a...9e53  win64/en-GB/Firefox Setup 51.0.1.exe"
             */

            string url = "https://ftp.mozilla.org/pub/firefox/releases/" + newerVersion + "/SHA512SUMS";
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
                logger.Warn("Exception occurred while checking for newer version of Firefox: " + ex.Message);
                return null;
            }

            // look for line with the correct language code and version for 32 bit
            var reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum32Bit = reChecksum32Bit.Match(sha512SumsContent);
            if (!matchChecksum32Bit.Success)
                return null;
            // look for line with the correct language code and version for 64 bit
            var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum64Bit = reChecksum64Bit.Match(sha512SumsContent);
            if (!matchChecksum64Bit.Success)
                return null;
            // checksum is the first 128 characters of the match
            return new string[] { matchChecksum32Bit.Value[..128], matchChecksum64Bit.Value[..128] };
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
            logger.Info("Searcing for newer version of Firefox...");
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
                // failure occurred
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
        /// language code for the Firefox ESR version
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
