﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017 - 2025  Dirk Stolle

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
        private const string currentVersion = "135.0b6";


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
            // https://ftp.mozilla.org/pub/devedition/releases/135.0b6/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "96f32609df9d4e945e9780a15ee6e51f3eaecbc469839cbd447167adb25195a307c84267484460bff88a68bdace8b1fa818c5aa6f15111e483efd49df941ac11" },
                { "af", "4d87698d6111261b3b4022b0e93fb8eadb47033a7dff33926a1d3e095e8ad555c539daa9d673fdb2bb1b94cbd379b367aa4dbc37293efbec6b2871a51c9d9c7c" },
                { "an", "9525ef82f32111e212bb3f4843110fa1835a823fd869467811c33f766cfca3dcb39e755a4fb74234c9d62e82a5d9f77d4d05e54996deccc896725a7435ffe11a" },
                { "ar", "2acbc4b231a3af51e457d7f4e2c76021825d6a635329d3731f0ad9d56dd6c699629e282f400f6b7601fddbfd0988760e76e451fe4fb7becc28db359062f18d15" },
                { "ast", "3369fe46520594d31c770aa11d05a7976a0d4ddef32ccc28e1d68c66702c906c636deefc208816f8e26c04e22e3093cc099d2bf0b35e44879e19315ba9187009" },
                { "az", "81bf32f6128782e31df9f95b433f7401e0162d7ddfef5c0b86e06ae9bd27659e3e49428f7d8f872232c118d7c8145000bd88ae4f48455d84e95c4959c4f94757" },
                { "be", "cd8da8d8ed838b978cec01356ea8cdb10f435b61026aa3f9709b28171235710abd0231e99df78b31d7347d0a69f3924314e245e27fe591f19b8332980fbfa36a" },
                { "bg", "2dcc2b711ba7092fa196dc92c923dc14a08fae22276163e8e50c33aabf2b8e1559e3eb817d7387ea3c5295c756e36f574fb5b7f05d9ce136cee3c5d1db9dd13f" },
                { "bn", "f5cfd7c44295edcffb39026a0e9e666958f1c443492ccf4cba854762dadd159b530d7810a49804f2f68fcc61075c6e7d24a2ead29be329a07a30e92380dc6683" },
                { "br", "5f0e1a39c41c1cdf0761204df1b1a0f0d97e86f79877fc1bc662989179a7541860f3f83e4c3d35c303daeee3bd5c0df58260136a7b6e50ebe487c28f03018ddf" },
                { "bs", "fe965870bbcb1f055a4677707fbdcc289c3c9828da90eff540b3120aa581243b517ace079eef74b328d6409c44be38018f32b9c2d9745ea70fee7da0b56a7263" },
                { "ca", "264b912ffaf244ba650b784f3a9b0a215ebede75655c1cd16396aee64b907f8c882305ee18ea84fc5726407de0d8025ee81b6562fb2f35f20fd5273fa5a0f149" },
                { "cak", "aae9a52149810a6d78b49b5f797aec16efc82cf413301027f93aeff3450ce5a4667b522d1643ad374a497cfa8ab962488301e0a9dcbf7d4d14272c59803486c4" },
                { "cs", "11d21ed5f13b6bcdc3bd6cf97d45c09b00c24eaceee0c30259366b432c259b17ce139c547ed94b86049ff006d8f754f94f5f0e4ea8e863682e3039a330350162" },
                { "cy", "7b563c744bfca0cbc1388a3ac5fcde4571d32544388d50aef2008f1e37390b20e2c3464bc2dee438245fa543336168cb30c99301db5970780dbf7039e83e707a" },
                { "da", "73e434677d2314e288f31f1988bb4476cb02461ac3227df15f12a489f94674e86710d322f83de4f6ac44b70f60ed4d2b27084175fb060786ab941bdc917378b3" },
                { "de", "1ffd3dd1e31e83938b699be6f1d61279e6ae95ef6708f9053ff82b094f9836040e54f1d2c7bae554c09acd39141ded2cd8154eca6596c414dc45179333326427" },
                { "dsb", "886247c222ccb12b6f55ae157152f6c42ed2781e6d0eb2d5752532d4507ed7928acfaca131fe0b3aa7e3e8ac10273b91e596a899fdd7a3c30eab80a9f4ca1172" },
                { "el", "4ff92e5ac71075383827459a640fbb423b006eec2bb577a7b3a25103ca1fb722b64fa08125bdd1e3c3bcd034788e488894853a109115c1c283f8335a9de49571" },
                { "en-CA", "489cea8162349dd40124dd8a6ff33ea5eabebd209469ef80a22122742bf460b2dea7f632140f3233b91349d8ecc191d9af5bad7c3668e48dc5a224c2c7e9d57b" },
                { "en-GB", "d995c2019307b00ac61665d8b83b55c2e25f40b53f7518819b2180f7d3e59bd282dbfff55c85205bf11c2f75513aacfdc09d42d7eab66917feaeb3d7203b0019" },
                { "en-US", "6f6427b2f000a172b7b9148b2f333d3f5367d2f5397ca4c7d8bd2ee2807230f7aa25fa5dce4bfecbd9d7bde4a7083554fcf9d797e0bbab899b03430bcb8bb2f2" },
                { "eo", "c64e619823365d84daa30e698b889ddbad4a1f4c6a8c003fb7b6f2a733b70f81fc13bc0976928726dd326f22093df8f7549dbd82c2238219ec852663256c7c6d" },
                { "es-AR", "a28fb98790b42e76e3d4854a7bf888342ffe3c569ab87d2ba8efe333be06016bdf1d3f5a9427e9b85eb4ecd9e3b865f9fa165de0c9068d8aaf220c9b9548dd97" },
                { "es-CL", "edbd84c70407083c977234b3f8bd0a1f4097904895476b803fd889f6f46a307cd46ddc8111bfad397ef7279dad150b1212c5c5fd8d0d86479f141b6f421fe041" },
                { "es-ES", "d75ede8cb4234d2259b960b1146807a8ab8ead7802dc1c534387819ec29ec5b8de2a60eb304498f509c483ed0e8e66054718268549efec487e5b20fa88c42947" },
                { "es-MX", "979ee97cbc1ea5711f78d147bae4e6f27aedd2dc29bc0f35a15a49575ef05f46bdffc0741701dd179a768c5b3a1293e72559707bec3901a78af152a8c8b5f1d4" },
                { "et", "fa47c8196f4b3cf26121d145553a53bc78a4b84e95aec5144b42afe01af553f498434768a6b58af7bb40241d5cf00261aa93c41b97b54ca83faf1ba44fc0ea86" },
                { "eu", "21afd135bcdde10adb18151f4f2ad524aeb4841f7ba23903c9bb2eccd9a62731c8c106c9ecd34df113dc6c3236b9485338b43494c2f5653b4462e63e78060b78" },
                { "fa", "857990e5af598b6c2eff0c21100990a93c5d5f1db47511759103aedb5064e507e9f66d411fd38599aa1c97b074f2fb4b8387261cde6df15fae9c3295a091d7b8" },
                { "ff", "0eacbb5667ee40a4ee01b23b72ddfa8c16a89113bd38e69fdcbbc74a8485675516d0c3805e97240eb33c359f57f339b37ec7bb2a63e299f4e6b692f2947e001a" },
                { "fi", "b24eca120a4af17f83b1644a0f5c67feadfb5815598d8eb238bfe8376ff65290f93bce155b9f5a6fe541b47d6253508980a2c0dfc5b375581e7ba3b937edea46" },
                { "fr", "7e397a482a17dd628aaecbe132638a555ede3ca3fcb9186bbc74a2a1f299e2ea8d179dcf83fc60193d0239a5656a3b92b826c83de814a11934469fb4143a150c" },
                { "fur", "c156c828d691dcb3c2dbd1c5338519719659e849603baecc51afd066102c8f3410a2ecb84716e513e977a76ca17d593057c3258c8f261b73849a652556e1392d" },
                { "fy-NL", "f1e2fe84f6f3df1acb2e04d0bd5778e03275027354acff5889b38c9551b21a7364c895b51772cb47e3ed4f1b187376cd23c1062d1b612ff65ca72c10d4364ec3" },
                { "ga-IE", "cb22deaa345f4656eb12f886bb837165d8b1023b5c549c45b43b07a2de8a8cda9be25e76ef7823776c31b57ffb52408e2f2ec2b41cb7362807368f894a98239e" },
                { "gd", "868fb5bbb10310f2ee18f021f59b1f3f73bac831e2a2db6bcc623f61d314cc6f6515bc296fddbee221b3b5d765a3893973f5335c9fce229ed81541e4a24eaf0c" },
                { "gl", "4a90f14ad00afacef9d86c30ba642b7537ed2d2252748808748d2f233c859eafb2300185eeb81f86c869e9a138ce62132542bc6acf81c3cbf212e1b5313c2695" },
                { "gn", "166865e2d0bf025574c9d0ef6cbfe8d9d004c61d733d173e52cc390bfdd32d23e03a25668e6389ca0ae9772acd0cfd3842bfa8ff1c9ea939c273c95535778b31" },
                { "gu-IN", "d33a9ecbd043368b7b563948560584b8e922bb4d3ea5831a592a86f03c28da369a026f97f7eb70691c7a3c3e0451ecbb949a34a72e93c929cad063ecb090b89f" },
                { "he", "51f8f4504d2f54d5d664f5f12363096bc525d24c3d46a731664261021725faccdac58311b0fa8483d8ca7611d32c2bfc6fcd4d890ad1da3675b747b824955e30" },
                { "hi-IN", "a682ee3cd0ccdecc7f79a81d34c55bc0b11de49c7582d0bbf19d24d516880ec4d063647facf722ce978cb336c2df83665fe50895ecaff2357b438c088917dfb0" },
                { "hr", "1a2098c05939c1a8a209f3abff5bb1d01fb1bb6354e574546b3cfa91fab0bb41c13b7e9bf59a9479b2c31e7495e042cd3104993b3d058692e940a60fc76d5709" },
                { "hsb", "d1aec00f1026c362b6e4d8262591329c40ab2ff1a280944c6bf4dfce2c321601b8692b3661d5e163ea14aac66928d8868c8250e1cf587ff5b7f8816cd23ef9e2" },
                { "hu", "af8ce4f50cb811152a5727090681578526e49380320a244d424f74262637dba7c4a6bd6de95021f868b6bbf29ddb8d1876ac6cfe8bf9f4410e0fa47348283dd5" },
                { "hy-AM", "24f62f0aeaf3d6505a71f157108cd609b121aca547d5420996f558528cc70ffe0b66e05107041446e4d1a1c4862d2e450a83111977e8083b5beb753a59698f36" },
                { "ia", "b1d346061d69918a462cb4a3132e585685f949acc8edee0ee59764329044ab424f6035a214f5342251ab0e49d1e16c86a982b966c15d134eb1b1956b7e375afd" },
                { "id", "1c65558e23882a6c552e490180847a0bcea3f919bc353b5892a34c7b42a7638ec6bc7d37ea39e2d15d6108d1d7827f4e1a6827738fc685c76370dc84e619fd08" },
                { "is", "bb5bdc90e749725c052f197d6dfc9f14fe6576bf93d60aff02cb21545759a5335417695f2b52b368ed72b73093c3076dc98bd35ba5512ea70bb3dcecb1a9a266" },
                { "it", "7c880d7f992b1a610ef9134a30d2c18d086a18f109f2dc7773532c53fe8fab737818b4a8db6a9a49f028fe64cfcf019f3b9db2d2e93a78c24f48422e544ce7f9" },
                { "ja", "caf2f985ea315f8dd7cf27eefdc82d6d32b3dd1a301490a49fb1eb3a91176f90301e56b9a2f475158c46d627d8a63a778ff9d61bdefd432d9dc588a57fdaf0a6" },
                { "ka", "14e6bc2886e91eabcf0c2a235d295ab1817130c8a072d56c39e542249b8b41c5066eba10e3ed7a4a5f7dd0aeedcc58e6844209d7939bb7c4d13dd710f826f1a5" },
                { "kab", "92d5890fd392dccefb37fd5cd4fef738d3aa74cfbd1efaf619d43c1b1e4deb42be374b8a26bb89dde1051d420d5eafd1e71500dc8ef2e9e91bc067f5c292a79c" },
                { "kk", "93462723daabf2f3a8bd7556997df9de564557eb68dcdfa59bd45f6ba008dd1a1f27170a096d749fa03cdfdc4967c04af8e484516537a6a22f1c1c9febbd2f50" },
                { "km", "1b15c15f34370254821edab7df0b189c0132e950998d245367abc5ddfebb17c5119d96363b117b18ac679b68368f592b093c230d7d3fd68c43494b8f54947c37" },
                { "kn", "3c49ecba69877add0472141ead3e759a5f24caa1d719149e8b57aca549a53cd57f18cd826e263e1ad53fe8c8e088c5658c3ee57838e7be3b5fd30c21493b7f3c" },
                { "ko", "6e79644b3a8d60463f483bc919068ca47ccaf5cf740ad8ec904f5c9ced406667c48867941391e4d3b795063e9dbfa896173ede4d1083d87cccda5038fd716a2d" },
                { "lij", "07bc70e0d2e428a2b9556e20d23dd2ad92c7bf7719463987e2ca36907a12102e171a66fdd1f44047acdbd79897cc443c79a95ea4976f6e1202b08c160f786bcc" },
                { "lt", "1ea74697e795daa24d839c63dce49f708034a33383b7ab844918495e2344f117cdb15ec4eefc29c00bf3b22b88e27865eb7c4e78b44901e3b0d1a3ce0dd396af" },
                { "lv", "05a4be2ca5f6fd06e9a0c5c1e54169ddcae8ff5cdc73b8375a9593df3d5c703fbda038dd6db0fa24b09bb4ceffed248a1f8a8f01014c8d98ac342a0e1d932f2a" },
                { "mk", "124dc39262af8133d6d638578142ebd77223b1bfab72b4a1f8326aeb66bd1acc0e9c8239c97583b8883360186c51c4b443dc5aa6a1a0f2d3820843ca7248f644" },
                { "mr", "228f7401db60434031222a1fca24dea73000f53a749b1c765ac98022a921ed5e7b196b507f75a1e4686d289233921037be690615025bb96462d18dc29007187d" },
                { "ms", "ed229a34ef9589b60b9614c9f1fd01998b1930059b0a98969c2704ee729fd794f0f557aa4084a2022d49e68f6d691d5bb827400a4a1ba7b1824a815481a6890a" },
                { "my", "277b89770949715e097632d33acbed4ae133b0dc63dcc480d0ca090251e19c96bc84c57e1f74a82cfec518425cf6508695c88b4d6e3dc5ca72913cff9e9d20ec" },
                { "nb-NO", "aea6b366c2546e8d495209343b64b8a3b48ab2db4c8759837d9d2e810ceca320bfda352e9e8af34f291dbb33f3685d7861d605232a297f3799d50e64f8d1ce60" },
                { "ne-NP", "ecdedd1db5e03b01798718d8be89b50832ff5c2b70a14a67ef8cef844fa2f8c0ee8bf876b51746ad86708e838f2d26c25ff6fea8c370409975602be273696ddd" },
                { "nl", "8ab135e9c34fbe3e75071cbfa3193e483f170e5ed3493c8153313a225398e9a16936c307b1d586de3b398b02ff1c8c6d8c324cb8bd50d01214d2d78a7d920590" },
                { "nn-NO", "de56ed35540cc7b13c9e6a7b5bd1b0523185cac68fdc843e04db72dc54dc91b91327f77ea2b0725dc46360c6236668f1004e5e6cb12218eab135e1150cd9cd0f" },
                { "oc", "e70dae3a9cd608746b4a34b88cffc7c45e78bb1060b40a75ec3e072d2d9257fc37997d10472333699d15d46ac4702a2a7cbf917b5e43f5bc83bc3fec882b387b" },
                { "pa-IN", "c90f8831d98d23328b4aee41e66bacbec5d29a18436590dbd776131b2d9d8ed939e54269d34bb9de40be95024f6de0595cc77d24ef260e1ddb79cc30918a02ae" },
                { "pl", "bca25714c9c883fc68f0e2a0beac022d5ff66a7394152a55756e5091d367fd8f4467c7260960254f2ca5117243f09efe4780a56023d4ba47248265926d0f618a" },
                { "pt-BR", "a783ab5396252c3e0c2a3ee3ad5cea037783270d2286fa8977ee406a63abeed4e71a52efb7f807251d7fb5fb3b04538393f2e4023f6bd02d76803dacfc708c8e" },
                { "pt-PT", "b533041d753a59f32f66d1420fd920fc6793e28a7a6e8f57b9ff190a5bd272a9ace4e7c6239312598303e818be2db1a34d1440cab27f95d741769aa506c1a94c" },
                { "rm", "44615b91259198c88a171b313ad6002b7d7136e1e4e6909628a5e0b460dee8c59c94b92f216d32dba5ba11d69c71f41c6855de56978fc7f06514abc29d85da86" },
                { "ro", "1c9b290cd4d3e54f08ed7e5a1f8aad6e7d3a1131abc1f4166be9d8eb5f525952fa70ea565e236e043cff052efb2c6054fbc7aeaef5b1926ddd80198984b978cb" },
                { "ru", "6df6cdad532bf0e9ef36a91e2cf5412b2a36ed09997d8584390856566a289ef89200ddbc36c6c716c6e160915a8cd8a275c9253b2103517c4a7ccbfa20dd19f9" },
                { "sat", "757ed2ff136ddaac7dc27f534411bfb74be33d676661fa9c59f551a60068ff27f5280bb30fa72777b79fca193202d8bb4e490fabce099096debd1e77bee224a0" },
                { "sc", "2973d62dba06b8bba3ecd387354edb6193ccf9752f4bd6117a7de7976fd526f9b32c1d768f3889fce992ec19bf2f5625dc5045afe1a10a3e55aa5e0e0c377dc8" },
                { "sco", "7182a48e8da4b05a53f01a76af28e2661e1a0c527cc95d39355d354dd414d430937c27ca228ade1bf53377b94f84319d05ce1c2aa90024dd7c90492960e2f0f1" },
                { "si", "8f36c85a086e3e323abe99cd2d73eadc2236d24eab26a60eb949e64d5712764114201aea8d88e189e0216e0b443be7a569170e37ddda97472785c07a46f3853f" },
                { "sk", "ceb4328ba06316d2ecc864be62951dc2f9196a40b1263eb4077ce1bdc9fc39927bf4ea31a44ddfca56abb90308b479c62e006bb44872072617017cfc18586245" },
                { "skr", "881c21cc93ad14cf336510525a373dfc04626c4f3a16511c81e683cabf00fb7283176cb0c4d65f9afa0bfcc8825de4a4a0d8e71720810af9164ae280f92b720e" },
                { "sl", "4f30f695f2bb87a251958b68d5ce4e5a7a2955ad2731a6c2a44f3eab00f49ab868b51809dce7711ba34a410abb17cc1953086b37f313afb9cf006baba3e02c95" },
                { "son", "6025f0b68e4abbfd49768565247769bb7f448572ac4c61dd8c7151bbf6b7a9f69ff4ab3a302c1273e57fe5b3a41f4d883f6adfd68d24ab1725790e2593e67f25" },
                { "sq", "b5b0c2422a8bda7ec452f9c2a43ef5d0bbaf1539afd4a88921d4e48328912c7b0fe189d49c61025b3c07f304112faf862d6a56504f9027e53677cc1cbc2da573" },
                { "sr", "105d1721fc623e90116b59816690ef95e9b305dd5608d504614f5f33cef76caba647b76a1f387e1097cff2950886c4ea64b540e759e1bd8c824bec853b7db112" },
                { "sv-SE", "0b1f4d665630e65047ba836dd2e747233676c60cf17ddd0ead5ba137d9a9a1a059f454c45e25d33faf91ed3bd55728ce614047000ad836d772ebe7d45c5430f8" },
                { "szl", "692c4acc512614dc310fcb5d09417ed5e92a11b92ce91ab3c2bd7830b400e66a56828cbc28e62e2092afae5750a2cd245a9f251de5939ab304226fd9a85e51ad" },
                { "ta", "56011463f7fdf5b1d1e5db0c64241f079d563ff0063fb9c3db6e47a7ee821eb0b438e796ef62f85f07322eff733e5371d24a14c5512b9181ed571bf45f38cec2" },
                { "te", "53d621f68e634fa2e18fa9c31134b6c40cf252a58668ac2ee06bd1ce332127495a4194318fc883b0c76b6d54ffc211e78a3c15f3b73cc18db09a585fa8464153" },
                { "tg", "c2bff6ae4fbc8343a445014e45ad7aac65e1ce6cb9c85786ac5d99999090f621441417b7ec83ecb178392b5d0a994960807cdacb471304f9bdb999d0be9e0187" },
                { "th", "6c615611288e1d533a1102645bf53e7e64c240b5b425e7a1dc034820ff91fbf8fdb33486ceeb14dee9e2a7e3b0069221a197bc4c70487c23e0d5a477c41ca178" },
                { "tl", "44aa6478203ab6660e4130af149e84e1a7813d47ba3f062c7827c78f69a8d22776f6e879327f9b3e0e0485b50d516bcf36a54959e589d8dd11bc0edc3bbc2753" },
                { "tr", "e391fda341930ca7393bde5b1158657ecd4eed40967083fe44f938f48e068ae34f6aff6f113b691737e8f09f3f71a5e4a28e90ea0f7e6c5689404deb55d5f24d" },
                { "trs", "8e0f8e64a399be5c6aaf265da974838d26442bc2532554aaa0cf85fe9708aa7e4d9f1151166c339ab03e120258714c0c4ca5a1739517def6ecece26ebc4e0b59" },
                { "uk", "7fb33eb78a9493eb40957aae1f9b86f1087f70716e9b3478759c3b216bbe2caf1406a3940b47e8077769d94d5fc665a8c79a78a2ca3a828be488eb8724a9e43c" },
                { "ur", "36b84dbb047c80f8cd9cac842b4c7e9808a595d0aa8086c31f6368f13d9c02333122040d1947c4ad32ef58e6381461fedfff156b9fad551251a45a23fe74131c" },
                { "uz", "2352d17b0f346553d278021a784dde17e01fd8e2632efab7a55e341d8deb3bb9bf9ef8bd87f68ae9c7020ba231fc1bd2e0fdef80e2331a4c5a2794c16fed8cfc" },
                { "vi", "8f597f2b131853d9080ae3e79612824da1a060d79cab39623b53e55a9e19e11124ba4e8009638d81fc8630cba7d3e297176b1f3fa3a5b08924bdfadddf9e454d" },
                { "xh", "0ff5967f6739d750f2eec24e558c924d22fbc7dab68cc9f9b034325c7b88972f0cada30eadd9a8a995bedbf8491e39d509ff58c2f361b12c86e7fc0aa6450e84" },
                { "zh-CN", "e01249c1e4a28c878015398f50f441c322118a4ddf5f59e19e77c7072610d5a5dc9d4b7f68d2c75f2ced2ba6c6541ec77e7ca4aa09a6734d18ce87b5b8d8d70e" },
                { "zh-TW", "63577704d167b88ae01c69052553edfca7f3095b3c458f1337c0727b4fa4d96d6994c28e9b5153a9280f439c9e0c36a9e36a60f24849ae5a461fc231e82e9d6e" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64-bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/135.0b6/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "abdbefe65e2a6878bc7e798ec7fe3ae6d198a6d7a82b1d6848bdd364d728093ebf18409a025d92abc67e6956602a998071e72bfa6b923f2c8ce81c373fd8d1fb" },
                { "af", "7194572939c9b30f5ebf8b018c12d9b1e2557e59aa5e08dada2ae1709f0c75330ce9688120cbfeb764d5a39342a0dbf017c5c7f0b3a458d247a0e2ea5fe0b4ff" },
                { "an", "c3e6fa49267e915031db100783a3795a333944ab96ddba868c010a77afb3b70578d0409a3afb20ef1cec6ac981f42046ee38dbb1963620e19ce99447090cfed5" },
                { "ar", "db1c638c001c33f01c8073e7ab518c2b6c217a90792b02f22f8126d35cfb8f14b4e218b3444bcee2c035d116afa8d8b498703ab6589372f0c524225cfd6c4851" },
                { "ast", "7ad95608c982b96485a09a14ccf8b344d4e94258186b9513e42d4532bd817a37a88267728db3583ca94c601c46aedf004548703cb506a73d41ab22bc1417297b" },
                { "az", "704621ae7a95821b5c8e68f4d70780dc9d12755861b167ecbac30be393154c454944791bf952e9d4561164422c398c34cf092fda2c65de888a38f335862872b5" },
                { "be", "d76f55b55b65540d0ade279cf3928c9d8c999bbc662090ddade65d6e91ddee4698a549badb81b276347cf5d922cf9b6af83a07f659cf7d537825308cb3eaa8bb" },
                { "bg", "40cb65a681ea935bf5be583f484e0d88dd87ce5aa10fbb730e6a55ad3d27a7e0da3fc8c63e6cde86b6c2aa1da68b9cac45608bd8fb6012ecf0857814bdf39ef1" },
                { "bn", "b6a95083d86293e85d412514737d47fde846c1d6a91e99e71a3c8899586f7437a4c3d579114ed5ed99a243bae43ae15eafe88e79662353c1d67fba1d0a5bbd6f" },
                { "br", "fd24da52fae15ce2c1a3db7ba1892dff6efb5cac8251abadaf49abe199b0f4e58ffdb714e774439c370da92554514436e896d9aaa191a3a7f9f8c00e116b7fd5" },
                { "bs", "86e567ada731c522bc73275d587cff7f536f8eeeaa518bca51a9fd4039812cd2bcda0566d3c9ffe4618e90d24eb54322bf3f36f8b86d953f93838469371a7de7" },
                { "ca", "9cf2cca1224efe30474a890ffdca1de7b50b921a8e45a4aa92b258cd1e06b8b88b48ec44c7bfdb1a793f76b693f86e5af11e2854d624e8705cf053a41f6af9f2" },
                { "cak", "10a7ffb2594fb7b6c086ca97c644b17b9717cae1a16aff557f9b1f37a34c79a255d02f99a40f4cb9cb62da3482f100da0a8c76fd8138d7ff1b98b0cda1da2212" },
                { "cs", "861723813d38ba10422cf7b7ae01ffa4425fd28763e9ffd605c21e8a2cd63c58e26406a9031a1b3af0a7faab17d6880e1644e2f8669cd54b4925512da32ed247" },
                { "cy", "1029d18ea0dec5589aa78f484bdf05a686bf310ed730e0bb71ed91f8717d887371e1a1945698eab124314b86517e9aaa3e7491f8427aae5e1e04a409a09de7d0" },
                { "da", "5b7705b987c493fdd0e1ff7b4979ae31982d6e92769cca6a270cf0fc684c1337f97f50ddc0c8a73aa89ffb6349d239657ce1b4c2596e0e786913f74c8087bb43" },
                { "de", "038876fac6bfee522d56604ca8af0eef1a2023a8af2febcdb321a9a797f86d48a88e061d4bf215fa0370be3df3c421f8f11c0a4405788848f78770f1d6b72bfc" },
                { "dsb", "5c23a9187f944eb2f377b9a4694b658d1d53e18b2b4500f58a5ef7d8197f76af3738d13d910f5129192e714a580ede993cc2664a5da9f5c55770760b0b60ba9b" },
                { "el", "a5ffde23ba1edf850719c9d1f149a73355ea0f63d9307e06b58510f77b64b7c358243a2f17d4fa697f1d0359c457e4aadd5aa7941315d40860b1cfe6094335ae" },
                { "en-CA", "0b1d8708531f49917a7d7383d6b65a0e33a4c7e998a3c0554b686246220aaa3bd6a7730d5ad77198f6a391753ac1a697b87c8536cd927aec6f23ad1ac80a91d2" },
                { "en-GB", "a14a12854b5723c3f63c2c7e21df42dd3f11d1ba93b3404220d38f5e953661020b7d38f6c67dcc85616bd6df90a3fd5d972c597c3bb2b537c1a937793957c252" },
                { "en-US", "5bbfde20afd0fe3f87b93db53bc17bfb20627a4581668318df2b228e7126a9da631374afc3b5bf150ec1f50b3e46e7e4a1334d60f24eef3128a7d8f004a6a054" },
                { "eo", "d31183a6fa11478c3501ffd39c489ff9371411d9d44b609e098371625f6b39455f918a724757e41ba5ca71cca26f1c81b57f3859fda7f5b4a641554fa676a5d0" },
                { "es-AR", "df07536e52708e58ce662f838ef6edaca555ac6f566b39ec69bf9c8f78a4ee46b49fc17651b9599a4150e1252d19f71b95753bccdfc8c1ea8e98302ca7277509" },
                { "es-CL", "4c285dd12af8d48a3a9f963a8df7704064bebb2572d99f807ff8b8e09a1c411f4cc7b9831807ff87f6abb83964d5ddb60a6ecf1ab394fcd4750f9aa66bb55b9c" },
                { "es-ES", "5eecbaaf3f377e263a809df8ec5da4fef9b8a637ef735aa75c737899600a8dd15963cbc5e5ce726aadab5105f840df7c1867be33298650e10cb3840db3d03d85" },
                { "es-MX", "4e125947c49bdd94fb86ec7d9d04374c246614274b46413603cfe40c46a535f5915ec08970ef35cbb1f567cbac7ba3b8a69005fa5c6162560946292632e89dda" },
                { "et", "252a5b07990d6f77341dedd7cb530bc8aa4778a3d21cc542af3c5a769cc36578393961ac03ffcbf3708da4d409ecff861abd77970f4c3d316fe47126100f5382" },
                { "eu", "21001f045a9265b9ae5e55b451370d4e6adedce0a4a2615b67e44229396317540d86d081177fe6a1605cd4ec598f7f4e38fe8af80b72a886c652d38a68771e80" },
                { "fa", "8e56c4025244a5eb486502bb0fc744a2d256c165c7518de34df8b2ca2a56b7da5340fcf592684c6b52f2703ba68de1644c3c4274195119cf4064d767728f57c9" },
                { "ff", "ecd56cf13a02cfeef1df4681f2544038ebb39a1d1e2fab3b0582c3db7fc60d88aeec1085fe4983ba9e6bfe6302dc53242bd8f8b33325e2ad11f26cdddf3adcf2" },
                { "fi", "a049576bf851b363455ee143cdb89adfe554034c7e5ed725a9a35e720085485cc2e7d7785321bcff59cc2c7dbb5a5224190f49501e2e55864e1f314bb2fb93b5" },
                { "fr", "306ce0100b2f381facbe9dae71ce8e6ca3f1d0b3123f8c940966ec59070cbb423becf2143dd86aedfe4047d16ccdec1562261ec299f4934f210556fad01923f1" },
                { "fur", "5a1aea12e5afbd47e99003edf76aa32652a9e36b66aace653c34aaa8a6ab5bd1df2065c7552f55c66467e0b20d3e36c17d6343530db1771cf01d389ba9c62858" },
                { "fy-NL", "d89ee0ce7c35e7f748ff392fa3c30aa6478d2d55300a75820cbc0e594883114659e067f8d48861f4549bc22a91a61ceaa2f45bd2af55f6d75a2d2d1280ce6f95" },
                { "ga-IE", "99935445694ae84a7e3e887029a98ea9d856c2d1eaf0bb12fdc634920030bc88d3f2067250c4abce2e659e147e5021cbe85e9dcc719fb4303368d88723e8fe5e" },
                { "gd", "55f364c778ab5ddd8fbe2e46c1af8fba573a1e437889bd6e46061ce6255c4c8b47819d520b3a6c7513062ff9a5e3dc995c4ac86c0e386eb3491ad857030bf35c" },
                { "gl", "19f4b2370b1e7072d436a52373ca74141078411c5da25f6749ba4a1087b27ff25bbe372c03292b3ae7dc62c7ef68b344029e4038cbeea227d70bcde124ce5396" },
                { "gn", "9f9cf5e91b38fc02ea765da588b571065b3dadf06a74e5bc7fd00567653a0609e3af576f2ce7cf3c5022f9833b8af2dec2030141c0765c80979b1b9c7652a471" },
                { "gu-IN", "8c32570dfb8c0eda82a98ffd24944fbc7bee4e93a7c7f4804b6c2f4d8af21a64e24528697fbe18ce66a4e2a7993bd49cd8beef32820b7be7aab7f7f55e8f1106" },
                { "he", "84462941bbc3ae8583936eabda8c10eecdcd7227f5731c3f464dafd23f4069afa1fb1269cf77111bd1080389fc9bf40d42f0514cad73c47ea87ebf43661143ee" },
                { "hi-IN", "aa65e7d35f9fd08ff3fa6a01c6158360401676f0da24f607931bad6ff11eeabb452c1843c5f5f1a0e92002b5993c9bad4e880ff969e522dda02a2c1775cc9c01" },
                { "hr", "539418dd9a6cec1df43f9950ed7cb22fdfe5e5d79b78ca2f587ae47e6d530469ceb094466917a6fc234c95ce63beee1c17c1602c1733883b8f034b0cc734c525" },
                { "hsb", "4ceb29c3c6997fe32497d21a13be56b87df3b0237a65f4fa0129b12514dcba5094cb5372fe16e5420b4af8e301d77e4fdd0609359cb9053ebd3ebc903a759631" },
                { "hu", "e19e7e83e0cddea6035d4f05f7a538a96fac08fff78c692fe9637009ed6a09bb3f3e091c3464e40041605a990a98fddb7d89d86bb9467b5a1765980e0d3e0161" },
                { "hy-AM", "0e959c28b6a4fa4a0d413b50d3d72131745f30450c9eb878f32c0bce2acddf98e64c5feaa1562f967556cc1eaa916dad7d2b487696d5016348a70e091a1c47ce" },
                { "ia", "8e099cc0f2a3692f3f2691505303df777fa83af27336c4427c454115db9459a7de821df336bf6730ec4cd3248f162df99c9d76858454ab6064bd2bcf7b2de021" },
                { "id", "3936051d23910e41e4246f70deedfbc893215672ee88f42b39ccf7bf8d2a5ae6f14e51cb1aabb0950581c84631fda0a5f67e7598fbb820cf8cef7464588d5b84" },
                { "is", "6f1228f1b791987a222a84059636f5392bfd91f095179fa41f8fa5793b88c86b29c956c3a2e3b9d2badec4260f667aa7f2ce0037d4daa4b92f65553d0e9b9740" },
                { "it", "786888fce7f8ade07c3b8c4ef1ba384517b5f77ac46565a8af26a42339fea52948f834ff48fd6b4d4e68b48075766cb27b4e919e82e0fbf1287d847aec7da891" },
                { "ja", "7193a65396a6f9468f06302813424d72d3ccdd9093fcc37b9c6f84f50a3279e1393b75a641c4b544efa2535680bd6ea1a45e39c695a1284bc3353c5fab57db39" },
                { "ka", "d1f915173f5646fc02d269a92a8f08f71b2dc1e372daa15f0274ed137d525dece9aa6369a6a94d30e9296a01d91e502e1d446a358d72936d35f2fb1047e3a2d8" },
                { "kab", "cb60517d78025813107c1163d74e348d02e99444d06b319dcb32cfca7bafe413e434c204b81d5cef49517ffadda81e24c8192017623f37c9ec996066f5979e66" },
                { "kk", "bdc10186eca8299db40fe7153c130736c70463c58144f05e2c12c2b578dbbea893181e07519c765c38bc100f438027e4a25e9be2e7f19ec7b3142208f00f8517" },
                { "km", "ff3b6f1a9dc1a7a8687363f467a0cdf6b2f6055d2045a42130267cf427e74e08391bef0d8cabbc83ed36f409e98a7996ef159295e63593cd3fff245dfeeba737" },
                { "kn", "e423210f191d010e88d048d61ac5d2c0c44be63d215e4dd692df0812ebad719b0ad59406b56525fac949e98cb374a7b70ea4585fbd25ba20fc08761f63600e92" },
                { "ko", "4cb5908e9a6860a80de18a366234818697b3479c268ad4d302dfcfc27156b5aa4411726253966ae84d9e829bffca46234f7519e0451c239256fa1ce9f4a8777a" },
                { "lij", "c8e2c315005c4ee57ce0963df7ab973c5debca6db1b36bd423a60f81973dbb89bb3944930ae34df9800500a8b06dd02aaae70ef24f6eff9e8b45dae7a7710e5d" },
                { "lt", "97c09629df2f1f61f1ec3d9f496caab2269657992ee4e6d9e7743cfc8805ea8b27bc9880192cfe00e9261a1eca77d1ceb2a952003764491807a5cbb3c5915dd2" },
                { "lv", "c14d3a9feae9613095ce98d45c9870374a6d1e2d556e43afcd540acd7dde2c346a9406c2a89cbbef84d2e4e69da297fbb02e92f6140ec5434dea669b5777ef8f" },
                { "mk", "98280a06e590c4271fe6d46cd1fc9a8e46f3eac2c4f520c296265e0b616c65e64a459497b3d203e96838d9774d5bafa2dd76be7c9c35f1334e03168d60be3a79" },
                { "mr", "0d994a52908b5a042c83fd24cfc94e8f55d43b478fac25430e32adbbb21c9088bd3427f4f910863db7f10f886ac5e0b816054c94160f2e04f6245b1ee75588b1" },
                { "ms", "ac996c35d3b1e5177f621b28566e9587d0b3ddaa147101cb608e262a3ecf5968ebc37a35f5e40a16d71d32578f0e724c112e6d57c66b80db558682b4a01da679" },
                { "my", "e97ec28d2b8d10719e4ab001d89db3c8efeb0fa2d9680ff8b4eb3621b2835dc5ea412ce95539afcf9984ec908b414e5701dd36c29b93e16f6b4ab255d4d666f3" },
                { "nb-NO", "f40cfdfd3e667530a6a3ef1cd8b735b910f740ff2793c1dd6635791c1ad9043a400f3da014fec6330fae84f4fe7904ef7a98db74926d1b94ccd87f34f8d52a63" },
                { "ne-NP", "cde6ed6e07a869e27f8c343481d368671f4038cb36aa153f234305e5606b2b8e256b2a01fc9b372cc7ce01ce6aa1152d70871898a9bd19ba390c0d59c0c757fb" },
                { "nl", "7a61d8b6edc5b7bf6969bb51d0787b26fe32f1145d2b817f9eb7a493adc4067714ecf19d7c6895d8032a1e91d0e3ed180dd84d0dc9e6dfd39296ac6e6e50706f" },
                { "nn-NO", "33869b12ee3af5b68ef2b82544f5c667d9112fa952572fa54bc938a807c6b2ccb8102390d13cee782c09fa6ff2dfb351a8f70617c0c7e9e31b3e1585e1bfad4a" },
                { "oc", "d0f01fafa5fd30a9e52b4cacecd45d9866deeed5d5a2d3c7aca3e3230d3a108844cf053f04e1a042a9c310d00c3578355b04c435a6683c6c292e40f38e80a279" },
                { "pa-IN", "f782145c0f0d34fdbc9ec947e47546bfe582dd3dc35421f73a43d5ff54990cfd634342e3a5fe310ec04cb31feb0f0648f92bc15a31292008771930e86af6eb4e" },
                { "pl", "2c6352fcf22931e5f343a872e577289e7b1b85b9cb4274946ee9c5996979d341da321b64454c78efc80b05569ec0f14fa34c60f90cb55a8e4c9b3ad09f275dfb" },
                { "pt-BR", "43d8ee141c3f3445ed735ae35d6ab980ad45d0dcaa9ab309063b83178c733fa06078c49c413ea42ba2f8de02838831e53ff5928b43c03836534fc64cca6f5c75" },
                { "pt-PT", "483f36436e6cdafe4195b902d705fe815435b258028090c6820f56d2f278ae8e1d51ae4dffc8c0d362febeedbed70745f402a918b217f174231e154569affc28" },
                { "rm", "93c37dd5870e6f4a150ac981541b2c32455d5add89161c5503bc895fb477a8900384d8f6954ee5c69cc904339ece344a964b1ac05dc8caf083cebff48f483b49" },
                { "ro", "46956f9cdd6bc19c33059e5b820bbe5a6371dc5f517fd87d1dd32350009b4ccd9ee1c7b63f48e7d81619885b28640faf5f5ca519f9218acb38c232216c4ffd20" },
                { "ru", "ab0d113ed477514c6283e62e8d5531c48aef62a9beef74eda64864f170a7ad1bc62ed15f27f2484d51a8803c1bc9174cecd721abed880474101c797fc5734921" },
                { "sat", "47eb6ba1e9b139e86ca18a46ef652056921b3f0fd240abb07c77f568e85caeb9ad45a2cbb5bc4b8c90ac08733bb6e5357be2ab9d44cd6b25549d48522d3f1854" },
                { "sc", "3489ea1f150ac91f4abaa8d00dd29adc6dae8deaa8321b36daa69d0b45f1519e00be9b6c30052279266af301a0bc9b21939208e7cee6d4f7bf57cd0965c42e99" },
                { "sco", "87719d2b31547d4b778bc6bc02fc125bc65acc3dc58ea94bb585d4dfba4783d5e6af1152968095297ecb63d14b9ee56f59af9647d92c2332312bc9a629d096bd" },
                { "si", "2f35f1a3606a7279caede5319a85462d3a60ef40a642eafb7c2c953549e117cdfec47d21f6a18f69e3b679b9458cf4ff9d30fac3bf1f40f4563a54f4e3b94ff6" },
                { "sk", "8f7c5ac65c1a1d6fee1fb17a160ec72f4e32227f6f6a6341c0600d4438681fe2ab95df695d9c775e339af970153e2c918fbb841b4d5a8df7b82b5bebf96938bd" },
                { "skr", "10bf613265bd518aae58ba1ecf4d45b0762c8626a301bbf442e6c24534ff7b0b72a17bc24b4e89049a288bbf7daf44665519a59570a75309a3c8f7cf568b203c" },
                { "sl", "837df3b57a38c57d3071cccd200389e05595bc31bda4c12b086479c77b22f303b404367c7eef780fab9a74c5575730eed9d785c26daac3289bdb75cdffcfb280" },
                { "son", "cda6dbab2ad9ad849b29f7c7d06702007019cb3da26a0b3bd11ae6a631070572385de4337bdbea55768dfaadb9c400c2a020b94f960d55dcfd0aa331f21d4501" },
                { "sq", "037d1797f67ee65f341238649996547e8306414040a5416385ca1d62ff68329139c5367fa876879f52f8b94b5fcf19e82f477103b03fa9c457cf0795587d8a34" },
                { "sr", "11b250d770a6923a0aab9e61f9cf8ab593a78fcacbc0fcdbee1d9d8049d71f379f8b914cba1f766a10f292e9eb9bee422c23ef846f45787ff1070184f37fa253" },
                { "sv-SE", "c18ae27e1ae43cd8f5a4d2321cceb205513d3ae9b19d0550d6fd93272088479f823e7b9a24b4b83575d008b6cfcd1f5991f56c654754b7859118bca55461616d" },
                { "szl", "d6f7327cb7e35fabf582895193c982ed0adddf62a0538fb471413411a9ad4800aaa060ebcd7edd944487142402e5b5e83fcd936fb2a9b0462b9cb03a8a60e96b" },
                { "ta", "0a142bc1625dffc6d1380b1a908b8e566d4fedb40aa928ed6d4738b4e8e02aef19713e2f5d8435d5c0cb2aa1ab899a7bcb5d10502419e74132b2147a099ee266" },
                { "te", "3f11b0b1bee2aefde29559d95be1189c003236cfa966a8dd2a86a0fad8a34aad947c42b7cc9779b051fd78c533f15d37521be1cd73ccd8f76c2f12219d7d1097" },
                { "tg", "e39eefb5cea57e1733b1210ebfa80456d98ba2093db0a3e9e1e54ef5ad260ad40d009bd0ea93d80f5b72711a74818a1c1fa861ea94edb3d5cd9ef1ce585a4991" },
                { "th", "75ac0229439a0ea43e4c178f3ab32980b7f174bb69ce224a00b5af32e3c4c498777c64ed54e75e3a774b2d5125fb0854d4abc51893e72da2e0240fc0170b6bf3" },
                { "tl", "190e4043cd6596a22b367b91c7d538459f83ae86ae7908a01f318b08353d8a018234d2ffc9234d33af72655e1c2720d02df3de483441fc481149f19ad68d0689" },
                { "tr", "db737e1ba4ebca3f491d1d0e70e03cdd70708fdd69198072eaaf8925b761104c30b71c34b1622ec43a0220f5d1a00d7db16c1cf04d08e0d3e4846f1710c0fab2" },
                { "trs", "8098ee127b3553dcb329b0be8ec30d4590427cf54119773aab54f3096bd7e2e0e8e2acc4d13a2d03499db7db33b4e778c04575bc0eea752820214c4891c9f94c" },
                { "uk", "244ed0ac03291c90621da69c930d8cc78c4b905ded6a45e293635e5083de04dc297550741288848aa88d9a2d5b21c49190becb18e506a558c28923a7b1c56bbb" },
                { "ur", "04bd8b2b293d26c4f51b871095636b1b1ae653c452bc625f9d11a8946c0194a35a2bac14044ed25e9817b4c01b204693766e61a4c5b7dc3d47825b24112855f1" },
                { "uz", "7169c7119daed27c09a708b5d38977efb5ee5f451c375d5233e1720a13762bf0acf6fb9faaeb68e820d5afd16ad9674e2560eeeb16b1995a1529145c25803c81" },
                { "vi", "37065bc64a46058d95f31b589ad7d00260ed811f785f00c91555c70543a268aaf964e2881462a7ccabc0d20757d74f8a3764820023128f3dc95f35846dda22e1" },
                { "xh", "56ec38633fe081a8320f2b3669ae54a90fcf9a77c8a8196551dcf20da285603e2fc16d1f6ac37f72ed3cc6b52c9057cdfd1e82e4703e0fb71271e179fd327798" },
                { "zh-CN", "6dbc9ae64ddce71cd333e002ada5a8004004b182c331031b49dbd84db8f2ac759e93726da12d264174e1096458c9f9498c94b592d1cd960a1d02fefd172b1584" },
                { "zh-TW", "13095506d48c39f4f8f8fd190f615023a3fc0d73f05ea649c2e0e2d8a6ebad893247bb38510007923c8ec3304b75b4e2b0decef526c13d8bcc33753683673dfa" }
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
            return ["firefox-aurora", "firefox-aurora-" + languageCode.ToLower()];
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
                return versions[^1].full();
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
                if (cs64 != null && cs32 != null
                    && cs32.TryGetValue(languageCode, out string hash32)
                    && cs64.TryGetValue(languageCode, out string hash64))
                {
                    return [hash32, hash64];
                }
            }
            var sums = new List<string>(2);
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
            return [.. sums];
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
                    cs32 = [];
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
                    cs64 = [];
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
            return [];
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
