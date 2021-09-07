﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2019, 2020, 2021  Dirk Stolle

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
using System.Net;
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
        private static readonly DateTime certificateExpiration = new DateTime(2024, 6, 19, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// the currently known newest version
        /// </summary>
        private const string currentVersion = "93.0b1";

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
                throw new ArgumentNullException("langCode", "The language code must not be null, empty or whitespace!");
            }
            languageCode = langCode.Trim();
            var validCodes = validLanguageCodes();
            if (!validCodes.Contains<string>(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException("langCode", "The string '" + langCode + "' does not represent a valid language code!");
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
            // https://ftp.mozilla.org/pub/devedition/releases/93.0b1/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "8b22d443bf3b189acbfbbd94e3c48441a0962dffa5055256ee1aa106553d5e71d8255e2286abae79bb4a44edd8854612dedd367050d1cf2291f745a6d76a9195" },
                { "af", "fb2a6fbf5eb4cdf8ab88f85077ea5f8975f334d8bc3db90c92a53199ccae91ce49ea32b43f8b737cc13e9711c14d1f925a0496227d82708f3c468dbe70337050" },
                { "an", "25b3162436dd81c31cce2cd3ff93688452a124ae627c9240045d4d3b1435dbb959dc4224e2681a33743cb30e5fb15d2c7d9bbce89232842d9e62ac90ea4b8b7e" },
                { "ar", "9dc9c00dd8ad4c55d743e7b28ba34c989175216333a2c5e8634e831825d491f48f6f050b2e1b3710b7016df07013683f31942fc005645e798de56485dd962664" },
                { "ast", "33a31b3b047f7abf925ba76bd185fdece681939513251c23c5d66d88c0748f528fbb69c2c321a0c5c1cec70936df9d46f5a2b25cf162c7ed9f0bbbd937b696bf" },
                { "az", "150a0b088da9413fabb5644d8fcfbaf939f436011b18282c6ecf11424497c8082582b303dc0d75c4f7d565e80f5f158b34d2c72273b54b67c00c06580c5986f7" },
                { "be", "be7d1e2a6f3bd56a1673f13180f8f5dd82bd18e03ab457cd996a39d799262af93ccfeb37254a5a567db1870f27d2f6efbdf22a13f6fa0c2096edc60a5a7ce419" },
                { "bg", "d87de86bb3f66f4adfec5124d701d9d0dbfa3bdd7c473cc2bb5a878b0bec072dfbac75fefc1d084308c01a390a005df5881ee1619eed76e4d82478428118b7aa" },
                { "bn", "04e71f371b9df8658a456ec661ae141a315573d8cc3779ccdb00e1a9bd5328e1901d019d39027c6d8e3c6868b6e98a74a2746f0302143d5a297f920646190995" },
                { "br", "603ab4bb1c05dd0bad01858d9331eee51e611fdb4f89bd6ce7532c1437f50128700c420b3876b1dd09efbc292d0fa60fee0bdcf07239b538ad45d5c24cd9cbc9" },
                { "bs", "835f5323a3897ae17d292b97eb42e2677660167bad71c7093119d597570115460209bd3e1162badc83365b8f0adc553c23c3abff5ad843e70816de3f67279337" },
                { "ca", "969a727110fd93d75b6baab28b376498d225a0b92d3a6be1b93f0aac9dd7e18de3fd64d6a4260cc8ef495fa4c9d1121fd0f94dc43276a53be631a20447494388" },
                { "cak", "4f1f0e1c6c27e15a8cf75d45a5b6dd41dca900cf5ae1a3962e34d9757349239efc32ad2e9aec4f08714308dee4665a3e5af54b05592eacf1718282b7ff8af792" },
                { "cs", "e6bc4171fd6608d32eadc221a267c62f255076d85b41bcc7690330ba1cee458f59ddb4c575f70c3a32128d5f413060e28802fce96a3d99b28582e68ccc212a0e" },
                { "cy", "ff538e5799c559c43e5aff787f013e8b57539cd799daa4b43fda897058df12107defadf093b8e45c5c2bdea213c602d846af2b5bf81a35647e0e2626e870f9a0" },
                { "da", "d2c50f6e3e914e9e28266d05662b211c0875a965f2694c70c4777d1af6a76bdb7b9759a2bfda122feef7cb51609a6159581459819a2ef07659a79e1f4b1a2083" },
                { "de", "5c501045d0d3315bdb6df5edf353373e4247ce79417c4a16416e761932ced1fe80627c7358ca9c71a5f80deaa3ac23f93523eee4b0f791e81ed42f05a1409b4f" },
                { "dsb", "96a5edf233094927eb2554bf6543232ab914f7bfa529613859dddd3f96e667f75cf1546f0386cd20a0589ae69fbac157550e8a16a96c09026adff96988c1acbd" },
                { "el", "4045b23ede1b379f76f60077be9d59e1f13edc4dade5ea3f15cf0d34c80d4e80f15b417c042c1ea9d6d7d1aa400eada5ced9b9a3cb1ef470ed613784603fd006" },
                { "en-CA", "49de201e63aefea9f2c70974031ec82349e06fd1734f68ffb2139884db52173eafb0a752b8d6b7c5d6a563a03fec640f7f4200519d86a559334f9ea85d2b1184" },
                { "en-GB", "929cec5590771ae07d93870acffe9bf83f23162fe288b6e5922352f15426aa0debaf29b1efb204f6cbf44e1074c21bda99e12e998248d2d9d183e92543391a2f" },
                { "en-US", "5ae480b33bf3909006c84e435ec7104a3c025acf75bf557b57a4b1285f21fb9ab1d3ddf30743b5b4f5974fc7c4ccbccbf67a261878d22ee68ebeb1bba6ae8a36" },
                { "eo", "d2e58f51a692d6a6fc460d5f0c7491ea902a4a63bb66baab046c7a28527a7590d87532fdf59813a11e9319c332ea4afb7c975fa41073d4d4d66a7ec6fb66ad0d" },
                { "es-AR", "efe9758e77fa6358360ee1efb11bb339d00a779070154fdfbee85275d819dc83776346cd1be07467e40a6881be2809822f980855abf2acf950a0dd7ba09e1e6e" },
                { "es-CL", "201d2eb66004ddd22c67ab4e7187e79468ce76e3568e5eadb13a41c73778a7f35c59a32de9a2cda11ca7b192e0a31fc7a44a8fdd4bb69604b676cfb6749e096e" },
                { "es-ES", "f2f82b2b5c1b98f46cb8f3f82f3c0eb1091719b1e4b7d598e15957f377522631daeafd3a801be5fda3dd05ed46016d7fb936ea93921302626a7ea1af52b2cf15" },
                { "es-MX", "bfdae4d4ead41fc02ed81bbe177f1eadd26efa50902c56a34d1ebe22bba3c0b55dc6c385188dfb22d0cfc108e510e5bd93672d6787e2b2eb8cd0ebf54ddebc5c" },
                { "et", "064c76277821d0ed463dd1b3d3b6c89714c3b163a0caba7715959efb5a54868c22ac8718066534264e7e1ae10294f93c23b5d514d6d10acadcd7fea462930b41" },
                { "eu", "a6e3cc2f30e4d27f9fdfc9820f1b7303298484830e2b3fbbfe834f330eb3df0e2b45d4b176580d0aadb3c51be19c44cfbc32344b8cb5b5e53d34f127b25f1cb8" },
                { "fa", "046d30e37bd3e220f164aa7e9fe74dcc360d8f683058a8f94f63b94c387ac8b8468d7308bd5a952b51142332e1155a1410f81e145e24c19c85061e4d4fa7ca1f" },
                { "ff", "8da5112185e3bcdc8a5399f017fc3f695da54290ad44c4f245b4fa167ba23a16724d56846bb64fe2b853868bd46bf9570c40a61cf0853ec6af4103d5078595d2" },
                { "fi", "f6021fc26b1574010977e541a639178dfadc734c03e9bce8d5c3c309c393b1da29441b4a484e66a725b1cae68dd48583090f19780bd457c65d1fa72fec869775" },
                { "fr", "929a6f1b7b66e5a43972bc63274e4b486688678b31a0b9e31601340593a8e1442b75d4878ce75c6705b1b3a94cdd3d8543c12026ad4965bfe34a016fb1237009" },
                { "fy-NL", "9b84d7dca27781be0aad7b0f2cf7109552f71ce6eeefe4e52803f88a81e2a6aa145c979481835a79605a67d1ba1525ff7db63dc72e5e205ed9a488112a71a7d7" },
                { "ga-IE", "0b09dce961f9f93b8cda6182c649ca7811f9635949bb360b1fd817137e5ed07417c6a1ef6e21ee8dccb8b5a36d9fc3ff25a0ccd524ce5240d0999dbb038bcf47" },
                { "gd", "61ce67aea87b0de57499285fb30b032608f08724d5719ec71ff27f05e0201397fce181017833eb6e76a82dd3d3ea8191d6dbb4547f00059b295a6a28773c11bf" },
                { "gl", "87e39b34f72903e32fb7f1c976cb4f134474f028007c050ad2fd35f1de8250fd8a9fdfd2672307f481dbb615de8cd864bca006d81ffe8430dc2014808aab77fd" },
                { "gn", "42bce371d35a9eac4e635b49ea5f558f60e3470c71e5a3f479aed673fb114ab083938080c0dfeac41e286b156b7e32b4746511b5ccd66139a16a55d46e2b5231" },
                { "gu-IN", "1568114ee39edd1ea3dea5667f8a417b728d8c22984063e7f46a5deb8d7ef3f864a98ad5d87c3d798a0378fa6856afa54a515ebba9530fb2dd17f73f780f36c1" },
                { "he", "af238990083098384e05792e295c434d31cf148545ac104ddb7995d1c4a18d563c4714baca514ee318e066f0632f7113fc4eec6dda827ad0ae8f850f017aa35b" },
                { "hi-IN", "556a29fbac0a6a01bc801670e9345d15147b59e3fc36b8ccd844421f9f49ecb419a4b6862bd47ff727033275f1e9988db055d9b37e91f955cd71440f06369fcd" },
                { "hr", "35bef0aca6fda50f8c47046758c12ad5b9356d232d4f722b7a72b92f86814ab51388169dbbf5010fd1eeb71b501eab3a1d5179c2f196dc699d8a89e283ca1836" },
                { "hsb", "a485d88d6ea268ad5ca6dfcb73dc3d31ca40fbc5fab7c6dd4e3243772ad5f5dfbec2433bff3e1c95bb5e1977410a7cad2d9791b5bceea10895770a8cd8ba3fe2" },
                { "hu", "802001d5859fb29b2a2f47ee805a1fff4f2e9cad46f767aa5af3717dec5fa3196b45db308c68158bfb3490fc0423ea1ae95cb0946bade0cb0bd122f608f8ac5f" },
                { "hy-AM", "f7dd49ed670a1bc1c36c0297188dc817d4a22e1d7a450500119b4f55341040a8beb0fe51c0f712caad5de7a1643b44e7d6d3a34d0b84ae987f38f1dfe5994850" },
                { "ia", "4de78b8e7e0c47fbc8d0db6fa2e0c97e2b8e53c953f60b3030bc9b3777787064f391190e6072a6cbd197239a17984282b368e041178c70bfc899d02baa18d9eb" },
                { "id", "2a20c83a339d2f9051e9fd1d0426b8e027f197548edf74d5a4e549cb23146a3d5c7026ac82492124e5821e864f4e7f1f6eeee8d0c87c76ced907f5b62581490b" },
                { "is", "ec3e6c6e433e84132934060797d78b08c779b968b5b31be7cfe367e6de794d8bbccf63310fe2d96851daae6bcd08cf13dccb250f128fc3fcbba5649dc195c20f" },
                { "it", "a1682d5d328a139c4d4570a4f67d31cd90d0d0e9fe8d55a4138898d84abff26c1338ac98563194f7bb714ecc39f2bbdbdfc805d35dba8b32b645cc45b2a0e54b" },
                { "ja", "3f5cbf42ed289ced4fc0fedcf8ac55377562fba60e6a1cd8e751c0e761126fd92e7c10f9b6c1a9fa20d5a01e1317ffbc14cef4d529d134b442f6a2c0b0c05ec9" },
                { "ka", "79f31979830987a73b7e120486dd077f1844092f49d578e4120a531c1cd5d4527184c89cf4c8c166ef2c43322976e2c4e612b84f754b8526e6b192db0a9caf54" },
                { "kab", "1f6f15def5ec271772dc66358785cba42a7863f8b7aea7d811661f06135dbf43953e15f7574b2a97af59972ca392b8ffe1597585c51364dbe5500ade8f5f0469" },
                { "kk", "335a7802be7987fe68e3b6d0861222093e43612b67f87d4505d4fe1c0631867c2acddaf8046401ed30a808545cea3e170feba962608b9e8146e5af79c3ee85a1" },
                { "km", "c163650a059aa73b722374bca404854c4354adceed8cb866220e7c7f3498ef3d00168d83022e1287894998b580ddc3a5bdeeebaaace21807bfc6e575cd825d1a" },
                { "kn", "2c9d7933f98da0b4b11dcf5bb88eac76043faaeaaaa79543697052f4ff675881cbca459dd8fec09f154001e7ce53050587013c5e8f27837564f60a1ba627b4a7" },
                { "ko", "97f679f1662b9edd87568ef792862488b9e84787f3df294d6380ccf184ab914f13e0b513d7af62bceb81375eb3e224b6ed815199ac5bbfcfef34224822953665" },
                { "lij", "3d92064d0b302346e95855d69f72c2b85c7207091b22e54c23e627d12d34708537f4054e2b43efd1af53179920e8f932dfa3ce7d5650276bc87cbdfa6d8c22a2" },
                { "lt", "0fe917340780945e97fd69689c353a94b3c33a5c809fbc0e3aa91324ba9c26704c0a5848a661061bb3d8fd82cc2ad1970124caad79972b7de8056091c0ce30db" },
                { "lv", "62f87be2f63bd317baff9dcad54189bd061056df7b6d17355b442091ef58e6b612178f6cf9c1a1b2150cf4d8f624cc8e5415b008f8b6ba7d282cbd9426ad71bf" },
                { "mk", "5874a62da23c61b719f6ca6b21588b221814378f317193074d698ab99e5bf4e3e54d5d4d5f1491b5f1b0df2e63d2a91861e57131612d1a709fb1192e8aab1581" },
                { "mr", "3553e5f4de7d0c01f2bf89a3fda260b4a156365bdc6db5b07b758b1c8af70151c93a1695a4fb707d62e0b5b586c154e490baa0e140c377496c20d9e54ad5be45" },
                { "ms", "7d20d25300e97c660d48ffd76286068cf3323a41d3c8661c60dc8d1d894fe8a56698ea474484378535d24f7a7da5e48fe387397cd75ab99f3c38f186f16f94ad" },
                { "my", "e3833f8c3c78be9f69032af0a2cc3521a583b681764b09a3686a00c1898e278b6d5c6753a9985e4397016bd7128c7a636ca6036f0dd37ffc896247699b3d3dfc" },
                { "nb-NO", "14aaf61d148551f26c68f71d0b381e8f1af3c9d38002d866c808963c2137d227e74189bb6bd2d987692696ea50c595777dd4b423c976a8bbc3058acacfb6ff3c" },
                { "ne-NP", "7c0d12e7fdecd52210cd6dfcdf6e3266c1e91e422b89ea472064faabcdca93d6279e9ab1d479875f5f270f69b45932500faec0f1a3397fba5ad136adf8366e6b" },
                { "nl", "01109ef8356e1698a6dd7173c3583d352c269da31005df8da5ea73d061430c8640b061c8674dd81db1a774114bd0d5505ba8f9ac71b7383aa467b86a654547ab" },
                { "nn-NO", "7f0a86421d2d1445b8bb0f88ab5956d64e6448133073bc59be9220ae45dcd80ab06026aa4c9dbb46f9f145c2104d43f6d6b3092014a5f3b7d840637023e59f43" },
                { "oc", "35dd2df7651f30bd8dd33c7829a0a31ccd2b69a951ee294d1dbee689cac0cd0a97cf0370721e6db9a9f44381ef814732412b185cbdc39e332e2085fe719e22e6" },
                { "pa-IN", "6c6f3f2f0f22cc710275a7bcf965a221a66d06f29d761d825d4018f85d061f072c092d86b692562209a27c3241c7556c9b400d805a7260d3a448734d645a08a6" },
                { "pl", "b6bbeb38935002bf89bbbbcb70403218f47915ca877e7211d26b06d68567af799fa21ee05cbfaeff45ac2a349ca38d9d06be69381d8f6f1471db47a0a277a734" },
                { "pt-BR", "7bea409c5494d96175efffad02f03fd4d64e8bbdab502cc3b6b48cc00de9766a2185febe1af0da89e8d8f9581820659d5efd37f9da257cc5b8ec47aeab231184" },
                { "pt-PT", "1f159d300d63be1d91aa7ceec93f9523c19157acfa057ec262a0521a6accd33c32b0a7c7c773725f3bec80d148b822197c30fd58f851676ad1d97a9f81b9f7c8" },
                { "rm", "01ee933ecca6bb63babf59b6e0c1fa072d72e3a5091eab79737fad8bb98145821f0fae21c32ecbc356d6e8b5008a6218b01f2723de3c7b01a570dfffef5e7e7e" },
                { "ro", "81379c13f4437349db00adce14dc02584e797fe3c8f5a526be6af8e42ce28e4c046b616a47f72a0b6bf682a943d5a95bf640464fec3ff1e947df8aba1bdeae99" },
                { "ru", "ffe07f796daed414b113ab22928a03954862a7efc4be63fd76da25a6133740b1bc243660317104ade6c6029a557e071569cd8ef5c81e6636e555f37ba90dfde6" },
                { "sco", "351d8e06d73621313b99ad3b1360f224e6a9fc71349e4df2cb87892091eea8002542cdf1b99da48b0a213993ccf45a2624e043b5e7b5e37a230ac662eacd3c27" },
                { "si", "4cfcd772464af3a4ec8a42efc1fc44afa0dd3d1f93d54463b8ddbb64f171c1acd6340d3823dff60b6421f788481c5291a9fe5348058860c22c563da69faffa5a" },
                { "sk", "82e758ad618e134e3a8d3996cd8e34c4bc252d8c78110960bd0bdb12d3e025212f5e39e7ebd49cc22f00569cef30d09e5c24904e97b17fea307c96156a0c2f77" },
                { "sl", "f4d59eb0b1662139088506f77af66c05097736f26b8dded01265c5e750ad9f33f249f4839e71bedd8da3aaac9a0076697f458d445e06ce8bba16d0fb186b0dcf" },
                { "son", "6a5f1a5888db1668b5ee2849f0f414fe07dd1514ac1845dea18032c045134239d46bbfde928dc8a327a26efe22444c3983451045873a0edcad0e4731d909ec60" },
                { "sq", "bcda2c7073ad6677c17380891f13bc4d57f77f21ae3b8790d06ed0a33c3b4204b0dad772db2d6f046465770829a167b4ff01c7a1a7e86c8aa8f0eef60682f1db" },
                { "sr", "1a68cc9c1ce7e98725158572fefa8709ceadc64b30e0698d21efefcdc4d8831a4730de1733f868c8e5d3d26f49c73b024d2e9168db3058e690402865ad802c20" },
                { "sv-SE", "4245b6bcf300a752ca293d13bf750730e999d15c12f77422fccd565fb31f5949e8f0ef7bfbf9cb321827a8217f7a2428e095639a41024c6ac024db7432f1872b" },
                { "szl", "55fba6ef7b5b5114b2e5b40fc409f6cdf394d1cf7ae33c8d941cfc782e033c51c646ff3d73c5cbdaa3ee3dc6361bca639342eb5a826fd297e445d29f98656eff" },
                { "ta", "4ec628cdba55ddc4b4c34644c73f869a66baaea708f78697e375a4ae6c77ac5535ad87d7410a09e2d0857e9847a6d16e915e59d4191f716beee90d45424d6150" },
                { "te", "a2b956840c50843846ef7c299803f47060ecc0e8be14c1f2db70d63bb2d153bdd6a5bf44e29bbcd1d178dfc432250d4e4f98522918383297cba329a3816404f4" },
                { "th", "7b70e33a0cb456dce40e424bc61618ac34fde88069253321f2c1da80a6f2a7ea08959ef2c11c9fdfde5bc055c6e9038490f480bf249db0988166bf6d0a715e44" },
                { "tl", "4584dd1aaf6f351925d326cb05f8367af393ddca8419b3199bb04c5981ead7473549c1c2cc5145201e8e00fa2a82739e9020bbc248f57c418011acc55f7b7ccf" },
                { "tr", "522cd1c21d4d201001385fdd39da19d00ff3cdc51799c553fb536c78d2530bb0e9c7024589099474fdf98710eb2b83da5391afd9a37cb018b3ffb0739942621b" },
                { "trs", "8cfaef78aa5f1afd389f9c30437cc99e5b23623dfcbf6c582498629bff56073274d3fe3d57858221753498f7ebc90210a022428b5042a879668e3c035f70b669" },
                { "uk", "f6961152a959f779ee448b43a6d1252186d75bc7ea65834fda74dc1298698b91f3c466703b756b8407c99744d2b6c2c86dd4b47208d489002204aada488965f3" },
                { "ur", "afdfbd049947bf25b1a11d3cd1a7b7494d81cef8f01bdad2268dca39692f8fc7affe881dd2112aa1a946f8ee83384044951289feb77bcd6a77890315169e8733" },
                { "uz", "f4ca97b160ca563c6cadde5a7e2d0d4b378a7419e05743896e4cbcf00978ed4a2adfa7c72635435ebcdaec23637f31922f1d7ceb298303e54ede6b873f91e4db" },
                { "vi", "2a0ed512775f083dd43393d3d716adf510db75891deb5c164adfd4d145af5be7816a36d89ddadba647560bb8921b8fdfbc7356feca64a2389130ec866a879b63" },
                { "xh", "f26c303914d165d16d93134509a288d903f8f82991077f43196171d4d41ade3559d8025a4b83eaaf53c08c503be2184b8f35244ed9d1d1f868ebc250d04b8b92" },
                { "zh-CN", "b1161115ef6fff6092797aaa7f34498c62a33c63088b7f2d161d6732fb394802b90cff049d33b904debd29eaae6ef4ac5b4acb9e344d5008da63f3bd0c041523" },
                { "zh-TW", "2af294b4ab8e1212200eb0f9f80ffd9d0f21cd6723b308d09824664533b87f94bd3c15eb74c7c47895c5682b9f85a23fb808990e0fb457622b60f39a98f1452d" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/93.0b1/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "8de1655315f2ac75f3f1e4d1af30cf510fa4d8cd087a4f5b992584a8afe7cc9129e128c83f28fc6c37467b236102f4fa16d7d019b9fcc0c5062c699e7993e914" },
                { "af", "0e3959d4b4f6e9a741aa107ff243b6738de953080279f0ebe87b75efc49e8f93aada7f762ec1886a79d0f87ca35ca35f8b2dec11581d528a96c7454b128983ae" },
                { "an", "2a834e148f158c7175e68d92e3ee3c85662988717fc8343010b07a38d9ae6e94d4de0a4e4ba6f2ee39cdabc15ad2d74ebc656cbad255ca07ad57d0bd6ae2e1a8" },
                { "ar", "82aebf401339053fd7e24d7a967a5012eed0efeda601e4b59ca1c0ece57fb5ccbf2c8c371c44555bb7aae9d2ed825abc95990e4e0b788ae8776eaa1c9d68cd38" },
                { "ast", "fc4d68ecf6c38907c72ac1829075dc79e2053e48244c5da3a38ae9da9453dbe3ef3cc6e7778cf1164cb8f96d179edcae67a40cb621c23a2598cb7577072dc4c2" },
                { "az", "0128f22cf69838be97706ac5ddfc3ddd4e2d444007ebe183534d433d73204e34d3cc1828c619d5a32134cfd1335d105a153713e9e9414bc725581ea227f65bb9" },
                { "be", "4edce18673de045a1b22cfcafe53092da5ee8a13706c190deba8f51ea16a2b0e884c278c5fb60eba1f3f5c04ecac2c29886d8205505dc529bbbd22461fdf4e8f" },
                { "bg", "ec699f3c26c718f77fdf66db901708508ff1a66d3ab2c0ca13d8c32b480b310672f4c21688fe249b9b460e9912a5239258381bf24f63c7eaca4424a8a78d2740" },
                { "bn", "e0f9b5ebb46f0b6764db5fa3391591cf04333d5da9ae1474b2f1c68cfabdf40b7281d9c2d9ee9c5cfb1058fe78edc32fa2382c7bd1f59bf7b8d463d4fe82b390" },
                { "br", "dbf5f0cbbb2faac83562d1d5f6e53d2a6300cecc66eb160c8c44b2722a543eabf657b79e9aa93610009a7fa4c9991847a78ec5c4b4c5701c479b1cfc8d6c3b33" },
                { "bs", "1a0a474f91f3016b0fe7ed699fc07c53476845028d73f6ee71263caf7fdc8b9c3456647e87f9b9c16f2f586d321c5c3bdb67af33f09c6261e9ff0387f222cf66" },
                { "ca", "44b20997af18799921635f6b6a35a92d5fc7a594c905da12c3fbe217bdc2217641ea6630e900f941aa1df7d4c95efd43f00ae75996edf72c195266f6b7182c6a" },
                { "cak", "5bcaa778741f7becbc5ed3839a2ba61bc1d94f7d53cfe2285d6b6f70227d9bdfc353864aa03b5c9f15bc93b906e38d7e58a8e54d6dbd127cac0aae79b4491d57" },
                { "cs", "c7103a68c51d1b7880b6752a5f935eda65b30a3b3e9dc6049f6be8b9f7d76c38c293a4f3eeced4ebed48490db33182aa807c4658fd471324001466b6d6ad2975" },
                { "cy", "dab0bf4a4a46111522b6040b949ca8abc50ff022b114b9825cfad64c72a654f9255061f92c06be664bc8c8132c97355db3683dc40f8a7be85ed1bb62cabe2f26" },
                { "da", "5b03b061be5f2cc3d772749263c2e9a6a81d7cd4d7dea2f30467e1e6a3103c6a55e625131e86514b6fd7f087fe72eee62c93c9fbca3be4e2fa552fff0793d838" },
                { "de", "ba0ca45249dcb45d1c6cf71f3a87760dc24bae5babac6510941fd7d188f7b37cc2b24f3978b36709cfd9643758cd8c7ef6e33b5682143517e108eeafc0172c8b" },
                { "dsb", "2b9ed5d5670b2317b7710b16c5eb93c4472718664389f0a7582ba80c64999301c0bfd25c7de6345f99665c20d5254fa45f4506b0a2032c65f1f3911e06088918" },
                { "el", "f2b4b2130b2931c92254062eb9ca179b94b04d8f540c4c5c88804e7208310fa999112ab9acfe6e2faaeb7d9415bbc7cc2da48fcb353b2d185041e726d8955798" },
                { "en-CA", "4658f3aae3bbc7b41c0b69eeb3ea42d279e28345356889c435a2211d08a061dc9e1cbdf29170add369e4f5b7c74b92c7c5f9d53f4326a48dbcd8d96e2d1e8e0c" },
                { "en-GB", "aab1d06fa661cddbb09d4dab90916604f5628de1937141cfa9bfe7481bc5ae43cf5fd8da49e64e7a3bf2bc011a866d50f91feca96dfd8178beacd948c8260a78" },
                { "en-US", "71d9c126bead940d2bfa20445e3dc01749855110e6114d543c466c8fb34a76185884ba69e418ec64590c742f186c84066a24bfb2d211ce81cf59411aa3a815e0" },
                { "eo", "eff2282e587ec42193a84e49d7b24bea8cae92d1dc25e98623c2e9dffc6248a4c5b54fd8f22a8de4f23bd9352b125878e1e7e8898fb01aa476c4672a78f67e5f" },
                { "es-AR", "4aba368d37092f5296b89dd400415b9e6adf7c7c6925ef7c82ce408217d3e937a700040001fc2da22072a8ee498fcc9a73891d3b6c94ef36eb5c14ca57014516" },
                { "es-CL", "ec5057a59dd39431fab025eb7196bf4038914786b15d790f9029f51edff9d5511241cb464cceab2b22c23e35c5b52db9d51cb3229521ab8aa3e397b06dc9ed9f" },
                { "es-ES", "86c12f49d268cbe15820ea96fa824e2791e47578c085baac513cffbb9fe4323c65e3adc9f2246261131e5728497d402b81fdf18ed2fb0da770119c1c9ba524a0" },
                { "es-MX", "6a80128fa28292459c80a8a0b07944b6581099c0f0cf65eae26bf38ccf42f357ea6c24994220068fddae1f6c22342d518a87e697e56d885b5998444adc45c7fb" },
                { "et", "8ae116000a9118bb12f7b43d9cbdedf3a4d9e3cd610495666517e0381b7f8e62de528bb7df145a6ae60a0b7ce8648f8393eb0fafff7a63ac2a0d18b16869c8a3" },
                { "eu", "172723f1b74d7eab51f6bf3ff905b8a0eeb1d9ff8d1b1c3baf8ec5e9cd015f3ed0a6f65adddfcd236622217b98072affe8abfd8c7cd88f812ec1114972ebe7d2" },
                { "fa", "1e60a689d7acfe389f809dceb62a2fd4f0c39a0ebe71c912066159ad0f51c22b2e88806af0f3cf52d9da6d3754a431f55d07875ac605bb0aa56fddece08d5641" },
                { "ff", "708baa354b341dd76fd6d326a7d80c5727e509ed6e555e9e9cbf24d4ec9a14efba3347e0b93ad7a7d6b9e29d6e68d9ffd010f465d2f221d3a7f1853c3409b7ae" },
                { "fi", "696d9f277323279ef8622dd0b5bb7ca877f3a1d7e0549930042c214706c6b1cb5b15d9a7aa575acd29662f86bcef212a25640440756f3f0d4a0104ccad123138" },
                { "fr", "98e24f5506d97b724f88af703893f0c2b6e0766d4f589a8cb11ef11ba853bdfc8dcaa28951dac8c5cd78a3ae8b0cb4986fa617b3830ec87e39dfd951c608bae7" },
                { "fy-NL", "0206e245aceff5f399e5db6e73cba2a6f024865df1caca0fbbc3cfe058317187b033b76c8ce6941a0d00fd87c2dbd6ce3d71852ef3c3ecbb0fcd25e69e8ab87c" },
                { "ga-IE", "3fb60aca90ff0777ac3f8c0de0b089419cdd3ea68788648250fd6681867bb2369ecb011ea9a745f9c4a17f7bf64fee987a7e3a9cb366ae0b78399e5c83ce0435" },
                { "gd", "9cc9a60e92848e3b619b6e97746beec7edcb887e8b0216a6838d6b0e05353f3c4130cc2ec13a0e1d5f9299983f6e3109448bd2828c8d30b4d0a62ffc4f533966" },
                { "gl", "4d0c21d3a29f409d51038a84b3990ccc6b8623ed6a38e3eb1364558e22e23eb4605ae3fd9c64799f20719391535803c2dfdf66b779b0083be6ef5fb70835a50a" },
                { "gn", "6828ce9492981118486caeba8bbb3e5881dacbc95600383a960ec22d1d813718e484e5c1759d2b36cb2044c16b4620fb3a3e33537a6cc1f1b1c704c0d7346471" },
                { "gu-IN", "323a8b1258c3d0967578798e06d5e14075616634b2ebd9ed609c612babdb69d07b3e211c9b13a4925a4220ed8935e9eb4f49b09953e798702238ec35c7dea85a" },
                { "he", "aac28c3704ce90a60c0b331b17ad2955095697d1d18d03ae1818d26efd243041745c27f8331e04f30c9fc7e4d350f8b9203cfae5cc6263fed95440a80642f12e" },
                { "hi-IN", "d094abac2699d0c96fded1d187ab922392c2761fa01be40f1886659e4bb71d0e2692ad2f6d5b25423a21096b73514509382424e02e5954013f44ce0417b7a49b" },
                { "hr", "e7afacc8600ba4deca69eb03443370deb1b3894f94186bd84534e9079c605d4fa7a9aad14f4e4c3123a32f0a803c141290503567031a020060eb58fcaf99b6b5" },
                { "hsb", "ca454143b88a3ee2d35969c04e5dece8c66292d4b396412fc753998a7c2663d7f0037d03d218e1cf0a6e3dfc6fb0441b446a6ab7318aa1e7ded02fc48382ce32" },
                { "hu", "f9eba6e79689c40caae06030346d626fa9f24bb57a22ab0f993f9dd28f59485fe62477f06ea5df5dac998e0f13da463338dec5003e592aed874583973f45a3e9" },
                { "hy-AM", "71d547afe02e125ebd099ff723a0c6e4c2cd2a5e81dc45411bd80c1025cf1ce7d7d07ffb416a2132ba0b33ff756e4f3bec3e214b5da8d7a0611b2b5c1c1c6c74" },
                { "ia", "face65cde8f1ee1333485b8f94383e15484a4cc97e5384868bd570434e0014cbd8e33a8ad007417b2adea98543f45fce23755dde22dc3e396575b32421da32ba" },
                { "id", "978784c8e5a9eb062e082afdbdc0e08dab19321e7efad4442d1499e2b83e5e3121c2fad20477c2647a9d3f97747086164cb9ec76b63dc006f357f2ae31e8902e" },
                { "is", "eee4a9cfe0e2f5eadfeb4bb1c4ff3df5c7d978c39d31a4ce5003f498b5e045c281af5c7ed57b10e9e9280b5c13613c56179c08f68e5dca2d9ed58f74e56c4337" },
                { "it", "34d88ee0d2eeb4da1d0373118e86754c471c7e09dd2ba8ba7b78aed83d0dba7ded428fa40051e23ac9342d3c0ed3d9e204f8f3114f6c0554a855f461cad55bd0" },
                { "ja", "0fc6ff4738ab93e8195834364ec8360d260db354128166f9e8fab88de576dd80e4fc0dc1526ecd83bef2ade3c4b6e6b96410c33dc988451ddbf2f28bffbd9a0e" },
                { "ka", "2e62da16469d316838065578ae2391d24d2dcf0e103c7a688519133d20b69cb7e6b6252a468711ff87b71b8ba9711658d5409355ef20d3824eccbb95bbc374b1" },
                { "kab", "3ee6073975ae3f5b7244004d38c188c2ecdb7f61ecde344ee556e205185eb2e3e6b4e0e103c193bb1ea6ddefbdaaeff27cbaae9fd0530ff770166a37dedac4cf" },
                { "kk", "67e69ee4e0609bdf029f5d9561aa2b5a21d5804d558aee8ed4abecaf55c13546a85a64929014abbd06057b4900a6fd15493bbf144157095dc7b552f9a245f1a1" },
                { "km", "27d74ee3410b93c57d38c39c20d117e443a71ac480e246a4bb543cb633de42b7fd357f5e6a882d631acb2e46cef994dfa74ceaf2510c60d361d8fd1c9c7ec17f" },
                { "kn", "808d6d1f527cfe51816b3667d5e4335220b697e82d8a5038fd783125e0dcde59959e0a9baf69caf895db355086afe621b98539435981b42648c8c49021ef3f33" },
                { "ko", "9398cc3c63d6fb54312dd85ced445b3ed6cb49b5cb3d40d29183eeac464d69fae4d82c6ea87caf05780285780b168e6e8770522efa4f9a97eb6d50e105a980fd" },
                { "lij", "3625d3053c93885db305a6a833fd08c1d8b0a1dd7bb337300044b14269d274320487c1de8f74f846b7510490ef7f1f3a825bb4aae3fb661bc3d8d2cec63f6c7b" },
                { "lt", "3d96f59b39ad5c8a3280f72c9f2149e7c761387108706921b97bf60d5d6409c17c53f50b770b69075001502485245f71701b179bb1e53def015af5777494e0f3" },
                { "lv", "700861d5519ab5ad98cd79581e0211af4b64a704dcc6bf5f2744314610281c206e082345526dcf6b7200b7f3f3ee0cdeb77d7663e8a593e79ba1fb785b8a288f" },
                { "mk", "dec64e50003cef3fe286d76581b05dcf27572668b42678a013c545aed1d2a52e424eca00759b0c8b298c2bac15a336430d354a8da4ad71fc11720236ff62ff77" },
                { "mr", "6a69a7daf4e6f78e75a14accded54385149ef0ba3bf61bfea13dbf1d0758b9466f432b00312f7a409fb6b30e0cdef1025f916b967b9c49c7fecfb98970eee1e8" },
                { "ms", "2d7afe463d186763d81cf31baba0b01a67b69b3848e2423888c792e37b68bce8195208b21ae16ffffd069c45e76433d7b83c8916a2f03702df08e43f84f1ef24" },
                { "my", "a6e239d1042dd198de477ee6cff36323b3e5abe2a238dbb4a445b31124db1d8063753f042a2e78d8cfe23a514d6f562d207ac701e231aa4c7fbc038ed282ad3e" },
                { "nb-NO", "8ca77117122bc305ec4f9991f7bf12db51bead06b8079a5de26a92597b8c04902f3917d23033d13c89b36ad1afccea03f9f661c800d933661628843d767e5cd8" },
                { "ne-NP", "16ea7a04cc3dcffd3ad3bacb0741051574fb1772ccf803fa74c12ca85e1446a933d357b05c17794516d8c7a4604dcd90283d36c9f950031080fb70af30d1883e" },
                { "nl", "c5a2a89a5badba5cf37aa8e1159680daa362dcec06078bb0b0ebbec73769f54cb3b7f65931c40afea315c91b972ac2118f1ecc24bbaeef193053538b9e345efa" },
                { "nn-NO", "45b7cc0652ac43c87a0b4af4e42cda6f11b4ec5db5bb0eae2faae83d418662d859e85238eae48d7c1fb0ce030d1dafdff1a7bdb0003af7fa302b428c39606cef" },
                { "oc", "74579f3ab46c1f2fe33d2c165c8c350a69bfb61b4114a147e9fa7b98fcacb8e3a6eb6e0fb6ae31f3b7b1708c914ec347c9b4083b4bbdd82ffd08406ad9f28242" },
                { "pa-IN", "e6b20cb0830ef54c188186b2fe19b9eae40f848ba1b44b87816bf7fba423680e5036d0fcaf423f553c4f5d604c7877bb5edf72f222c6716fe9f9da88105a4806" },
                { "pl", "2220c511a8365ea61ce83aa8790a8b0efe51c7d937772e3f32a2132a40157dd6cdc1793927cb6df2ea06324d9515d8333878e33bce2081f1d507b69ba0798082" },
                { "pt-BR", "4dfc601d3c5da9c5198edbb83c493d5a3d639b8b193fc78d0b78b137457cb9609294d1980b1b14a116a5d1cc1e8f591654235c5925e708cb2fefb6711432b84c" },
                { "pt-PT", "83cc7ac96712a2a21b743d202ab9529c5201f2fd24db00286c947d618fdfb51b6f26fa054a14a14fc5daaecaeb1c3dae77dd1998d64e45f63768079c0abe4e8a" },
                { "rm", "80481d23a8366f76a425af6e08bcfc152457646e5b033f6c21b10d393e717c9093face2355dfd3530c3de7a6f2738fcb7e81bd07e7a82c119e0d2641b47fc998" },
                { "ro", "b4400ad89d84021f98f13e4b3a834a8fec87c5a70e07751a2583ea07887f4efbd0bf5609783898ef74ee5b8595bdeb70a1ce955cfd206931a396905c521c540a" },
                { "ru", "b95b25ee4443c41ae81b2055384892ab6227a56fbef6e249734b9604e693deee57ea69338b5e0c5b369808829a06174bdefc385c46df7f2e85d06fb9893e16dc" },
                { "sco", "e5c1fd1fc20ee09ff7fc7ad2a148819bccfc92c6f006b7b9936b17e63ecae772ec084199d4c7541980857889b8b0a99f1b21e7d294b1050390741afdb9589459" },
                { "si", "4aad0b67279258b6bb60e5e77e4aa95660091a0653e43bcc5c992a8df3ec834129f545c781a69c652f8aef6031743c364480f914c1cb11bd1b2d0cc271662bbb" },
                { "sk", "3c791c40fce34cda269ee87b1c095281d230021d7c447cbbaacfcd943d4404b807d505141ae0fa5e254038978e992b993a1e096f73ebd69cf1525c5d08b85a9c" },
                { "sl", "be2c8c912faa1e0c52634cf6882b00680e42c50cfc7e7841fbe81f749683c66e9695eb5ed99487e072d3207a2b5dd00e33036b04bc4211703fe8e22552ddf1ef" },
                { "son", "8dd5801f0615f429eef167e427e43ea062f07d720eccb2a47ad2c7e6593ecbdd55ace6ef3543a04c0b9517f8268d358ca0a7af39b9901c2f6cdfd72607ff1424" },
                { "sq", "dfd7e73ae28f7dbc73ad997c9ca7e113d7cf8626687de5f41a1d0420b49ae85903120a5bcbf4e1e81b6bc18fe5aa52d217e5df7d841e74a8a061978deb5dc0cf" },
                { "sr", "cf65ff76c3474b2ba142a0c39b1ff65383f948fa98cd59a633365ab090c83b06aef11a938f494a749ed508d545b318bf36b3ce247861ac374f1906ef682c5fad" },
                { "sv-SE", "7360d019177e44d219aff2587bf34ae093c13a59573393ed6ad6daaa9245315df227b3f771fd3e14ab79558357ddb5e04af4248d276e30a8c9c70cf0f7d3ccfa" },
                { "szl", "79be0b35fec584623d0553fb1076f583395ef04f7a5dfce03a3a9f2fea6ffc06fad17b0ce12e69d601d5dfce1dde0c134d23fc057d5fbf5bf3f29fcce200e34d" },
                { "ta", "fd39e47e98b6c4e63e3bdc2900818ad6fbd6dffc8fe270b32156b2aed8824fc51d431ad43c535ed0c81b3eb088b48945eaffa2300f5588116883461b1d5d975c" },
                { "te", "4287874a6bf56cf3a4f47fa15ba5d5472646cd636812021b57c25fe6d770efafd054e863d6b5c93436253c6e0dfd3e08ccfaf5c438c7bc034a763b885d60e423" },
                { "th", "2d9df1cf0ece7c376ee27700ce67538c391522fe7a3d17f4c1b9ec9c8cf26ed76a0754ed846d98efe9079090d3298bcc615ae085a0197cee68b234cd6ec45603" },
                { "tl", "a9b1e77e8a0dcd85d97dcbc728f53dfc552ac4a3cee9bcec60f1afbaca19ac88cf865dbf1c4de0eb65096f00fdf2189d20e255686e3e74da3148c085dcb69ac6" },
                { "tr", "b262d2ecf40cb009aa5e9107da5f9a4dae8cb235fb2057bed5d88cdc6bce8eeb2cdf58cb75ce6f8856b5a9afaf4b47f5d85d200ffa4db78c896afd250fece839" },
                { "trs", "c8dd59c77b2eb454bfcd338565f70f08be8c714b58b17dd230c918fa66126e97b6f622621e51fe1be8f0ddf9514039a34913a08532c74e093211f5178f9656ee" },
                { "uk", "7dd7d44dc75b5bced5d38b00d87bffd11147b181975397c9c8a974778e92f2fe8d5b055389768769c7f6f4c8094f0bb945300e6a5ad98d82dc569bd690e9f8e7" },
                { "ur", "6bc49f837e4d7afb24494673955270817a677caffa9d9c51bf63dd3575e9ec258fff21603b878e68f7dfa6e35560c61a0934ca80335770ca6e6ad899d45e74e7" },
                { "uz", "4ade79f86c232915b1a604eef1295d79ae2aa75217248bb51fa7ab375df0b3c37b3723dd6db0749315618737e4c266b106345b14729032abc8fd6099e41994b7" },
                { "vi", "4743ce9f26548d7eb4e55cd253530d5af7d98d9ec9d861e145635b5557fd33bc6140255b30bafcc8ee16769b2b1374cbbfb5f781ab6892ffc5a40e73ba1c65bc" },
                { "xh", "82049b94b539951e14e2fa71f2583f561ca7380cb92c94cf437ea120acff1b4d810c113885f70c94f3547f09867517dab1837b8f825435e782984576089763b1" },
                { "zh-CN", "dbf3b19d143e4e1200ca30dc9f75f6a12e0ad877dc6588bd7407c9edf502032f2e71200d0a085de849b7ceb7b81b9fa6c89452a5cbd3fcfc971567505e62ebc1" },
                { "zh-TW", "84e9d699a212a7107896a924bbd111e1ac720974e7d753ee19c14712db53f49fa2a4df177f4999f62f8350c4f635f5ab3584d7ad6e01f016a77ea76c8452acc1" }
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

            string htmlContent = null;
            using (var client = new WebClient())
            {
                try
                {
                    htmlContent = client.DownloadString(url);
                }
                catch (Exception ex)
                {
                    logger.Warn("Error while looking for newer Firefox Developer Edition version: " + ex.Message);
                    return null;
                }
                client.Dispose();
            } // using

            // HTML source contains something like "<a href="/pub/devedition/releases/54.0b11/">54.0b11/</a>"
            // for every version. We just collect them all and look for the newest version.
            List<QuartetAurora> versions = new List<QuartetAurora>();
            Regex regEx = new Regex("<a href=\"/pub/devedition/releases/([0-9]+\\.[0-9]+[a-z][0-9]+)/\">([0-9]+\\.[0-9]+[a-z][0-9]+)/</a>");
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
            string sha512SumsContent = null;
            if (!string.IsNullOrWhiteSpace(checksumsText) && (newerVersion == currentVersion))
            {
                // Use text from earlier request.
                sha512SumsContent = checksumsText;
            }
            else
            {
                // Get file content from Mozilla server.
                string url = "https://ftp.mozilla.org/pub/devedition/releases/" + newerVersion + "/SHA512SUMS";
                using (var client = new WebClient())
                {
                    try
                    {
                        sha512SumsContent = client.DownloadString(url);
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
                    client.Dispose();
                } // using
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
                Regex reChecksum = new Regex("[0-9a-f]{128}  win" + bits + "/" + languageCode.Replace("-", "\\-")
                    + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
                Match matchChecksum = reChecksum.Match(sha512SumsContent);
                if (!matchChecksum.Success)
                    return null;
                // checksum is the first 128 characters of the match
                sums.Add(matchChecksum.Value.Substring(0, 128));
            } // foreach
            // return list as array
            return sums.ToArray();
        }


        /// <summary>
        /// Takes the plain text from the checksum file (if already present) and extracts checksums from that file into a dictionary.
        /// </summary>
        private void fillChecksumDictionaries()
        {
            if (!string.IsNullOrWhiteSpace(checksumsText))
            {
                if ((null == cs32) || (cs32.Count == 0))
                {
                    // look for lines with language code and version for 32 bit
                    Regex reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs32 = new SortedDictionary<string, string>();
                    MatchCollection matches = reChecksum32Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value.Substring(136).Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs32.Add(language, matches[i].Value.Substring(0, 128));
                    }
                }

                if ((null == cs64) || (cs64.Count == 0))
                {
                    // look for line with the correct language code and version for 64 bit
                    Regex reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs64 = new SortedDictionary<string, string>();
                    MatchCollection matches = reChecksum64Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value.Substring(136).Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs64.Add(language, matches[i].Value.Substring(0, 128));
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
