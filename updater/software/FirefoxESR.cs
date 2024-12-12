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
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using updater.data;
using updater.versions;

namespace updater.software
{
    /// <summary>
    /// Firefox Extended Support Release
    /// </summary>
    public class FirefoxESR : NoPreUpdateProcessSoftware
    {
        /// <summary>
        /// NLog.Logger for FirefoxESR class
        /// </summary>
        private static readonly NLog.Logger logger = NLog.LogManager.GetLogger(typeof(FirefoxESR).FullName);


        /// <summary>
        /// publisher name for signed executables of Firefox ESR
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=San Francisco, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2027, 6, 18, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// currently known newest version
        /// </summary>
        private const string knownVersion = "128.5.2";


        /// <summary>
        /// constructor with language code
        /// </summary>
        /// <param name="langCode">the language code for the Firefox ESR software,
        /// e.g. "de" for German, "en-GB" for British English, "fr" for French, etc.</param>
        /// <param name="autoGetNewer">whether to automatically get
        /// newer information about the software when calling the info() method</param>
        public FirefoxESR(string langCode, bool autoGetNewer)
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
            if (!d32.TryGetValue(languageCode, out checksum32Bit) || !d64.TryGetValue(languageCode, out checksum64Bit))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32-bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/128.5.2esr/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "28d33857589a5804c5c4b58eecbf1395599f7036af072eca82d44e60223abc421e6ac4784fd1f86ee6118f0c76c1d83be7a9ec064381e833c4600b25e24a4cae" },
                { "af", "d7bd5d88666496174d23e7c32d3cd9db8c97dac67255e6c7cb0cb7b1435731d10358a9abffb0a57822dbaa48f9816ce9643ebe6fb4e2e5f17a1e01ae9c241ba5" },
                { "an", "69ed3092a36391e9ed36858dbf825c468e1301fd164fe3b3b6052b44ec3839310affec096f70ccadfe7d799442b359d97cd93fa55744288d69e65153f7f7f12d" },
                { "ar", "b325e6ed4c2e41bf0987dcb424301785180108b36e12fbd45913751d7366cae4b7babfbeedc68b1eab4c54f6b363e27958b221760eb6b8636ab36935f4e26a2d" },
                { "ast", "dce72cf53e720973352a6407ee8f7185475667347d01968c50527767e160499c7c470c97c8618b604c84454326039c1bd38feb51c7599345068c25d827b79d0d" },
                { "az", "4bca5534179834e678092a6d317c55da8f309263223638b3118d781128472bb4276206b72132d826f343471588fe0378a1009dde8bd8de33fa467908fefda559" },
                { "be", "7f99b54e3dd23055d5b22b237e6fd92adc320d0e17d758e293719d3a09eaf6beb8a98528d9fe3c84918da5e270ae45688e1a91fb23caccac405f0b522b09dd6f" },
                { "bg", "ab828e71423f2d679acddadaddd9b456b13caa6a467d08347c224158f72d230e17562f53fba290eed47c2f4775c9d560279ddc4e27e20bf43f017ece7f72bd9e" },
                { "bn", "8fc5302967867f7f8bd7effae1da4bacf2e5e16219ac85d58e88a390f9079a1238438d9567c4bca4af6c6f90cd800aa5c47e9e46b2b223b6db3266f3ce084ce3" },
                { "br", "99041ba189bd8d88b7de77bf88336169f75929a458bfabb12b2e09692f80f051a6ce3874d1c03cfc25197e0b0f36498fb6ed316f59356502a1c77d69e6a755a3" },
                { "bs", "6caf01709a1247833fdae63e5959dc2bf69e6b506d6dcf4d18a908a66708cdaf71aca8ec516e3197226f372cd97a99e3e0d71612a45ca9d59d164d0c885dd23f" },
                { "ca", "bcbf279cc0fd659c0e320b167c2e664f14f2a63a772ab3fe63303ccaec995616e0dc7a09d50c2834fa5e041a9d5d386e223d7c5d1944cf9cb7a39131d538ca40" },
                { "cak", "35d374265427b71ad368c6ecf0a6f337095ff671240ed4dc98eccd122e34d36d92aa3788cb942ddf6d0b021d43048916dc86fb118ca6d429cf9f9277e391b55d" },
                { "cs", "03553bc98c1148fa298e282c275302846ad5689d367e83805ed24649738c09cad236b8c814267296eca9d2a99464dae1f7f1eecdeb31fd7ca932890d65700a6e" },
                { "cy", "a90c437ab0555e773543c192be0f367a5653b53c86014d49966d7b2a744c30b4993c85e08425a063c801d7aad401fb1ce3ad8229bcb71c98b471e7f5c5db1c92" },
                { "da", "eca2b48f26b30c2e875ffa689a8230392d2bf441301b739d65d246c525d1ff6b74291abcc9b602f7dadbcfbaf98baaa508cba30b7201fd270e77f95ae7cd5a3f" },
                { "de", "67e2bd482d295b18ebffeb14de41af1b8c801ee91fb2ea944733be0f58bee077d8a1fade14421127b71802d2f7f535bcb3b0bcdccc730f4be44a1e04af9010a9" },
                { "dsb", "14e52957b5753cd49dd24305b29102fea172efdeab7134e89cdff441f77175dd34e442dddd6824bb8ee499dc473fa3bf11d5b567203e257eb83c9abd7e262beb" },
                { "el", "c8f3cc7eabb634399fa18eae4cfce74adee8abcbc1da53aa1c258464e33be89eaedc5b6c76f761886e16059d702e4a7590024116bb1f2b172207e09f047eabcf" },
                { "en-CA", "093e535bdc98a745fe28437d824fc9d0b51f8d7dc8b8711e363b63bdd0b6fc1f9743118a0ad334fb1503d751816b35341fe9ba5a92a496f00878d6157050de82" },
                { "en-GB", "797df0651306068d027691b2a53843dbbf22cf489086b72d6196320e2d82861929e174475579845166ffa0beda1207e5054ffe2099b0bdc13d994c0e2b4a004b" },
                { "en-US", "a16601e6929956a4977be33b0a0660cad9bcd477bb23dea59bd2ec85185877b2673676f69c6a7ab1925a13ee8f77545c5bd28e16fae6b270b7ac38fa5ee83e02" },
                { "eo", "0d634e940cd84af9ce4cf5eaefcd3978d637e7203598cb9874df0f762db3b021666f5f9c97362af5b0aa939177dda46f734717f000fc40e8800b80b38021eb1d" },
                { "es-AR", "2c1f5ec5bf77adc15ae65c1a521167486bab8eb99ac9df4707b168f1d21892b149e1d33c83b10df97130bb0a8b993f008ed392a1634450202c286e70af7805a0" },
                { "es-CL", "d0280734211b01395cbd5fa13cd5b670eb66e3808b3e261aa20d34d79ff954d81d6340b4bbf038ef59314a1d985a594acc35b68508898dd18eb37a8457559c64" },
                { "es-ES", "7f46e6011c3c6fed9c3f6d0c05ec38b8e2ce9809b3696200c8d648dfa08c55ba52160d551079f2a8cfccbe2dd57078add1690720ab55404deb1578774e363b91" },
                { "es-MX", "f091c450de41d57d391873769bfc9c05fc936965107c203af4ae3cafdea78070b503c8fdef04280b6bdeba2683a342a5b2770d212857cec4d1f6fc592e8d627d" },
                { "et", "6b43193ebc54ea64954881244b15071391d0fba4df04c9ed478d35dcff9ae8cdd979941d6c729db992e25b7001701b4581bb8bde77eb3c353dbf4e8150e51a80" },
                { "eu", "6955927c3f57bd2a99b705dc0d50ef2638cbca854cf9cf2c11af557939ae932cfc7f5ed888ec31b24d3215b8456d563af49a2cc430c657c58d4ec42ddf371fdb" },
                { "fa", "9d250fccd6af3ab132d4b4e39fc4684803d0bbce93419a8207bdfdb07c41c6502710a024ed3e23674dc39e6e8f377de341ab2d66256bfff839c128923afe1015" },
                { "ff", "cd83c8e9800ad3f633cb016d5656d377b8f663f3718f6bc704a9be1c6458f8c8186cd4c4b6cb7b03dfd41a40c129792e2207c3971b5407675db7e27708948520" },
                { "fi", "c6d8ebdb7b6c34564c0510fbdbca6863dda41e87222d95d01b7dd379d3b63420e03cda42caff0f90e3ce6c43fa83b2ba5df5a0c52a64b52ad3a1cd9c451735bc" },
                { "fr", "455e46503c221265751179fb2b41c983eccf03d4ddac951079df165c0e429412c507ae5017182c444448ea5be70a6fd114191595cab78fc591f667c1d6693318" },
                { "fur", "3147cf87ff90b9cc138ae1287bcfbbe1d920eda70a3b914fb4c8aaf98265d514d8806d012eedf2f3bbbe6649b240d1faaf3f9085b57e5a8ddeedd4caabb99dc7" },
                { "fy-NL", "d528fba08fc3ee95002332fbfd8c2d5681bd980d018ab42ae5666ad6ab1e30696d9eb7c7a19c17820d3b0f253fe620bd813b2a649316c448dbd8cb8e7e6c84e8" },
                { "ga-IE", "21e0dab7da5496c7459473935a05c96e55abf4ea28768a9e1c6b4a7058229f341b29107cd0d48b0b321860266e8185001c49eedbf96c338760e3930899ce67e6" },
                { "gd", "6424e9c454dc64b2cddc8ff35ee6f40aeb88b5c725ebf634b8526ccfaf0b1b635fedbfc53acfdb03bbffd42d50ebd9a106e93a3ec58ebd643614b5c79012c854" },
                { "gl", "aa001adf3be6bc21b15406d5bdf98abc336d855b586e6fbf3369a65b63700475e494031bfecd8c73aeaff6efb6805460f8fdc4458e246385bc71305cc1a6f94a" },
                { "gn", "fa8f8a65636b77963b03272c74ee432a094f2bd5a1551fc49954c75d870a009649692f04056a2b1bb50e9829435cfc9f88d401f81a9436ccbceb15e1c11ab132" },
                { "gu-IN", "008d5a4aaaf98b8ddcd561d91a7116a9282cfa248942f0d2fb3a9452a3f3bd95b8c8ad0feed37887c37e6129cbbb4ea81b730fca60423cf57ff8fd1bc42da060" },
                { "he", "7974ddc5f0f216db7ec3c6c8e93b7877a8bac1396d72d8528fbcb3adafd50514a5248cf2156212f0898f488bf7489f381abfe8e38b721844dd67f45c71d2d741" },
                { "hi-IN", "23b765e99c7ce57e133250019fbfdbc892f8a51c02d1cb9e8addb91d140a70cb760fb27b3282a2a3c9dda2205fe36bd81996069a63978559a8cab0070b0fc38e" },
                { "hr", "ee1d964524657f222c6ee2c0cf25aaf383d7e15ba45db7a866a10173f3c95072f3d20999c03aaf764da2eb2566b827e428a2fa8041685ac106ae6e0d2c1bd75f" },
                { "hsb", "9849f951fecc34f619aeee0854801f76c2e9b9bde603b9a6941260a56ce03e41009877ff457ddbc87e1625c4bf4370b86a44c2164bb4ae147fc85d0bfb6216e0" },
                { "hu", "d43e0f916f759c3798746a23abe314deecbc79dfa692feff9d2df9bdd4dcebaa7db6079a0d57eb34c49a4cfc32124dd52a3415701166c90a23e281c51be8a507" },
                { "hy-AM", "7d345807cae072560e9ebd6194330d8880673343c9d069ae4393c7d6c5aefd5e841b10248abf700575cd73d3cb8da81958b9d17d5aafddea54c98bb1d30e7599" },
                { "ia", "1a7eef5715632a65f2f7339588288fddf7291e8c6aa275b90f58d204b804b91e9ffecae1dd7705c5b020967c79b15c61620a8a99bb8b502df67db47e04f1d3e2" },
                { "id", "63a281c887e6bf6edd93dfeb4ee70df6f1e42be789e8128039d46303f4e61be3ff47c303fe72d253f666ae8a7965083c7dbd623d0341c886f4493620b64f9f44" },
                { "is", "8c2d625cd677b12e6bb3ec62faa07432768b4931f896b96fdb52e6189bd8ca31d96f333fceceed68bb927610bf89b92032cfa3eb755a55c20b6c0d2119dbf8cb" },
                { "it", "37ec055883618324d9253732ba0d615e5f3bcdc3fb1fa650cc23fd31e5100560f6a886f4c351fa6b900f336b0e6b1fc80144ff521c9d8dd6a80ea299d90ec4dd" },
                { "ja", "5c65bf29abe35cb6451ace58ce47013dab282e82e7a203087fd67b694d252aafa768fdf345ce71ffec9d16780780345085029cb6313a55fa0c44bbee7ebd7e79" },
                { "ka", "b3713759f44ee45b31644b42c9f3629bfa08c107d95f4ed518cb35feef80cc4de4397c70c86a262293f9f3759e184f9556ec5940ec14765afe5d6669d6e1546b" },
                { "kab", "2372d357da529f9f5334161ed70d65c649306a9f7e819593998410da03a6f39d94d580dce4504e62b23298d6e3a87e4fb9ff0144f1e7cbcae6001b3f0c7fa6d6" },
                { "kk", "8ce31f910d45df19674788e5d8d321dd3c06ba2cee951d067ccbf9be3a61dc4fe4b929693262b4827f95780141cb482c37bcf0cc46280cb4d23c185a81ad0f24" },
                { "km", "232c0ddb8cc55a37491d7adc14f69b8affe52eea002f73cad3eb84f3483a30eb8597e6471b6ae2c19843817f3c2780606a0ff17ab788091a01689f2ec685a2b1" },
                { "kn", "f88959c0c340c35b415715966c34e1dc539a79552d55303e9384b86f7ee0eb43654669828a39ee0392c3c2b40fcea8ff1842661cb1730f916ec28ba06f923b60" },
                { "ko", "32adece01cf53e27a4a34f426a3cd046aae6d432d196a3e3dd134470226b7b98d99cc6cb73110119bf604ac97a0e6cbe561c12f6eed10299353d163848e3a317" },
                { "lij", "b800002de604ac7080b047d8e452b172029717da55270258f81164f4243e2728ce457703c5cc8cfa7f57a225d873625d6d7f74f070f0286af0812369ac42a99d" },
                { "lt", "6ba5abf1ae30b4228ff45c5dfa4d9d3f824462cfde64aef16ca7432b78996eb3e171596f017dbb1fd37d075b8d936edaa0118eb91a9cba047899876c3b854f5f" },
                { "lv", "989116b97bd4ba78e423f6459474c9f3f0620f5c0dff6f21a0cd8e8e7cb2b153857388472a66862f094ca9c0965f37fb2c11ce51814ee7e0098a390950a3bdaa" },
                { "mk", "23e5d9d8d1786df17d64265726d65c4b19430e5751781475d0e49d9a6e69e01d5b2d1aec135002e51117d1fa06181affb6e7156a66eaf5225b16787495f0018e" },
                { "mr", "1920d8dd70ec809de0895ff84722667d1269b329013694bed473f34edc2e4da57d52609be58f9818dc79b521f788e4d3ce80707b288605651d5847befd951a21" },
                { "ms", "43258d6f3bf00cf6d9070380af2e954c880d09131e325e41818459cb4b0d491b0014c4221ca3b6b45cec8dc88228d6bea226256fa24da2049ebad1f5dea55a5a" },
                { "my", "2f01e14065e1d476d743eb6d0febafeec999d76b3d67a753d95ba3e66ed6201acb5483ccadf9fa1f53e331f7591631d0fb06cd956890b85e811a07c9653d8ab2" },
                { "nb-NO", "fb87e11d9ba69a0434e4034da8fd6f7d6480a70d522580a2b424a7bbcfa7228db6b0be44235fcb657560fbd3d6320bb1588788dba085bc4771b054892b805df5" },
                { "ne-NP", "64c05eaf0c9af6d2f57e5157bedb4ffd2fd8e2fdb1843fab3ce5507b5109c5efd1fa37ee09654920cafdf47153e4feaaae62bbd3c4b229311b490d6b5652b1eb" },
                { "nl", "4bca6a0650688437c7b8e667819f13184563b2389e2dda65c7d78b774324cbf096602ee22fe3e0b936cd30a313e1da7bc8a3e5555b609778746951bc35713e24" },
                { "nn-NO", "f0c26d546a19b5c52c4dbbe5606d38a2abd710e2d89c90d50f1f51f88d9f725118e996768d0cdf791329438ad0d44cf8d10c3f1735d31777f37c7226e942e928" },
                { "oc", "c32479a4276dc64edc528da58a09d7fcc8fb8124a8577a22af5c7a7069e8e3e1a53dd129c48a765d5f59673947fbc215278b4a917cdfeac09f4a230978a66194" },
                { "pa-IN", "07ccfa81ba7cacf3fc114a7bce69019aa9353a793707b38ee5dc2dc5e946db4ce4a566c27d2548346351531dc5af7edce88894ad73086b00bb678a796aae27c4" },
                { "pl", "8ebfe84204c678d6abfe8c780dbd3112aba38680548b9f562e5b241cd183345350497a07cf767f0628f611a711bd40683af319f3568d8e706ff18f6e59c05cc5" },
                { "pt-BR", "4e5ce2fd019402145c1a8a3d565cc456d3b634b00d56499cdedce4fe7f338ccafee0bdfbc8717487dc56f4deac699480b26abeb0f311dd70f94db2a96af2a5cd" },
                { "pt-PT", "cd45b7fa0416b076aa9f72d4a2160624dddb2bf736644cce99e35a2f5e232a75ee0d2b447108b77086c15544b2b4595cae39cfe6b82c1cb0692236f8bc3f03d6" },
                { "rm", "140b6f8ab51bc789b54cf4e9cd53af5be657af9858b13953b27075a2e3a8d14446f51cb71118f6ed8724573eee0836ff350f19b0b35819dc7caabb8dbba14f0d" },
                { "ro", "4755c17eb3aa96d5e03de1b599b5a43878a4e1d643077e99c95033e2ef4b58b2f4ebeccb7b8db7ed9d87440d7be258f08734d97f5fdf126b723ed16b8a874bcf" },
                { "ru", "169a74ce2adade1ad3358cc82e1aaf8b89a8894b00ad60566379a024a409343c69286b6e2a131b95862a5009fd6ee65153c698aaaf4205b4b50cec0829c7fd19" },
                { "sat", "ed15d8887c7efa78b30fc7d0351125d5cd188fdba647ca152aa0ae8846e995d5c4bc6cf982c854a3296e919550841b96916315a188ea91cb4e2ed9d2f1a64a5c" },
                { "sc", "41739308c35c058f7ccefa83a720bd7ca21cf090e40913c823403e527ce0ad238bc1343670ee7d19979fcd3f91e654f8f0e65f5039a0f4b24a20556fa7e861fc" },
                { "sco", "9f361076f2f1d5d50bb4becb4c3c2294b40b8b81b63a4f68a654b9ee3a0677c682a7ad42810814607a6530c9e63d24081ed096364b38921d04dbd9e42232add9" },
                { "si", "ecaa0028c3c02df27f366cc46a6f0b2f2eb416b076a7666c3fa8ee32d30a3afe24e4e0a4c68e650e0cb4f41c565eaf3bd306544df5bba8bbef9741eea2d4944c" },
                { "sk", "c0180a277c1b33807fc2a066c3e0a1702ed6657630d7147cc6eb2c0f25c503f6760abef390e8f704fc3bfac138b78ff43c72e0bed098c0077d5fa59dd2ab347f" },
                { "skr", "ad249d1983933b1757ed37c7a1c49a529ccde98c87a32541978051f8e01600a747e6ead9f6b38a4584769136e1c473b986331bd388341e45042331bc848cdcf8" },
                { "sl", "dcd9f3a8d06a5fc7aba43145198604bcb972a26130301324b1e44e2f95e7418a0d4687d24931d5b941a3345cf0b6a3ff311546f55765f34b2ebd9cf1395fb24d" },
                { "son", "0ecf78393695a824f128369f33e83abd087e79aa855c3898057eb16eb9b872194b25d29f40b451a88a283b3502f2761caf91e63248cb5ee4e077fb7fc7a57628" },
                { "sq", "eb18a25b698a3bb906cc3b763cd6b4c668ba783f619592185d5ceb2be095a965dfebcb2dd58250b02b061807b1a72b633e1fee5aec9c1b076775ab30f2e63b06" },
                { "sr", "4bec50caa0e1fecb2e31ed9ed7c16ef6c009d1ee6eea033a0352da98209524b879d04b70c54aa38d1d149f04ca578dd2d481cc47c4ee41691e988026cb93bbb1" },
                { "sv-SE", "8fb6c2176ce55954193fb65485ecf9b11390c4d62b0dd6c1261f731c6a530b04f72bbead34b9c43d840bab631344a68f883bf64abb1ec0c51fe6e6f3cd980f63" },
                { "szl", "c60e8fa228fdf74d97e1d68c9eb8bf72ee2c8cd22e5a1bd328947726c5b175fcdb9923abcc1d255354e163a7d56efbb7ff6fdf01e51a39d428f5316cef1fb8ca" },
                { "ta", "cba366e53150c1a88e88adab25f3730443a3da1f9a064edce6be3db62e69a9199684fc8d0eea11fc14954abb41295ea57e0ee0b953c99aee1d587187b347fded" },
                { "te", "5bff72993a592805d40a0a05b205d9a216dcc62a45e3f5a2667cf450d4f478112002df849920d0884b298be5c94f9722e23512bec8b5143f51b4053d9a6c2521" },
                { "tg", "4b48f6bbb710bbd460d7bb658986b60120b7b5314ce32f00e9d2a5ef28d9ebb4df3b2653506d5391b9f2bd3d07f99c40352ba37863b599ef4ef28ca9ef52a73a" },
                { "th", "b6be301b566513dc4bfaaea90d9d96cf11bade27f203ee5f245c02131360d752e2b5c6dc0940ce35e0c8b3b679818140747008ef7eca2c49fdbadfcb628dd48e" },
                { "tl", "ed33c02fb6e42dee55b323f5ab6b4bd4f1ab8cd0652aac06f4e64eee3fd758168e751b2daac522d27f7aafa561d05b3a7ad398e56647cedf44dedcbee2c1185e" },
                { "tr", "b5f9a2c1d39b971da9ff4995bc5c0fb2449ac6cc06c7c04e0a4d3e1f6edb9beb5a70ff7236d165d27667f1058cc2512972e2e6117b3f5ad6408f35ab46187380" },
                { "trs", "b8aa6d4a26e9368ddb3a8a573c9e7f07c1e3751ba02d0330fc3ba930add4cb8fd0a053fbbf83cdc56812adf9435a4fdaaa00fb14aa84d27639f384c3246f3442" },
                { "uk", "f606b52a41b671a17f913d03754d534f89107967b8a3ac1f6698870856b0de3765130b1cc6fbf9c09e117e8fe17d70d298351b9f0748f79fbf75d2af6ee54014" },
                { "ur", "3fc7c0256d29567090db91e7eaa9e3c4eda0ee664c82ce5478de499819aa19e447b0b3d265db546308c4f3455b3438dadd9cfcbc030462cbf06dabd5026e6d8a" },
                { "uz", "928c87c9c9e163bcbe4cbbfd6754c306e7568fcdc800526e78c6a1eeb923b347c80ccdabe8df4c351de3928f14bbde53a904b245857f1979f314d3b97f98aa4d" },
                { "vi", "1d9540140dc4963312ad12a6cb71d2f31be685ea7bb388859dab4db927a844f2f999845a7b7402ecda3cb5a5f5ac0bb70e03e4f26243a71131303d2e2c80b06e" },
                { "xh", "a908c211668e51cce9b9aa5125fa7ae224225090cd186a95a8c88698afc2a2830071fd058c199cd70031a02d4c01d0f128c7aeac1cdb23a0418d3aacb72d6d5a" },
                { "zh-CN", "f99a9ee2a5d093eb3da6b71507d74c0a08a74c8b1dfd8c3ca74f77a362afa66100a1ce7ff3c4be90f8684a64d50d1b4f8b48c49587595e0825b711863c7beaf5" },
                { "zh-TW", "f4a40351b85b978a357673bc739f090184a70dff22490047d371583545faf31c19139c409dcab97c4fba4a891c9b9b4fa5ebad7f1a14a0dce1c32e5e945e4e1f" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64-bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/128.5.2esr/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "3c360892c08176b92d2d60edb4061bb08f02712f6be51d230f620813315e6014b3204e10f212e3736dab646e34b5611fe072dabd260fd95581424651b7cbb571" },
                { "af", "ea7c93154cb10a1ca53c6d2a1b0e2ae565ce81581db07cc019723420a7f253dfbeb1ba7b7a70858fab677e189dc49e6ce031d5bcaa348d84c4ec1c3b6c91f375" },
                { "an", "29ec5619a52a0e97d1a5128e042173af9bef4c65a71e403d53994d0fc7f6672b29d3fffd88b2b001e1fe16a1c8c88018597e3349c769a465c366db45aec2a865" },
                { "ar", "f0b428e1f2dcabba0ca851a8c773f7aca4fc02e89a9ef5c2831b0395d901a758bb935110c65fdda7b2b56281d31abe306e705ed3c25da2a8cee2c40c87b88448" },
                { "ast", "62aac3c168b1a24d421bc760719466725dd2b2fc7417a062882d005d11a2f4e21da6f966e2b4f84d298d0efd728e7f6496ef4ce4464bbeb6c00537657bdbc4ad" },
                { "az", "3658b58e1bc116442f3a72030c633333b1b88f8fc473b4c7b8b31f6157485f1a9881003cf6b5c0ba62e88d1aaa07a9979f18e3305fb280d965c1a37fa4f63196" },
                { "be", "b1a9e272b86c9d52dc7567b96051c1533bc4e09a93f19cece1cf87132ecf7511614e999e51faeaa006968aea32896c70f4ab97634c76a6a9a0c6a5eff89a4349" },
                { "bg", "4c645652d1c0478f7768f80238bec656d76c2c303eda33d1fe07f8fa4e571da27b395c05a9bd5d835d13a6020495f793279a170d9424478f0e6fab0218801fee" },
                { "bn", "340da107ccf9b47a0adadb7fde0f5cf9b09d0e4de92e80bd7692f39c870b92897324504582ba796c137362b07abd25d6b0085e24b6ad2ef6ffc4291d4230b92a" },
                { "br", "0bba6859740a3a14d0ee1e5af935af833860baa3c54a26483bddc89334d8e4e280c94201f80aaef500fd7383d5636532b446fdb0a0e10f174c498c4ee72be668" },
                { "bs", "460415be1f6264dee4ec8192f2541e929e891e05de702c8f4be3feb687915d2cc8075b8d8d5ae017115fe074ea5b0609015b0ea3d64116525ab0ec512187aa8d" },
                { "ca", "16e2ce5600d8866e49dec3921ac8531d244f5d7b19afb8d706784a8432ba87faadb5bdf4b66787bda15b03c4412b2d764b5107578bd10e44cefda343a89dec15" },
                { "cak", "7197c9c5e7a077db93246e17645493eb30e864e0d07e669f21889eadcc8df06c301a87ba90f0684e3d9567fad7355a5088689a55ed1e853f7df7321331d413d9" },
                { "cs", "94fd50045422eeb7937ee81320c9b06e1005c75cbb53c1b7ab69b12d6ab22270fade2dd0c8763fe06589ec73c450a8a709bfcea388609832015c836963fbc4eb" },
                { "cy", "f9941aa1c89a237be618fed069b8f448b2356c120d8bceb1e20dc9291509425369edd227bdb27e62278f67076a62b00ce5c15e23ed441db2bdfe29be5c7dedc1" },
                { "da", "f79231f9d39c60071cde73e3161be9d8463508ba7e399867dbf561eb4892ca45a122069c3082f7baa9c9ba2268e85474d51af7569f44ffde6be74ce3edc49e9d" },
                { "de", "2f7a5bc26e9a53f2e69aa2386f52f10f90cd950a67015a0487d980f36fc4de6b2b0b604679ca9b29f4ed90c1fbe48f38ee1112edbd5b1a874198cbfe423e77b6" },
                { "dsb", "2a02a653e221e407e2535d629a3d6370c8d0951c29d94c0809e09cee1f1364d6dfce018b3cab04fe0bd93b9a4231545aeac5f55cf36c45bf8fea24f2d6ee583b" },
                { "el", "1d70876ae71e1860a60594c2f40b31bd17d3943e03a70af4e437e3ba933df9bdb222176eb3a1a30f954457c180abae996d250375fb02379176642df08bd9b8e1" },
                { "en-CA", "30d7b6d717da709c9ff044c2400129625767fc927f51c6377c67eeb267f6a3bf87a87532def622bc10c273d20dec7a50547ed0475266238b794e72ae8bc6a62a" },
                { "en-GB", "6b47d27e8067d16e5cb19541c9e6f117f4422965fa1d72ad7b9c9c38db46a9185a7e0ad37c309da7a5d78cd1841b0cdf0cd47132df6ec1b4c71d651350128bef" },
                { "en-US", "677ed08d91530b0790306f3eeb49255084c46c58a352fd072c88853382435b175dfa0ffead45121e837d0219909bceae90cd1e658cbe711cfd841c4ab363e2ad" },
                { "eo", "b32d594d962b3295290a3c72b4406ce788a4025ce80e6f52c22ebd3797e688ea27b48e5e59ec831396c23e06446749d2c4f05833d64fe9eaea9a05c245bf8a24" },
                { "es-AR", "23fe0fc2401e051f976312713c593a5c79f0b02ab98ddf8fe6ded9f94a8c30b86bea473797b80af893aa9168ae378376f209ba5837fc552c3343d1f8381f7556" },
                { "es-CL", "d14eaa23ed21e248d43ee406c7d74a5d5c451c89700f2a0ab2bd03748f970c15b92c904397ec3d95315d417005e40f465bc9d468af076c8c2b5f2671489a75e7" },
                { "es-ES", "6f26e9b3d176db4a2a8b578c04140d7df5b7afd881e4109e7a957e318b19fb38aa33e0e730bfb7e7734c091e48e35650ec9f37ff95c49333914f866d60296c20" },
                { "es-MX", "9986c49db4acf1b86b63906c0d07663f0d1ba2a1e3177a06835534ce89f7596b245eae0b9cbf258b0422f6508c052a165619cca440b8770c4bd16bec06e3c315" },
                { "et", "6b5717c61b29c0795da7bd6e48fa80c5134c5652fd702fc421f32cde01f929dc39a9d41e16264bd1de86b1b3de7815e85300d1654e75d318551416faa12cfd4d" },
                { "eu", "8c7b40d0c5f444a04658cdf590dfb078fa94dc84e97e363f09f3092ece2a15868d92f3e70a1d03f1ce55b7e26eeddbb81c35a0875e974e44a200e95d4a4b297d" },
                { "fa", "29b1e5337cd32e3a77f921f7c9162b4f6cdb666b13d4f154d1edd658310f22c9527b0f9c81c6fa0aa5eabcec494eea1da218be48cef14d25c04c74d0d1fd429d" },
                { "ff", "2ce0c25e7c2e6eda93a3c43f10e5db8e68adfe181a00177c4438fee8347f104417dcec61de0527c475f3ac7c88e1e40a7e907efb4c40c761fe4b31f6b1e12d22" },
                { "fi", "d56d87d95865eb248f2f6a3db4bca76504a4d768da73c59c4002f8b0b9891183eda9e6f4bbbfdddce7f2b4a1a6437a2919d9e1359c18ec998af2548bab8b1b87" },
                { "fr", "cc8efc295d400cbebeadfdec15267072581eb6db001ced62e2819591e84827636baa7ef79d2c6d90dc77a04d92512b9513f0b456a0a1ce2c0b8a640f27735130" },
                { "fur", "a619ac8a9278995372007de57fecd47ec4efc0bea971e21e58f033a604c2b6a38c64b5402372516151e0bbaf30035181eac965e0f039d690cf93bdf3a818780b" },
                { "fy-NL", "954f8f4a52f6d5a341b766ceec17413ac639c0d68f2172d034c241ed64012394ffe81d1b48f35217a9d2446e93ecee055cb0f03df2835754a283310abbd3269c" },
                { "ga-IE", "b0fc2dd49b008e1ad1426615b39e721f322d727169a97946f6580d17d57d7508801ff45714528ffb540785187a2c3c10791fa8b213092b2bbeebf9f0ded03acc" },
                { "gd", "d732afd54b486f45882b594c2cc9b614922f25742146067cdffd3584e74f167a73b188e4853600e6166aed69caa68c2a26463bd2e2f9f4c68072dacd766a0217" },
                { "gl", "dc9c1e5c64c9366126552b78d223f2ea51ed6eedd064ba8b9dc85693abf01fc112756a0f77e90b020b763c6f9eba3db4ebb9337f79a7ecb07d39644adce45996" },
                { "gn", "3cea805b655ab298c8911157ad87ba77a73314c8826bc02f05524b5c859751203720b6865b9eeea28fc2571266fd747f1c396e200748136c7c304848094c2b0e" },
                { "gu-IN", "060871817406d5720111fee622b281fd436f8f53c44b60813a2b1844e7d97843004fe4eacb704b99138ccc72d3918fcd7acd919d21e498e6288b5b80fda09047" },
                { "he", "8f067a2217091857374bb8b25fe8512eb530c3f849ac198940aa127d90714023f9126a2d48b543b37021fdcf60cd47ac680d01192179dc2e8cdda593dd999434" },
                { "hi-IN", "732b2a815eea050e369ac18059b055be5931d65aedf306735c16a0995ff880ca4f8d1ebae468e537bf8880dff9ab5d2bd707ddfd972b4b7741bb15234f9aed0d" },
                { "hr", "dacd0c5401cf1967a3e0e7368bb7aec7cb14dcba866e113f7d798ed2cc0d5de3334d24a7acca5b099d15f24a924ec34dd715ed52bea13971129ef01eae7d3c29" },
                { "hsb", "5d8a19679bd8097359a25251e92044250491f3068e741be829bc2d0442eb4aec8dc10d7bf26989fb5b9053a58cd4783320a052a4708d2b928bb8e498cd51da79" },
                { "hu", "776a0ff1500b0c7d1288385de3dc07101b894674bb987bc8b23ef1eacd739b8dff0d23b3c4e031fa958622c1ec0e9a9796950d737777a4c8999209bf27914bb6" },
                { "hy-AM", "02cce76f01ecff42162867e8d8176a8ec10ae65993b893336716a00669a3ca5a1db6d9b30fecc664b7a249dea1500324a1c4c5f940d8a648eddc02955a58591f" },
                { "ia", "c014487f2f570b941fdccfe6e69a18affd9839fa04ceb91b49291773f81d3abfd549a54f4f82eeaea9eb2586458b9b27646ceebe22322ecbdf60e33cfbd93b85" },
                { "id", "3187a48997b4c136bce123536d2ea6249e582f3d2bce2a0ffd1ec10e005f1ad57d9038ccdb6c4f191c278807449b23285cdbeb030e93afbe6989e96850973126" },
                { "is", "4dd493412747e4b08bd7203f7ea56570ef7306f1de6bed0d7afc6ff2bb89d22da96cf926f9c6b985dc5b5c243af6a83045d63be138e4a9adf666af35e441e5b0" },
                { "it", "544b0f1d61b5b4af1154d588e41dae4fb8581783bc4917ec2aa9764df0e2184188df29792a6d83c5cf54a35376cb1467a5ceaab33d133f770fd2a4767c313627" },
                { "ja", "6f4ebee297d04c906bf8087e634d3aca509b443635248064d6deeda823bd017180a594a238015e61ece7cbe036aa8340385d9a87c7ea4868a7f16a40721d87aa" },
                { "ka", "3b14ea6b9736a692745417852c1d5470026415a8a17414c0876cdd307cea7564f3c019fd18e221194281ec34caed6c367f1f7a01ce505b31277818d0f1ea536e" },
                { "kab", "a293b8bc5a9273b54083dcd84e72df95a09878e08b654899ef859879155d3d7e93ad16cf90724465123f68c96167c0b1482b345da2b667fceefb87b268fa0a64" },
                { "kk", "a8668643de01a9dddb87db07be94c9b849203d48359e97212516a0b9036502307f5ca7b38d6ba3989db92fc3f2e19f11c7c6fe3ee3a904f9f62094c88137f154" },
                { "km", "46ff2b474050cad39079efa11b9aff49c07d267ba8591e988f273b49c89f5e51218dc363b6b2e355bc237cb267445a98e279bf42052de4c7900f51df02965601" },
                { "kn", "43702ac461c63f918cf1aea16a85cc570a1ab6342fe8007cced254221a3a484a2dcc75260cef8e742c8e871bc5a71a2994c28a50c458356ea2309001560e9778" },
                { "ko", "74fff7f61e35f5cec24a0c9e04dd538303e7a592fa110891fb431048028de9a417b89089ce4653ddfde82a80ac1cf410b595a913057064d76daec95156243673" },
                { "lij", "064a6bdfd8f1ffc24f8046720d1d88d2768da461a5de5422f0aefaf690264666d39e82be531bdcbcc55d0f29d317ef438742e3270a4f73ab7d754179157da2ca" },
                { "lt", "7d526e5af5c3aa00f6852ee92ad73f7fae073052906bf9efa57922ce663212221869c814c805aa0e81075ad738ac6e450eb037d813d632c53a3baea9880c63d0" },
                { "lv", "646bfc771857ee93f2ffca051dd93c6a5b60bfaa875cae0575a5ffa29c8147049a7d33d14ab6f04ac951f936ccf7dc9508473da95932a8b20c3825defb73a79e" },
                { "mk", "00ffb793444eab5b7cf32d86ea8c6ee332397f820e6de4e1b92ed447f764c70922a62dd1091ccbca44a116412c81ebc2c8c5afb8db061832a1da667eb8077baa" },
                { "mr", "180bfa123d1695263d061d266136241c791b8c489e900f50e155bf702c30665d9996c3f1c2c8f5c9ecdc80f7e76d8bc54673ec4b204b80301c0b3dbb8d39703d" },
                { "ms", "552bc55c1a7e91b2c26a9701b6098113183d4bbd0f845a229a9ea1fac7a08354d2e4e42c7f8d2680e02c221c82a0c3a54ebb7e9215bf81a3f14513016f1042fa" },
                { "my", "d6b5f870fe8163196bd2ca5349b52a29d6f9ae238ef9d0ead78504437f155d5863b61502cf8b32fb6cdcd0e4f7178ddd8a1a9e46a533606ee0685e1a3dd3ef81" },
                { "nb-NO", "967ce1d0c4bdfde5e2ffa445089d8fba29cb54e384b079426632dcf8e5eb8661fdde35f9a8fcc9dbf89b8ac01a0bf138b2b027d33320cff4ac60984755c0d7b4" },
                { "ne-NP", "e41b80ae5e5734de806e73a8d59c73b467243581edfef572be63d20810bdae6ec7d75126f5357d2ee6000999c0578c63cf6efa8008a43bb58e4fd86cee028186" },
                { "nl", "88617b6d0c0e30f8ca6d9630428223c006931eec6f13403988f000b95652ccf45deb48369a2b158045f32676bd1ffc27fcc12e50bfa56586f3b02f6ed8178497" },
                { "nn-NO", "5ceef1abb152795e514aa5ce395d8d1517b7fb0fc17c8d6425bdd1eed9b00acf77d8b2092eb2abb2147ad41c35a705f928cefb307d4fa470021506eb7c2269c7" },
                { "oc", "53d9dccd438d719dde7fcf2ad07138797e25a086592c3e63331475747491cfa51a01d8edbac98cca82f78d64c6b69c7e833740eec61d15a09a4686206bc6df39" },
                { "pa-IN", "0b1f1373b876158ed2cdb4759d2b4afa612ad28c25d506607b408406da9c6d71c891755cce2b3f11fd498a59d4e8bdb3c186828e07d6e0731c31bd6b5aeaa66b" },
                { "pl", "91f58ffde26bd0c43db25b651e54c0a6eca343caf008c99eb86057ba8840d5d05f96b156ec0a1ecb64c057e918ae6b8346b47f813c5f8b84dc1995869e179267" },
                { "pt-BR", "fc0ba5c0facb74ce21b5d1a2f6a66a04a868439300cda181b5b440007d90ddce1dec07f927430a7a0723e23efd8ec348e65f03e47ad927b44c9b071494613715" },
                { "pt-PT", "d84eb734b801d78a4ee185b4432d2ffadd277be5e5e739e29657394ef5081fb3314461cdc4c5caad9478b7b21153fbb3b70c2df238eb0638c0c32d415e6f2f7f" },
                { "rm", "a9909a4d739cc6a927ac920ac044c60f7dceae80a9d8c9cf7ef98dba15bf76daa898e795e846be696dae890a3cbf3e948d9654d8fc691b6117611d997cafe915" },
                { "ro", "3392705b46b24b2f4368c0a9f7974cd3bc5361ac710c05dcf557f48070c661b53e775df2f810de1df984768bbbdeb8c72d4845eb0b7e70e8143e49ca60dfd96f" },
                { "ru", "2ef0eb03349484e9d7653b67f2f2e35595274a1ae4d7afb0607c94135509117dba2ec371c7300afa7ad107bacab69f55507499f0bc96b92be03ca89c8f9302a4" },
                { "sat", "67c894ac76e2c77cdcc554f2b5bd8f1d0c8605032696debcd91b96566c0e5ec679dedc643350d3ce449b7d1098d999bf6f4fca6f4e4fdb4ceb410d3de9fb3124" },
                { "sc", "8116c6b16dda3f45b0c6b32127eb8d10a9f20d7fd168d058168a237b91d83144ffac54f4621bb6bcc6cecbdbf8194ac5ac32215113f89e96001d2a599776174d" },
                { "sco", "a1bf3cdaa58c64d95c745e93864fd5ad5cb7153eb519948c3f5f9f4e547f18f51c909ad3db37fa43876c908b24e73041cadb3961e8fd06fa86df7a26bb4fe22b" },
                { "si", "def61343c0a06163f44b5ac0f854828ddad7f6dfcef089a6908d58b878f2323ea0d54af290ad94188fc5384e92c3cf7c5ccdafc5bd0c8a906e812b73f14b0281" },
                { "sk", "f6d8b20a098807738126273a5bb101fcecbf5f8661735daeab9f350b1c1c0cfe1a6b6c890859f183f062c02a8ace4998b9e7b57d71acd8a213232ad3a2381f8a" },
                { "skr", "8bbb6bb67ad7dbcca618ba90d5923c8a413af6916a23c1158d91314d9095a6bc9c44d8bb145eee8be8eb6c597e173a0136abc360c491e2ab55c601cd44404baa" },
                { "sl", "ebf82ced55998acf46ebdb1d0516527fafe832d632b1ac884c7138d18c2fb6db1cba95118aeb12db2b76c32ee733b9ac970de12ad08e8a3b25f585005ef33d61" },
                { "son", "c0538bd39ce1b0a124990b302a136201c0e8e136081aeea90814f2c806e5655dbc5b7b85c6011516affd85f9ab6be96d55c4ad27a76841dc12f90cebb627a35c" },
                { "sq", "9d2546aaaf5e14ec5084f40fd27f138846099a3a911a5bd04bb5008388687b715cadffafd1f11b9a53596b48ae291022fb046c9c92357f09e0069f8ae88174ca" },
                { "sr", "9e4eb3ca6424747b2a441f4845a64ae3427854fb9e8ef1751290a5b8d2e7782dbbf56454af8dc6a7f9a093801f75f87940d11c2785cc8dfa821b6264f3873667" },
                { "sv-SE", "2256e7e433dd6dda14cbad1932b8248373009d1f28abf1c0e31375c641c7b2877717ea774c091237cb3de84111f21f257a5ee2b94504030e674b818c85cab803" },
                { "szl", "fbdbb60d4c9540e03f7ad119943ee9cd1d03333a33b9dba1811a2ac7d3349c3e29044e5a0a8f472013024b01421c6ffe4b0e0e630cc1010a019d20a767570bdb" },
                { "ta", "d77158971a0f491369c53764d6375c4791c7d70b59fbce43158372183148ed2be5ee364b19dbe056bf483981ac706479e29ae919f3a339863f27f5205a975be6" },
                { "te", "c4bd49a998606de91f4a8117be3ed8fe018e6b4d216833a6dd70b0f767724293f4633775cf82dec437c21819afb73d05d5e9f444f3ee35b9db0d4c3d3df2915a" },
                { "tg", "32595b0ea1b7c6d567e08847d921d7e1e921f009e3da736ed382f73521df91f9d47aa182705d27f48673403cccdd4ecb8cf2457dcbd39538ae9b1a54a9a79934" },
                { "th", "518a51d7ff822228b4b60ba5c89af642a8eddbf3d798964f3d12d584ec09b66dff541fe18a268bdb2ff874d9aa15339eec77c62701b607fa35da3e55d423ee2b" },
                { "tl", "aaef104bb5d674db8a5b82d85b5e50cb4f4169da96ca9eb18ddac192d3e66b8831cbb1d9c967d0630eee01ab69c58a70a760aab8d2d29d88bfe4067bbfbbab43" },
                { "tr", "ef67d4a3456c66bf0f651219a713da19654bc63aa6a9dfa88aba2e88b152dcd09eec4f7dd11347db521b86972e645b7e0a1be4673224b7793cb1f42b3ad21d00" },
                { "trs", "4b8d9ba8cfc46edaa0349f1517ecfa7f6b2d4f4ce78d60c475209ad0d2c831ae1ab42a41f91c41bc67ada3241dd05123b0694e2eaf03354ad7766d2fc235fce3" },
                { "uk", "6d1f46c1e059960f45bd5fa6b6b89c9dd864131f53f1e9cf34d859520e7c70d182bf4e95fc314a971e641d82d96177aad6d3a72458c9f6c8bff9f344112251a8" },
                { "ur", "83a42fe74e57e4ef881773a3bc41a82f39324a1487f23a2fa3dc15eb8552c2f32a0c52085195d7a06f119653b2d29d4b5b75a892872d1794184206608ec7d6eb" },
                { "uz", "4472606446b4e591f0938be5b2948285505e6efba170cb57434d5aaacdf4fae6f12a096582da2312740c4121f09cc52103b55f4c3e3298be0b2ea4e43dfa5350" },
                { "vi", "d388b1b418fedd25af4b2e9b483f5c3d648303d1ad1c2517cf6a8c58878e970da70016a143c0ad637d87bd55953e2735bde4de7c32f3fc180cc21f37b706dace" },
                { "xh", "15bae169f87067df0f9276420a6592426e83ed95096bc93ed1c043c5de5c984541f25d9c1bfea37bdef3855e2f6e613c4ad10aa12e8a9df4fb425fde2260b20d" },
                { "zh-CN", "cbc4cda2aca70ea560a510741949d5b5ba06d5876c6280a1bda5cba869eb8218e8757e2e158369b6db5a2108b8e661a34fac4c136130e7f127f3857e756966fe" },
                { "zh-TW", "ff2de62db1f9ec3c3623f6871310fd9a65991fa6842a7c73a79aa358fcda21beaaf8e9baf803a1c920fd3de4bef7abac1d369e77e735bea17128c421dd8d25eb" }
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
            var signature = new Signature(publisherX509, certificateExpiration);
            return new AvailableSoftware("Mozilla Firefox ESR (" + languageCode + ")",
                knownVersion,
                "^Mozilla Firefox( [0-9]+\\.[0-9]+(\\.[0-9]+)?)? ESR \\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Mozilla Firefox( [0-9]+\\.[0-9]+(\\.[0-9]+)?)? ESR \\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32-bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/firefox/releases/" + knownVersion + "esr/win32/" + languageCode + "/Firefox%20Setup%20" + knownVersion + "esr.exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64-bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/firefox/releases/" + knownVersion + "esr/win64/" + languageCode + "/Firefox%20Setup%20" + knownVersion + "esr.exe",
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
            return ["firefox-esr", "firefox-esr-" + languageCode.ToLower()];
        }


