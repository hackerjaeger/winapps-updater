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
        private const string currentVersion = "121.0b8";

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
            // https://ftp.mozilla.org/pub/devedition/releases/121.0b8/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "f82effdcbab9a6a4f329bdd87fea7648dcb2a82b60ad1226cb35abbda08596b0b3de820616f26814fd70035e59cb4ceaded018447afaa7c21a0b459e9da3be41" },
                { "af", "2ced9e387bd70b9534cd3baa85c304501ce1a5c35c08ec7dda25d25eed42bb30e6d86b986a24f7a8ef98b82653c06731d2861649023fa98d6f36013157cc3cb7" },
                { "an", "15807480a7c8f6de920027008466a6aa140b93ccccc18066ffac9e1fc0a46716350348d1d2ba251c6705410fae41bced05f0f59c0b1d6aaa1f9feba9adc621eb" },
                { "ar", "1bd99f2a24bda3c80bd0ecb2fb4c69b7337c7e8641acff5f0a943792d4acfd2f7aa009a3bac0763c60aa7ebd9e1817b943ddc43bc8e918e9d8022ed4ce9b9361" },
                { "ast", "3882ecf80a9817718eb83583dcd653cdede45c29298e0e6a25fc32271c8e260de1f89744e232ebaad524840b84edb52649ee8a430fd3ef7dadd58755ee1d6682" },
                { "az", "13a00234247f3a63d116db3db0c1a218ac3c9f74f2b5f52a8acbf03fa8baeab2fb9f3915d749e86d210122aef658a58ed7cd1c412f1252ba945848fff0cc378c" },
                { "be", "19fd946a4cdfc8684d74c7bd9644e240e29bb7057d10c970326c19f1d125df2080090afcf0e181d9b41251e867311ca3831425121f53f3e5c4bb3220c6eec0fa" },
                { "bg", "57a10225364b03b6b2f45695c2a1fcaf6442b58049f217b470d3275fa9a36383828c9b95efa3668d0a473239572baa273e25aca5338f7727bb99234a1537e187" },
                { "bn", "5ba792f3fd3a2d896d28955ba97743b090de8e2dac14ca28580a312bea5530ef756ed686f5f7d215eb4508ef6d31ee36167cd1ae20101677fe47b28149c885d2" },
                { "br", "1a2e8ed9cebdf3e54a0fd3fc760d9d69d318de4bb3e562a3d285f87842d6fef02cfdd07fccb38d3668987a3992e8846a15cf6f513851e22cb0c297398fc00aab" },
                { "bs", "644d4d5cbe714663f7848b323ed26072caa3baae55a5131e373b2a982dd030d1b62932a40260a8e224e15b426bcf0cd3093a10edd0f4c9a3c73795af54e76aa5" },
                { "ca", "8af66e8f6147065a20463894c8b5c2202e491476cab2a8e499fef551067d42d9e181efa3de3f72d620ca95820f03ce915c5b8f54e794e849a6be93ff5de61355" },
                { "cak", "1d1197b24c47ad907608d8c65d3fb19ae019b1b055767c9f533041dcafe118668c9e875a3deefa164efd8248b8d418029b731ab0e1e9384adc29852b4aef481d" },
                { "cs", "9af19a626c336539d9f268be2a270b39d2daef1b5b8c48e9aba66dc52674a5a849878ac7ba591764d2a4759cb832fd81734c269605b924c89bb91ac608a377cd" },
                { "cy", "4c9b4f67427bdedc7af3c1aa33afb5cb9e5e8587fd0516a300e0e23d3a92ef9f54f76a046f0ada155f74f3ec5f81b7b7437fe99f29e15f3445e3d5bdf35aef66" },
                { "da", "bfd8266d8c1d503564c4a6cad7ff6ccb01bf2659a32d0c68165e1d55405f313ceeddb7ca96a1b8f3c9a7db1271700076a49107764f766344e82195eba7713223" },
                { "de", "d458860ad04dc7e754a2e3236897a608dfe65d2969da5cb429924851e52911c40cfa315188cb93598ffca94963406c1cc173e221a27ab52e1dae5e9362cea98c" },
                { "dsb", "e292110706f593d45b7c810713a6e333e8e41165baa2272605e7da3be70457f52382301428f341a9ce6af10969bc9248dc6a9fdba43ef3decd9c29f803e378f5" },
                { "el", "ec48de7cb63a3acb6aee0d31c407c1c291e36aa29d465af20dab6fe80896df929bb59a59cac60faa58f06072d568b80a877b5e2e28d1f3f61fe47ba500b19d55" },
                { "en-CA", "c827d41648050f21ddf7f3c66f0cd1f9740fd1c2da4aa87a6ca6d8d25d42a54f194b52edf2547aa8055c616a671fb9b7bf21481b11e7852cd7e4009b727690d5" },
                { "en-GB", "7c68fe770ceb3651b9f12a76912034ba0eb9635e84ef14b581f31fe7fb4dc37c8977b33dcac9e8866a0aebd078c636c60a2b791079794aa47deb5eb159c6aeda" },
                { "en-US", "9e13c99db36b0a0fd98e938442bbf39532a3c4bc1de595f054740a2b2ac18aedc356367878b12fb67908bd3e894305e47107f6ed38b70a620d4c665a11b4dd16" },
                { "eo", "faa48536c0a9dbb4b5c9e3aea330e0b415a734e970554ddbd9951eec113e59673a4eadaa8feab8173854ae7a11847fa03b55b486ae8dc90e6dcb20932acdbe1a" },
                { "es-AR", "3515014ccb576b5e182a754ed9328187dd6d271f3341f3bd25f1cea673060564cbc0e1bcd1413a6a48cd90831ab416dbd4c11324837258992e5dc9b21682e466" },
                { "es-CL", "090c9347e7e76775a7bfdc6fa71f5ed7fedd48f85b1634072bce015d8151ff1bed6ddb39f119aae332952d585341f3d2db2ff7e955a5597a96451185de45a8ad" },
                { "es-ES", "1b81432089301eb66389bb5e43ee136ffdd5ae799bb3c4121d3495e3a7b2fa304a51cb7f5058dd82101053901257640640e5fa42d0f3f8c90fb30d8f3f96e31c" },
                { "es-MX", "27699b1d714e0e35133da71057cf69140e13edc057b64271e467d2611fb8f85c5284c58b10945aa90bf6478c86f6b70c31d3f6a4577459bf896aae492808a33f" },
                { "et", "3a76c133a89171c0c7b88113aa4775f4ff7dcc7739a86f889a955c4bbeba505b65ae7f56abf218084f2539c5798f44f22cef602b42754a45b9b3813500507753" },
                { "eu", "7695966d8cbd1dc2046a8baeeb6ae09f91ffc0c9605dca941ea226eefc3ed9cc0dc1e6afcd620f22e91f0355479f326e47ff61548e0b3dac19a9999f456bb8aa" },
                { "fa", "80ed9afb9d725f61548ab543122cce84e77a0c9b4cccb3b643d3a9ee291542dca9a510474d5e261da16c735b05d9e84a343eb441276c8ebdc78a9dae71657e19" },
                { "ff", "438f4f74682007648c67c99e7ffcad6a8c105663a9699b0055d5601249c5fcba9b67dc8c146de7e42acd181a68b85d3a1ebb93877a4c804307e5066c9c081237" },
                { "fi", "8b6b7d5fd7f48c8519330cdcfd85c2f5b6ab30f83cfe27bab124f232c18562a7414dafe17910b64c9c50e7c39942247095e1357d664f6909184e662e2a293c53" },
                { "fr", "e71ce86fd334e99627c98e1f4f4e9a918143b4ba3e15a8f7e697f90a787ebd791e050e045f6e5f121c3db3f4aef2b86e60919af2226bf345fefdb7cc291ac674" },
                { "fur", "6673722cf3fdb1676ef8af128300e5a3c91ce4d637b507d1da8c3f41444e66b214498607e832b1f404e169455d1eb5f16641030821604063e9ec1616c24ed8e8" },
                { "fy-NL", "480d70c09fef2ac0570d1547b3e27513a3c56060601700f5831637fcc800a852f0d52f6e62a58ee768b879ac09979c86e8bbc6eda8f4faa021e20bfc674c7d20" },
                { "ga-IE", "119df93e2fe271e8bfef4d109e4412c3e94476dfd5a1170f391a012685d87b122607c59de60450f167062e0e77c1769220b6731d5f7fbb6314ab98a598cb49c5" },
                { "gd", "c28c183c8841e6f761081082752eadc9f0d19eb6f4fbfa5a541759c17bae4280d5641feb4b862f02235f042d7255f5709b30d0433802010b17b99e7cd7e694d1" },
                { "gl", "daf2327041cc91d7aef25401e526b009f775538a7ad4b0a36afdda084fa9fce13296cf50014e6e711c627d8e9b629b8affdca7bdceaca80c4096053e32f71e60" },
                { "gn", "eef3ccabbd98dde95ac7a457692284b2841f4eb868a9d2b88f88593a73e17eaa072941afbcc599d41723cc9190da2b4c04da0b242e096634f9eef81cd361aac7" },
                { "gu-IN", "15c9828f8afc898861bb6030f9c97e426ea2471b2cdb69ada8b8db181dd384bca53edd7542b5a2611b322e80dcf7916c3d1157e435e25ff6931db5b90fcfeb28" },
                { "he", "023c9f5160161e4b756814c51a397a2e8f3001130f33807e3b32dada87118de56c53d9db089b95beb38e921aa06a3b5945d0c5d84dbceac4119e4f7c7215ad37" },
                { "hi-IN", "c1f242cf871f2b416fda37bad4a5ee42691d76e332f59cd9ed2db5257e76a95435bb4490f21125f6b705ff85d6716ddfd0dc53f150b812b359dd75c0e42da293" },
                { "hr", "3eacfa1049f615a87f2498328b4c1f13073d49501166f6cf04c9614a5ed977a5c3e1888b836568d2f768580cea6368b47072ba368f1e19ddf6b46b44b0c51dac" },
                { "hsb", "2d1a68d4361ee8dfdf274f169153490143c995565ada80fd7019b03eb1fec5c615b94247db96ea790f35fe75486f16fc48481db75a96b220a9ac778fe9402987" },
                { "hu", "28943e0a97a2615458bed2e12613e655565c94c37df164f5e8064b7a873f288583698c47bad45904ecbecae121acb296da1e5f43337492e90313fdd3b07dd3ad" },
                { "hy-AM", "fa25398a9108cf428eaade6b690c7d6bd080ef938fae1bdedf55c9132d4282a1ed9f1df90f5c6330d76ee7e6083e7e1df410ca4c4eaf1f5d0ed313df990530f7" },
                { "ia", "edce39e4f05799bc0bc927c678e222605d3b2de1b66975b323ec75de7c07d4ed8aac45c03d5c6a55bd170568366cf83a41fc16043605ed6a8a703884d4d916d8" },
                { "id", "cda611fe2e1038c8db44a130e2e3d63635270942acc396231845a40453413cc1ae77cd5bf14f356b161f0dfb44ef0e079172ec8250b15838b3a2fb65a78440a3" },
                { "is", "d157225babe6a6c11f82fa1f87b74e628fb36bb2e3a2b75c5acd2a3536fd162a41e1082e0d3a85995fb014e4c5f0ed4a026290990ee828cd7bdd5aab079d4a7b" },
                { "it", "a5dc7f97af6dd957c1bbcaae40b9d754bfbbcadb92655cfc133465b109301b1848500ad8ef799988cb0f125633c45cf782a16e25fe3be75d7e9a1c5e3e5c4cf6" },
                { "ja", "584c1e868654abe7ec3e103f72c829d16b09dfd46629ac1efd749af3ca3ff41787561c3f3e4cfe5fce35b4456960efe292e874b1aa177c0c1dffd4f761cb3ec8" },
                { "ka", "1734d757b27ab5b2d44fd7ba2774099f6e01ba9069027f329473dc36a6b56868433731abf9db2c3463f869d4537d86d7f85fae00697a871f85ba629e0ca31f60" },
                { "kab", "c9dc2eb9dad663b267fbb4466530c1279c01fa42ffc9ba5eb38c02111ca3c8b99102975793f19cd6067b20c036e725a243647db01ce34d083146fbbf1e0be207" },
                { "kk", "68b40b8d2d56c8ba34322bfdd4f4c76c6c7e6e81ab0deb7fa3ce6051a3970f613bea8daff2735f258b1f3de152de2e4933f712b565079e4af1f1512ef094fd62" },
                { "km", "a70e19f54f98c806f718571f664c413bd44e45100cbfc3fef1f2f35b3107607f45b9737e5ff5d2727d2fdd26df977242bfcc8b9d56311553330da73d23e36fb0" },
                { "kn", "e6f60d314282d087fc465bb441302166167be7112d308a5d8c897a93ae0d68b6ebf0a889e30bdb467739acc4eb3f56bb61fce6efefa7e3008a46a0fde4d1ad61" },
                { "ko", "0880e2b84afc9fffe40fe5db8e7de6160ac6ddc8b3c093b795ef0e7c47b183fae0c43beb5047ed29ebc9f218bffcc0e516c39f36b309b0dd2d6a6c3aabc35e4f" },
                { "lij", "f408078057d3fd0a47a5d24a9a6996f91a96b8d926c8c792418dd4ff8bcfbdc4d3f974a848f394ed921628b78d9cba4c42397487bd972f650a61ad4f6afc56fb" },
                { "lt", "4c20e0957139dd2268043165e0060652ce3d16f07f7323290f6d6333e17862190e667a6bb181d62289d55195ce400ef186cd2a5b0b2543f335f4abe3b2a71dfa" },
                { "lv", "05445b00d54ea690c9ac6531762766483bbce5fa1fe69348145efbbd60ed902d85b161db6a1323e63695024b470d9e44cc87cb40cc1d0222e0179d337ae3f549" },
                { "mk", "0b0e63fe87054d416d1dfe301ccd574be87882e1f7b2daefb159dc9f9d1c3f379f4c938ee928f6dbac71d36e8eb55bdcd4da546f66c2fb6f672b09361a652fcd" },
                { "mr", "91b7837dcbf74b23701f98f59bb736973bc45c5bcf02cb1458d29c02ec286d1abd04a617ffd7ad81f5f5e144d6e55db651cd0f734b72af9f4e8bc92d72ad3a7f" },
                { "ms", "85f6d353e4d717db82eed4be91ea837dac16239429f224e3c7b69bc286daab94ef5702b12f5830f2d487816c58b14816960b6b2136fe45d54fea774b8159ad7f" },
                { "my", "2b38e465007582be28ad5363232760eac874350a7f926a698f04fc6557fb12e7794d8807387f451307ddb8df5e86445b4a20c48f218d616f45146c03643b7a34" },
                { "nb-NO", "61be96fc39b2ae8f12f83e3fc91f2aece1bb2e8cc79b5bae298d5661f47626f1ea9c6acf1938493af6e509ded03f8dea706b5a860cdcc7214fba987343127b4b" },
                { "ne-NP", "30935a88a962a355fbe668690b3c70d896c458884b2ebd2cab8c4e0ec86c746065b12f1ff9c2a54c87983e263e26fa34f70f82d8cda3c6a2638f750410fcd5ff" },
                { "nl", "e88a13ac7fbbecdd46c6c95b82d4d4d82d9799fd4298fc168869d43122090e9e28c1f887890b257792de829dc01d0868985f4f4625f5306917665f105fb4851c" },
                { "nn-NO", "8d11bdba36f5752839c3e8d0581e9c1447564b5dfc0fe0aea868e784f4f7560fd08cff74c7a3916bf2fb8c52ddab36a719eac44a8460eb47272ef6afc83c3eaa" },
                { "oc", "2316154e84ea64f690fb3bd1328968f148a70f3f161a487a7b6594327bd7285a9712abfab5bb6597fa19c82dc7e3cf7b4762d6a052a548751de09b1ad44771dc" },
                { "pa-IN", "daf5b778f2bbc735feb4fd354c7587758a32d9f050a3b3d7f13a891486cade59cd07d8cf09dc300102b29f1d88aca6a66bcbc80143be928fc11dd8b683ecfb71" },
                { "pl", "78921f005bd22bc904bb672db4ebd3b16e579c9361beac3e12fcdbf1dec91ffa25a04ecff639277e21627fd8b582a964413d30ba94fafadbdad51d012fbb8311" },
                { "pt-BR", "67ce3c8623e9ca17fcd93a1951bfe05a4e7015d3f4e0febbf821bdff19464f78a724930504e175696dbd50d7d05be31a9312a182b9edd8bdade18d596d1a1e3b" },
                { "pt-PT", "73f6f5be51b6cacec1938b2635a6239f409eb376d101e2328c6b3cbe9fe19fc375c3e2af654f8889428d81d08c076b90b2a28c191505e531b507b33927975514" },
                { "rm", "96e305f475ea9d6e5b37f85bd1605ecd5eb6a14f194aa9a246befd268fbf5b7cc9fb618f47e01803734dc042c5ec6aadf357342d5f486cee520c1c70328ea9e6" },
                { "ro", "91c1860c43f44a9c49ff08073127d4b72c9ed172b7543d36268a4860a4833050d4a9c0fcde6a3db9087a54571585a3d47eb17714d4f9a6cb36fc4051412e8915" },
                { "ru", "d668137bd9bde380657bec0ab48d25d91b59189ba7d1d0dfa059205d9e8448e043a808fa08664924ea5d010cb60ba311eaf827d752481e6929bc5bdb41cc5e58" },
                { "sat", "3c489fedf68cfac42cc2d934b1355c9d70311dc42e25e6e7cd558a6cf74b155d3bf17af0b43139b1fa54805a24a50170194b079494a621515683844d17b78e08" },
                { "sc", "552962a95559d710a1d91d26aca995ec36548c1d1877183067ac65fb68f9f7290e8445c33b54f5b9f25d6a7b8c9c89be754fb6e91625895451d572ccef34de46" },
                { "sco", "c095a2f8288fae93cc6771ebbc88a8834f099775e2e385655d1520d26a024afd168b1594fa7236e37ee9c1a100c0acca6b191ed95df2f6220f74403804c7e36f" },
                { "si", "99b82321f9f8d3901f9573c6f3ade4ad623616646c29a177503db96ceb5c17b6d75fc27c912eca32b1c2f76ac7ef521b18e0a65511e13782d21cf586c05afde0" },
                { "sk", "6626ba2ea601e52d84a17a92366c0742bdd9c003d4ba2c2145395abeadc4dbda00a33779d1c4269aa23dfb072c36ebbb2e3f3fa3dc67c68edd3d08b22e851412" },
                { "sl", "ff8472283597201881fd2525032156177ac0a261293537e78a710a9f567b4987954ce37f301a7aaa3e12589a16356b5a4f4be45c491023fae5766854704f10a3" },
                { "son", "7dafb78e86dc6b7dd5a8f7821d41b3ff6225d1ccf2839f5faa4b66b6f2acb42c02409f4d23277ab16bb4fd88c390e40086a59abeaed01f2ee411045aa63a5b73" },
                { "sq", "5ff1756669a8c717da53d57c19ffa8cfbf445ea06fcfd34ac46ccfcd96a6ed55b1ffda65da134f405ed3ebb8db195f7f5a8a3dacc33249ec8ebe7093e8b5d9a2" },
                { "sr", "58e90ddfe177cf03cf75e90080ebba987a35e1365807c4aef69cbd855c7085649e9058bda0567120c6c8b861b8d97c694aab1e37ed37106afff77367954aca57" },
                { "sv-SE", "6c99cc5d9b951c89e7d47b5adcbc91b4b13316c9068f58e95c1b9b030e9484e1489da7689288352d25fa50115c6797f5aef7b078bca741f544f56ae18d76e057" },
                { "szl", "fca16fadec3e709aa74bbb8fd62dd2d704f4f1cecf182adfb19bf2d827ea1ad5d8722b6b53eb7062a7d9d6507c36901cd98206551aff553685f28485f98af7e5" },
                { "ta", "ec9d054a70ce705055b82a3a9ffc76744d05369b01ce23adbca9a925a906d4178bfa0d8559cc38380ed6402236161387847181870f426d26df446831d0b6b379" },
                { "te", "d2226a3531eee759bc08a58f8218308ef91051453200813085069cb5f9e11f072f24e502f28b3a224bbe825a31da731e5e96f0aba3b7e10fa6f4dc47fd5a73a7" },
                { "tg", "55a284120735dab85bc19dc09abf82de2223d0a2a4baf77adc963a594ee2f974c2d0ee1bcb1958c14150cb8880efb73032ff7934c20ed8abc4275801616951b1" },
                { "th", "138e4cfe8680c7592d2f4ad2b2a125e9900caed01a46de41a62be89197a3c4a884060f99d0552a287e073faabf51a898b42ace42ebc10d277af1c139a4eb3186" },
                { "tl", "9aa7663c6733f21eebe72446158159ba2baff5bf330c8a6376b9233bb524bebdbcf12ab05f1def2e9a49df60f001929d24e9841f1359ea9d109a1d5cb468b94c" },
                { "tr", "c9cc4e1f646e660b388ea3385aed041818613bbfeb75504ee6c1c951b109797373faa9bfb8b1931da8315e9299bb0ad83d7ae1f05d880d379ba71a5381a2eb1f" },
                { "trs", "4385219f2b645348a4d1e4eac2fecd5653f27f7031c6301c2ec5e9423882400b13623b1c01f7ce4011b15d7aeefaf72d7870166dd8df07426c8f7863791bd02f" },
                { "uk", "b3c278d06db6a298f0f75dc5ec9094dcf877dfff55a91bd956508474de8591b3505f5db742b2adbd0caedab1d2b3a31430d2cc13715028484f5005be5a1def6a" },
                { "ur", "365434b752eb044857cc046bd493665c563a49707aeb7d7c17009c82806d97332c46edb6082a1ee676f772e4fde17c8f87667e3561bfa53bf0559e9914543406" },
                { "uz", "21209ef2f48d74e83c321ed177f51981befee021b619c47762fa9bb9708d3c7da1c448d078157f071a5c51fef4573b9f3cce822bd3dcababd860f608f7456945" },
                { "vi", "e9760dbd280900f7e8154fc7e20c85710e683d00916b23ffdeafa8a484530ada013692459266fb4eeb3a563d28f60d093236a73f2d98db15610d260ce842eee3" },
                { "xh", "678f5d5d4578fa481525f13e4092ded82f982a5b34344361399ea5f083a054ff40040f96258ffd21f6746ff3d24a5d84e0006ecb24b9ccbdf713f8fb3dfd02ca" },
                { "zh-CN", "0c5e1ccdf570d758f7678cb8cf7a843e233a83a98b42195ce02e0752c267b2a50826e7c902465969bd112be12f7e73b9a8b88927b89f8a72aad11e2dbbbc5007" },
                { "zh-TW", "c3e92f31cc16102a8b134b462228a3968553ff5f4de0e03608ecf72708513a644f956ffe6fa95557652af31b3f6277ba7ef71fc1b81bc51906cc3eb354b3d227" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/121.0b8/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "4163bc83eff2d0f4ffa6e31e2d8b361540e42f0db9a1f200931c7e88fd3951bd16b598ec3b9a829d937bcc3b967300e02f08145ddfc498c74306ba654a2eede2" },
                { "af", "771e70bf7c88d821c4034109e460933ae362be2b12971d3d85cbfea483c397e3b586e6e390d75e07ebbcd6e6c5a00c9192192e562b4909b94ce4cdcb09e3bf24" },
                { "an", "568a9f12138b343d918e5f53a049548c8c10125d41ff768e432a7fe4fb7dc368065c122782101665ae946a0047e2f9f2963c910f6109d73d0aeff50fb1bb271d" },
                { "ar", "aa4651d59966b444316bb588bc9bf2a320fb442227ec308ee59e6232c2428e8501b8cb05d480b4cfee1cbb9296565561172031f151afc458b0a19af4d3894d40" },
                { "ast", "60b188acf4ec5c1280d632785bc3108ba7dcc2a3d910d847823a580a48b35fd538313151667d044df9214e9cc106d291208a69e0666b79e719d25d48d70291be" },
                { "az", "93e4bcd5781fc2ec5479fbbde5b00f67bb24a4bfe7aa267cddadf65c3d9fbcefb90037064adc0de5d07074889cbe301d243f50e3d9d1b84aa7eda5c0f6734f5b" },
                { "be", "8c85fe69f0b9f845f55f35d9425c103f33b452db9f75e9901174a20771f0549dd0ce5867ad53057815e9d4ce08c160edb46b5ce359664256a0476c682956258e" },
                { "bg", "1bfe23dbcda0123461df91a47208d89c537d479b32533148d717310e65c76f0ebd200c75330ec57890858f3b553e8313a618cb73b506ed96776918e9172ea7f1" },
                { "bn", "f002c613bbd1ddfbde6af3926e336d5dfc09c2a29bd0d1491c9b6b104a59bdd1c37d2f9a8cbe9ea0deaaa58b7693c1620d1273d717ecbf40f87be192b244b926" },
                { "br", "d8aadf27beca87269642e0224cf24dc5b005d638149de81d07b1c43a5504c6c5b7e37b3dbb4bffb0853b8801a4d7c25d0bc55715563068f860fcd6ffc235dd4a" },
                { "bs", "dbd3d7e70d112edcd9911f7b50f6e5017f606900b62ce0cb7278aca93b0f96576839151367dc2e2438f8e8b03d657870847aa0de67b803f9227bf8dcebe5bb6a" },
                { "ca", "c1c34696215086b5ab15feb827fdbbc386c0f9cd16587ea7dbe846bb64a99c4def1b186eb806d5fef20b9c1d6397739bc98645035c22d99e4b9ad5045caff127" },
                { "cak", "a0b3c1c28c399159058006584b12e4589c040cf9073b2023be84370753adbcac2725615bc4cb422ea12531721e018b009ef9adf4924d827c05d8d94586c7f83d" },
                { "cs", "19e8d36e7a029fc75e3d328dedd12247a45495ac8c3f59bf313c289f8d999b513feece60d842001d7b2e82f860010f1bfb5f9eb0a3bab64cd0508a8c631bd808" },
                { "cy", "dd663ad28c9b0a7f69c3055e7bfd6c1dd22cb6d30a4d910f7519fff85d238005b07d22a5cc67037a8108ac3b3787bcbb339bbfe3c6d1ede2f16a9a2bd208dc72" },
                { "da", "4454364f4b0450912b988f8b11bd6827ebc77b5ca09595b01114ee29ed5b7fd27fb6b07622422464585859579adc7e439efbc1fe3dec0821c54453dafb175a49" },
                { "de", "452516869840e67a3e73deaf6e4d4b2fc13a98c19670bf43f71ef81d5a7d1e86320914eddd4f07c87dfd14181fae946578249ba8aa78ad5d91af10fb227c5e1e" },
                { "dsb", "2c61fc3209d2d5fa0aa4d8c08b0e7567a6508e9b40459a703ddc8e2e1de38695ec860fb2f6bd0a9dfdaf761f18f9329ef06d36c364c5ad78a4b65f9285aaee89" },
                { "el", "35502750e46643b14ca1671dcb26b61ffc083e90252c30464b2736f560cd078ff565b69033969b8533194bae286986551cd2c40f108ddc70054934ea50def763" },
                { "en-CA", "16c14b326114ae7638e4a2c3ae573002e317dc808af4818faafec4a9c0fb2e983ff8277d374dece8929a794342839cd4585d28ae76227df603c4065258d6e29e" },
                { "en-GB", "642aa73921a5a6a32527bd59ca70ee5fbf799a6ef73a872106e4cf14f9fc264daf190e507b38898f62676366d74555d17f1ff89dcb239c2926b0b17550c7323f" },
                { "en-US", "84f553fccf8215ba2d6e417f4bc245ec6436c9dda8b85cac0d530b48e8446fceedebafcbf4d20e1222d6a05b37e21a229037931b8d76975ff3d6ba8ab099ee72" },
                { "eo", "6650e8eaa3b236f9f9c96b539df1b9a2f4a67dee505b838ae95748f7c9815abe8153f38551738b32e14a940ad8ccc5907aa370bb1075eed4b2cf594039a369d1" },
                { "es-AR", "7057ae94af26c82b69878b4107f630ee492f19d59cfa78815327a9e9326c1c1d3c5f9ef9f26bc0356e435f9856431e4b1491b543d032389f6a74a69f2e2dcbdd" },
                { "es-CL", "e6d3e36d2226a0e63db8e015402cf1a12307fa0776de05910cac7fbb362a22a8d0fbd6b2e8eedbc66d90f011da0ff9215bff7bc1bf98136515891e5f94b2ca11" },
                { "es-ES", "94f2b794a59aa4eebe684eb4bba11b63986beb1516c0231ad134b9d0152cf8fac9e3f8373ccd7c08487d4b481fc9cac98f2e49570396274e717a21d98a5e27fb" },
                { "es-MX", "18634e9a6ecba3999956c98a7b75cc8c8bab31d82a375c9d8414e47c33444c5291b2dfabd237f5b2667bc77c8176977f6df1587623c78b3c143766eb97515903" },
                { "et", "561850e7a061b5ed43e604c6e5c3ed0b8d9accb793929915a3cdf03e84246a28a3efa290aa67c559b00cf072016f5d0f4d7c9732d191a6d1cfcd0df41c8e4d48" },
                { "eu", "27e1cc373fa2fcd1da36c39b596fee826f03bcb0af72c88fe89681bbfd39d2bab0f7b16fd9ebd7587e87b1b1b9d05693ec0273199462918861c09026c27f5b40" },
                { "fa", "b12e815d1748baf84d35dc675d9873cc4eca348b1f85ed8f2e3fe0d42352de0eff955c9c3db2f87deb1d183e350dbfa4cb4aba6a3e6beab4e1c9bac4c3e9921a" },
                { "ff", "ea26eca14ae1401796c518d549ed6f3f7ac73b8bae8a63e9fb374856920739b79ca2e08a480d9fccf25733da7935292ccad4c92960a8991cc41920b38cf8032a" },
                { "fi", "ac003ba482a8b9f173470471eb97bcd872a3b622f9e8e6e3b9ef1983f58bb706c56993944a4675c8fc21cc6f5dcd704dd7d4ae74a6c68b81a9b324673bb9853b" },
                { "fr", "b61f90c6d3c4908895b4b3a6e46350cc447f9a1158994f286527ff08c7df7f9d1c45119a3f76a192149908620978cb5764f548ee206abd3823b8a16f349a17e1" },
                { "fur", "529efdfada8f4024907a2ae0a8d25bb653340e5d7decd85880bcea786d434c0f0a5ba8d25ccda80fc92136de9f4a2f4439da511b6be23f219bdafcc2b680e246" },
                { "fy-NL", "1c5cf25cda0a295c9ca4ecc8539451b4396c35e90e42ea8ca6504cfa0697e62c4a8032489d0085b6ee9785825f601bcc4aa8b3cbc90f1de1072e93f13a06a4a7" },
                { "ga-IE", "e6332f710a46e8b464e369c1164d25d6d8c0c905123a6f625247311203f251b820aaa68c16bafe167c8714717e81a7c34bac0a0c5f82145297c88c4839c9aa7a" },
                { "gd", "8dadf7910c16356f674585f232d62fa90b669adc072cb807a8fb1cc8a69314abac947973fd532291270d845307db2ef98a2d837d29b9d7d89732e3229e7db7c8" },
                { "gl", "4c54262ae44d6b857ea5e5f571c515e80805f4266eac88f4e68a4530af2653622eb9eceb1dfadbafff8a0391beaa02ba7bdaab3bc290ccd8c730783a6d95fa66" },
                { "gn", "16cbb3c16af2bd3ad5c9c93db8455006f44c74ccd1640a7b12dc9f14cbad6033420082b7fcf08a03224c0e0df4f230a472ac94549f10dd74bc584e3fe240e47c" },
                { "gu-IN", "86031e1e0158abe1f98729fb655f85808ab946c4b271bb0e627ec20fb03c049327428a6522581e6ae2957e1a7370975b7c27b6a5e0659f9e39ab093059ec537b" },
                { "he", "26e602026ffe942934ab371f3dfb563b062c3dbe633fa331baa30f92efd0f3977010393741b13347c2fa20c78958b39240357749a2b707c199afddd814128243" },
                { "hi-IN", "2600e24bbf3b4823532410e5b12ba67a931bb3902ea8e80c92cd082ff2b63e6c92936fff0469cea937e183becd0d8c661fc50bbed95c99a4dc4db8474ffdcbc6" },
                { "hr", "313612577706ca96aeffb2a5887f13abf0c5cb0f6751199ec705b024c2d46a5f3909887cf02cdf989bf870a4561c0765a262f7d4a6b0cd3d93ecefd1bdd52517" },
                { "hsb", "fd9408b6d635cc5a06eed6c7acdf69c058213e2c2487c96a7c48da9b568a7a73f93704e92b753e1246267fcc33c7ff250b1e4cd735cac29c09660457b1bd2c0c" },
                { "hu", "0d56744e03553504d0c6bb9a69a70000e311f085cea60dd67108d93afe9bd864c7cf8c35758380ccfcd430743249af9551a48c0d770d805bc14cdcf715281704" },
                { "hy-AM", "24eb4b4b50cd81d3e4d1cf803212e54bf5a0610044e05adda64c30873a2798664da3457ca83c83767e7b68873a325817dadff58e2f7c73e308adde20e3a419e5" },
                { "ia", "7714ec2412870e92c7d97c5b2ad3d9cab5c52701c09c2660695f4c1d6f8e65b0b28b4411ae1c8ba8a90d44baad0d3580f229b33d9181dbeb7d760f5866f5333e" },
                { "id", "b2687dbe37316e915006021d12fdb86dec624f1a2bae8689f46639389c5ec0a1a75c9092795f722be22fcea15ed7f36b381bd9bc89cca23878d72f8f8cca1a3d" },
                { "is", "093548fe91d708dd9caf9186e253bf0993b5090054068a3f1b4cfde3a64dbf9059d2d6acc474a2371e41055f7ef66ab865ee4e9fab52de4ba2f3eb015da76040" },
                { "it", "19a5e86cc47ea4d98b106fa88f62e27c27f159e9ec0491a83b6a30005cf7152b1cbb4a7da106d5ea805ed9d55801c12b36248f88b7d525f2f13d19533d561a25" },
                { "ja", "150b239cb4370588719700f91c6326628c00099cadba89579659cfc0a08f81af91b1b6ab45c78e5ccdf47e4dc7f45b65f2b3186fe279a9eb11b24697911b333e" },
                { "ka", "3ec41e7732b79992c4a7ced706b8d83957318d3c09fe89ffcc3d7a54f70c2b9fd3e391840718865b8a77ac72bfaa84cafc53607e86df46a115c721744e85f7a5" },
                { "kab", "89f8cb75b8a2969f69af89ae2c64cbd196e626ada817324fb4c97e52675a0eedb0a48f2ab836abdd8d12cab9739f869a33468784b9f0c5ab93be78901bef2a50" },
                { "kk", "50e37b4acdd0b69f0bd1503d94dec76e34edf766de8770ea964da338208fb0c72550d2c93ae807a1dda2c355601ba87563177a321dc8f6b10206f11270b72524" },
                { "km", "2157bb35450b732fd05a0b28629225e80da6e6af58abe041b065c320962f5011fc65a1c8e4b05d5880c5d1c88b333cba5d57d064642f9788fbaca49f0bdceb53" },
                { "kn", "d95c9584260719dcf4246acfe84a6b789489965ce9dd033b0451b8a48306d32cf5f2b66cecf59319cdb4fab14ba902201d212bef5dbdf06ce58cd8aa4473539c" },
                { "ko", "9842c87891aa3a57c3d41b310f8f4e80b8203f5902cc5748302a5ce0b924bbb92049d945fde7c0a890e13f1a9688c28d3100b5e51be14a7b85465ef57465d945" },
                { "lij", "3d353f3ebc6e3a8fa3cfcfeec4c2b291ab82bfef6b32ba196c91bff2cd55ba0822f7292f5a602f5b97c4419ed137a395596302b3c4b03eeab50feedcfa378ff4" },
                { "lt", "b39cf75851ac49763826c4dc13b1939d5024a684684e7b9babd0bdb7a02e3f5a1d6400e4e43f9d5bdac2ed111f841a7e8103fd4610c5e83f03cb12bc0d538a85" },
                { "lv", "7544d1fa4be87efeafe6dcb899e8cf6d439ffc93be9a4216b0504d357b18472f116db17740d326df3e653d9d17c76652d0b649c636eb4851a14d89faf8155d49" },
                { "mk", "f82246d5d5b7abbe2924b38764312b58dd7a0742597c8501aacff0b42ad91768a8abad7a3dcdcfdcdd85dbccbbdb726f61743a1424e420cfa8db0f7cf4b87071" },
                { "mr", "2072fb2a68877aef6c9fa8890c0d79bba77320188542a4fe9ba06255861d47fc815a50928b38417aff00e9a79ca047920a8bc69636eebc628fd337d775fda860" },
                { "ms", "f26e12894605ecf9e76437bcc23724d1f75b56a557f65a8ee8de616e7fdbedb717a221897bb973b24f80ddad63f9a98fc2ba5aede77ec752f204bfe1a1ebc7e1" },
                { "my", "e2b1734c13c6b2968aa58ce3fca1b244cf6ff7f1aa5239923dd80b7892ed61965b890275292c6582f4416d2696e99066be0204c0210d5301b884e78a3bd6cf97" },
                { "nb-NO", "e8ae12f3c291503a11f1d39fe8d13644f222d974085a77c328bb52e9fcccd4443afe33de0805ce4dcf63f423e0bcd0559bb8e46bff62ae5017c4823daee42e86" },
                { "ne-NP", "1b6f5ea71b502a134294d12e69cbe8638fce5900ab18febde0b8f52576810dbb7bf3146f6a799e8b5782094277947c971c439dccebe67e1590b1fb1905f53406" },
                { "nl", "e1eb6fc6d75c867cb63719320acb35234b71ed6a0c235496d481e6b62ff2f02597daad05e4104be399dff2b75fc03d9f881eb981deaec5870e7c7f1012ae15ea" },
                { "nn-NO", "6a32d151e1e0e91b7a0d1786e05d2fded92d739c21dcb52a74ef93a689b2c469140e887dbc0ce5d4f252a3c03c4ed91b23b835b24a89db3a7b68938fedcf5c7a" },
                { "oc", "d935e3df1c37a27615ee998a8834039b6cdf31a6c75a102c32c8c346321513a60dd83d9faf185c6da0925337274751bb57b0454fdae0417ea7cd5895b91320e8" },
                { "pa-IN", "046c845afd01ab23793a706314876e04846282edab68e5c499816a2fb6c2be630ff7bfa7c4d6ab68e5cc839c9c48f9cc024881dcb559d7d36b347cc904f256cc" },
                { "pl", "a0e22e1e80c6044720978265a45f9c27d88e834363b8a3e4a7a3e3fb839f9fba761e2120dcd4b454b850b1e37095519ed0021852205cfb9368e7920bde474761" },
                { "pt-BR", "c51631bebcc84a883208fdf14dd716790af3973e421f17a89f786b80b918dfe2194a7ef28d0ca4d241076f18fa0ab5b34d5da7ca3fc638af36a5f8660ddb88d4" },
                { "pt-PT", "a3f40888da5179c9ad157c10c026783da781f54feb1a1631a3ac335d72183c9043f317ae0fa8822819fbd7b747c1a385b70391a108b4721b2d57046458d67194" },
                { "rm", "bba6c2cbac871c2d947c45d1a9d3e4e22d8258107ff8e76a4f0ce15620160e4a7626ed0d212b7611bdd0ccf513f60d602bf1e378d95ee7509c9890493ab38391" },
                { "ro", "c0b1c2670e9832c543b50ff89a01dedf332c5bbf60305cc149060b376f27264a043af226f2c27648156c4e999bd4cc779354e4766ca45149ae9bb9ded7e8514a" },
                { "ru", "ab8d700b40f7ba1f04554c80e37dff7958e33504333c4045e09a7ff6df141d32adc1afe1a8cdab2bbd82a03b5c523289dec3db32fdb1d78ca6bf941451b1c64b" },
                { "sat", "815f92b7581618a01470e56b2b1e1558b290ffad6e012e181a253b6ce2042f629192540af8d3ef7b294fb3e8bdbf2a11c29cfea6819b41841eb3eff8e74b16b7" },
                { "sc", "4004d4a43c9ed1bac24823395b699acd3e6c6ead365760f3a0fd15b682f886612444609d48520cdaa9f6b64cc0c07cf4dffb29f4e61f15670e3157440be7f023" },
                { "sco", "2ffb2749385528694738fcd0cbc3015fec17164adf713aa90b6d279d7e13583cb6020f63778bc2fd39b2d4c358df1a3d81a98669e4faf041c6710a219b28d884" },
                { "si", "ac7104fa2d78bcf27ccfd3976911f18d141d4941e93298a2381bb232234579d13a9567c90dc8cad206fda354397ce4cab50d80f746d18d4913eff7a08599193a" },
                { "sk", "4a65ea55e7e270cf2c5f40f0af88fbcc56ad3a11778342679733285bd36f3e21c660f0d8fb17a96bd7aa6376a567e2c547823919f210615ce4abf2da155a4dfa" },
                { "sl", "d9bae3830627aeb7e47adda5344ed5b87bb87ac1b3eaa5a688e7ca414402635f4fe73d8621352b50279b97130d2c11640929bcdb22ccf651b10e022e5fc112cd" },
                { "son", "5438cda9fd070863f909d26593a20315693343d3f799e72eb1ff3573456fce0e0fe8bd41485df81eb2b9043b211ad183b4b927813403f81bf301daad48db37c0" },
                { "sq", "53d67fcaa07f2514d7d84a2c3cc86046e1fb6ac300a0c300657c1afb756cdee7a49d6f7a0ad183a3cb39f279f20827bb8a07c0a6e9f97b6837da7ba40adc5f41" },
                { "sr", "4ed0b9887bd90314a0b6020322164a2da6473449e5dcec5bc6f7229b054ce4c6f459e9f243fc06d60bf3510c3758f08a670ae406f84cd09c3bc6160380d20dec" },
                { "sv-SE", "e4e5d369bf8f184b258ff66c603470187536e2e488c854f1c660eb4693c31874b60e5c91a69316fd9e318962ed725e2df30ec989a8f49a9c4ff83941bf0a9567" },
                { "szl", "17fb6593317b1fb59d7816335d26dff5cb9405121394cc80d46eda1549aaf602bc8ce399e6060fe3e4cfa6ab9ff81bda7ba31a0ee00b4d18bbd9157a2d72218b" },
                { "ta", "9fa93bb1fe50ce9dae4b700373f78ac2eb9748b43fa8c1b4aaa1d809ed8679b283606eee97eb3beb8d91dabd81e4e6ac9c83484ca98e7373b11edc50cf252bec" },
                { "te", "8249d4254e4e71d0b135720cf16c431c3035237658f17c7d98fea1a6d8bc1d3fc2136a128c8ff4a070b62e040dcd03c3a4023edc2f74028dfa1e3d7cd67bd34b" },
                { "tg", "04657eaa5c4f283f051d47832124679dcecadc49eb4672c207d462f07eb7f99aa6baf5d801d64889d7e0900b5adebd405a80e88e0a155b8322af6eef4b91280e" },
                { "th", "e77a0d466cdc29bde2ef98b679dba7ac9daac16bed7a5b3dfd4ed8abd874abd0a98d8b09d18e016104720c7d4ae596e6acd9d5bd7e5f7f012579d6eac82742b2" },
                { "tl", "4bda9a72be465b6d01ed585f5b89efea3c5a798c5bbffba8da2adc3fad6298a0f7e6513228198bf86cb74eca124a00c86e1fb8cd6028cdb949884b42232958cf" },
                { "tr", "d834bc029d2df42452ca412c0a4b03e2f668d7f0dce264dd5777b8d2a42f67c7f7c76fbc2cc36bfba92cb22fda1dbe0570d040a4cd67f2bee3c2adb9e7bd00b5" },
                { "trs", "6169de6296c57c4d3d745176e0778cc943efcfdc29973912f392536e0d7a29576955dc6ea898f349ac698800e3ce703bf51fd4622250ace6da2273ccb8c916e3" },
                { "uk", "cda917cfa7c9595c7b9a19491bb73d7683b1cc5912d3214957b39b63295272a72e016c722f23358a5c10928b54abf5c96b66542b92c9328d2a64ef1bf156b247" },
                { "ur", "e83bb899c17161691ee9b2c7762dccacdeb88b2288545a39d26454a4b073135f54416a2e5bdf4e43c488659f9908f0edc5bdc9229999ba674741ac6e156aea5c" },
                { "uz", "1f066374775bc17b82994d6aa924f906d78c9fea00b4423359ca61455a3474a88e3d89b812403f11a66122e1bbcd1109eeba9ba494a1571d080519425db37c3c" },
                { "vi", "38e882bff1cb5b4030187b8673e38b0a675d2c5325d9861c6f8193cfd40154dd2d92f33cc0d6acbb4ca08cac679577b10dc3b63178c213016b14860c4a72263d" },
                { "xh", "513f22abaaea45d192a6ef8b6f285d5bf7bb7cd8c7831a1d9a806aa5120aa8c7f048b19bde527b9fb02a76705ebc85da8241497f3f8276d9c393e74cf42d319e" },
                { "zh-CN", "28dad6cf2f673453fea4ac0ce922d8293864103d9f2d01518b15898c4e254e7941d65b857112c1be03ef8053564606fcc0a57ae31b352e80711fa7d87660cdd7" },
                { "zh-TW", "99f761b3ce2302f1ce674c6ccb3682e7ee7fa3d8c3f0ac78b3677d354cb863dfcb8b79895ad76e21eb84172de6e58dcb5c8dc8d2f450bc76c2c4669136f21add" }
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