        /// <summary>
        /// Tries to find the newest version number of Firefox ESR.
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        public string determineNewestVersion()
        {
            string url = "https://download.mozilla.org/?product=firefox-esr-latest&os=win&lang=" + languageCode;
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
                client = null;
                response = null;
                var reVersion = new Regex("[0-9]+\\.[0-9]+(\\.[0-9]+)?");
                Match matchVersion = reVersion.Match(newLocation);
                if (!matchVersion.Success)
                    return null;
                Triple current = new(matchVersion.Value);
                Triple known = new(knownVersion);
                if (known > current)
                {
                    return knownVersion;
                }
                return matchVersion.Value;
            }
            catch (Exception ex)
            {
                logger.Warn("Error while looking for newer Firefox ESR version: " + ex.Message);
                return null;
            }
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
             * https://ftp.mozilla.org/pub/firefox/releases/45.7.0esr/SHA512SUMS
             * Common lines look like
             * "a59849ff...6761  win32/en-GB/Firefox Setup 45.7.0esr.exe"
             */

            string url = "https://ftp.mozilla.org/pub/firefox/releases/" + newerVersion + "esr/SHA512SUMS";
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
                logger.Warn("Exception occurred while checking for newer version of Firefox ESR: " + ex.Message);
                return null;
            }
            // look for line with the correct language code and version for 32-bit
            var reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "esr\\.exe");
            Match matchChecksum32Bit = reChecksum32Bit.Match(sha512SumsContent);
            if (!matchChecksum32Bit.Success)
                return null;
            // look for line with the correct language code and version for 64-bit
            var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "esr\\.exe");
            Match matchChecksum64Bit = reChecksum64Bit.Match(sha512SumsContent);
            if (!matchChecksum64Bit.Success)
                return null;
            // Checksum is the first 128 characters of the match.
            return [matchChecksum32Bit.Value[..128], matchChecksum64Bit.Value[..128]];
        }


        /// <summary>
        /// Lists names of processes that might block an update, e.g. because
        /// the application cannot be updated while it is running.
        /// </summary>
        /// <param name="detected">currently installed / detected software version</param>
        /// <returns>Returns a list of process names that block the upgrade.</returns>
        public override List<string> blockerProcesses(DetectedSoftware detected)
        {
            // Firefox ESR can be updated, even while it is running, so there
            // is no need to list firefox.exe here.
            return [];
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
            logger.Info("Searching for newer version of Firefox ESR (" + languageCode + ")...");
            string newerVersion = determineNewestVersion();
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            // If versions match, we can return the current information.
            var currentInfo = knownInfo();
            var newTriple = new versions.Triple(newerVersion);
            var currentTriple = new versions.Triple(currentInfo.newestVersion);
            if (newerVersion == currentInfo.newestVersion || newTriple < currentTriple)
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
        /// language code for the Firefox ESR version
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
    } // class
} // namespace
