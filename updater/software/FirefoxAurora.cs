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
        private const string currentVersion = "135.0b1";


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
            // https://ftp.mozilla.org/pub/devedition/releases/135.0b1/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "9f6bdf5d98d20212babfa07ba16f5f569d6011d82b7db31d06d56c2a9d3e8df14aa3fe4b9a2aa9d9c68063d42c99417aea0dfe8a0687fd8aedc6d499a31547bf" },
                { "af", "dd64492efa61f475b35ae74d066ec60683010c1ed3de84dc2ded77bf8c285f337b930c435f1ddf2fbbb0649c370314c97eb485690ff6be3fb97173197351a4ac" },
                { "an", "497bd2d2ecb49c856cade0bf6104f5bb0e20493ced6526017b1ca2879edb6aa8be9a3acd02da135909f62a1592c0ba4c20e486894b586bf50acdb82bd7f7093e" },
                { "ar", "9fd9f9d01d1b0b8e4da1f2c2c1326d157bf0e712db8bf0b9b37a241205f9973047381e963b8b4eeaf8107d8969dcb44e8c7c1ea9184ddcaa9a2e43844b30aba2" },
                { "ast", "b82310f49f0884fa5267fb30778355a2a7da0b92b1fb1c16f67c743e4c17b6c6309b0bc3ea6493ce90f4817d1b46b3d5cbe55a7d010aac825d006ca29459157c" },
                { "az", "b182603213997ad7627db702529df054b57dd92361b46966ce1675b25a139acc8e2b9f12240cbfb9d272912574a365e972193bffa7dc488155a3c6e6747af4cc" },
                { "be", "d0e7ede1951b9ed0adabe5d264fc5ac9bf9e646f105088a6cf6fc53aff3e82aa752003c182bb69406ba6f76158a70cd3e10bf39fddb498de6dc2375943fd641e" },
                { "bg", "8181feaaef5eba8d6ac6b6879d53f65b27556a2c635e70069d4241fa640fbf2b36c72e7681e135b149bf52bee50ae58456a83883454028df8978ef44129decd7" },
                { "bn", "23813c3e7f50d088bb299c218a11836df991f08c71b91f232d6a5bd7be9f41a980bbd2f7944120c6645f8d95cc69d2744641b9f48287b7ae0cdb8a544f85d9e8" },
                { "br", "6417cc05b42ac29f2830fe042b1964676d5722a9bf4da6baf600ff5df0461f3e7c0869664c072aadb2c077b7ee8f8b3af5b4379f9128009f794d985f8aa4a466" },
                { "bs", "2876651ba7fe45ecd316f41182e486c7316f2db5ecd04a024c2af0168eb52a6718e2cdbc457ee259379ac7d33181ce33bc327344ac5dc655ea058ad0a8ef9168" },
                { "ca", "56b69867851f121eb9dfbf2c875f4d7e921811d06e04873b123a9d7a00a9d8ec241c43bc8e3e1d281129b7488768f311622e79d3c3839a00729ca994439a9d67" },
                { "cak", "d0497c46fe8844436c62593fa5362b6f032475289e27c3bb04fddfcc2f3845b7f21a681fb5e2396962d67f98479f012cde87b135cf78161ef9f2c2edf63a92d1" },
                { "cs", "faf3d693d1e09ed47eb08ad6b0b36be16fb152dc47d5a2de54f10b070bdf1c71941dd8502aaeda3d84e40ea83621d55b1348f5df4ea95e6f15af79d34bb76d5b" },
                { "cy", "f26c1831760c1079f802c82e903849db3a2fb1b0c4aec1a65b550b1a315fa51d5d4fb26de799e844bacf2b43fe412119f01602dfc5ab07894e689740c6fa6bf3" },
                { "da", "f0888b7efa270d808ae23f27943c9c3a9f231aefb244516bce4e813d5901af4a5bf04c41fff855fc700603ca5816cdc1d35d51b7b32321b400926ef3b1b1c470" },
                { "de", "b9f4db3530c51103fdfa74d30c23141ddd05339e76a803145fb049664716a2a3e3e5fdc378312210a20e6cd7b8c68dce1a84fef29c5b1a48f2ea41510c10a4b0" },
                { "dsb", "fd0a657ad511e3f789e208527cd5c893a3addd3cbfabed9bb7e9978500e72368eaf94d03c05c493745c73fe14e0a863d405380061ed09ba2f256eab17cc0ef0a" },
                { "el", "fa5faa1db1689fb5af6022320a4d2c8a5997c28af927128405848c0d88932e3c66e5670f2c2a2f78a2416d1d06bb74aa9dbba580b94506dfc86e9f7042145a96" },
                { "en-CA", "374300f2ae9a7761eae7400e735171e5a5e57f44ae2c780768465067c815a2000acb477fa6d0e2ca2623290d26aebd8a3ccf1d7677779e8c4bf60d15d4b942fa" },
                { "en-GB", "14ef1817607d2a7fe4fe6cfaf229d5d2992dc92f18e5b941e9e36da3bab007ff9020fbfc406869fdf1910f30fe55cefcd9027e6ccf37ca5e358b29cb621be048" },
                { "en-US", "ed6becd9de94e429ee7b0c476c37c6347d93ba9f2572ec044268c8866109ce226aa4e97c0133187583d06d1b94e26c30b2dd9aaa238e0597ca263f44a9172cf2" },
                { "eo", "2f1ef765570ba919a6a2f8683acfc78bc6ef660f856af504c9a241f0af85b0d32ed97c48a26aa250c5892f5926458ae9f5a0e1ca4ca9f956bc81d2507e013903" },
                { "es-AR", "474b25af5ad12ad1e42781bd33219a375be44ab724bec09761621eb829f519f345c57389078307dc644a58e666c9483d8c10eed1c7e519badcd1e9c182c1f257" },
                { "es-CL", "1efdd2e13438cbb5e8c14e07bf16a9c2de34b4de365f4f8480b8a50b8fccb036bfb59acc2a2ae8be37e893c41872cb02958acdaf20faeff097e14caa036a0e9f" },
                { "es-ES", "6fa19f616167164b89f42abec0aadeb8969064e62e0f321d48cdd86cceb26f45267945cfad65aef11205f20a7956d7cf8303903da4f4977f2e0f98a9f382d113" },
                { "es-MX", "74cc37204f2a24f0eeb922f730eb40b4f6db16545b82f9746ed77b0ca88d1f685ad7d7b32ca4615144a98e33cec2a60e7ff43c5ec1af5c82019fc107d507d25c" },
                { "et", "1196c9f4bf08b9f378186ee2242bd85240ac65b99b822d8eb4bdc86002d6404ab73ad1b4e347e8a61bad3f43bbe6dcf095305ccf582737c207514bfe96b3e031" },
                { "eu", "a8823f547726d00d2a048d49b8cadb7013153cfefece7ea952bdc5ac378484978db8fea1429654dd51fb779779b2fbd673328016b6d2adc555d22a23b2861f7c" },
                { "fa", "4160b1ce6be57e6678bd7cd49bd3aa9be7579fe838587bc0a5d18916220a8c2156a11ee9b57a352b0ce9b0e9b26c7f42b19bc5dc7749b73514a9fd0d82ec8346" },
                { "ff", "d21fe10933b813322698adb0aecf35d1a1c0bd6ba01fcbb9228f4f534eddd12cf37bafc2c34f5cca69d0ae4e509ed579be0517a70e4a633ed39c4cdff7532402" },
                { "fi", "31f25347448041a83c43f1e5192b8e6d5a232544dc26e21d1124a88a6dead2790c3941f8e1420f0c605725c44adc393a19561976da209346491e066c2e96888e" },
                { "fr", "faf58ffd489999d35fdee30fbf1bb6434503fd456aefaebbfe217a1558f4b8bb610c5438af80312e5e0f73c637af4b8345232ed829021dc4977d9e04cdcac563" },
                { "fur", "ba2e1d2278bce921c1fb39ae2e0c14bb3bbd152b7d0872ded0a79b0b5e0c5256815af1281f675011a53a503e21d0fe2612bb82cdb49f8bdcd7f0ad80d125d756" },
                { "fy-NL", "fc1ceb5f59c4e7a952513341054de67f021501f8e1816b6c123b6ff6a6567459470b5681d1f52bbda85ee2383734403fc6baffdd6aac97273d622399027ee4f1" },
                { "ga-IE", "d69b9bf6193628c0a53bd39aeed35dac05f49dfb15a134a850753e708bf1ca289ba988ac7fcc530627bc789c7f9e3321ca6bca7f85384987f6ec392acdc3df9e" },
                { "gd", "eff410401606ca6f40b5f4a4153d6ba67864fcf12ec1c8ec1677e746982ea40571b479a82a0925eab4d29aced6492bd534490c1914e32189c857cd1fa893598e" },
                { "gl", "f5f550266705752704e781ffd7a8faf83f8ae1de56c24e53681dddad7b2554df19e341ff76c736f298059b8833f45a5a04ebf581f2e706302d9a50e00807ae86" },
                { "gn", "661305da65da31d947eb53abf18efbff4845fac980558b05b6fabcb9171ddf0f6b1c0efbec68b61ec63ea70358d26262885c8a91e0d1bc5b7ab7313c48dd81e2" },
                { "gu-IN", "6a8c445e81c4950b36b4bc0f17b45a53d5a1cd2aa4947896875d43e17c81e13dfd843b9b66acc517a661ababbedff9d1665035312b0607086a96fa34afc71063" },
                { "he", "bb0640f9d12dafe433ce6a147194ae8958bcafbbad2ace1870b3fac6aa261045cd99af2d63a843ac7a288c1e8930de2bd986f77ac2aea34df7c3787b7a86a7db" },
                { "hi-IN", "1b9edc9cff40fee1e1cf325615c84eb87fd11f3b6affd0d8075aa4466331ad339ff2e7a50b7ef588c7a4d81689421baa029dbbd5c3f5b7c7e39dfeac7d7b8759" },
                { "hr", "6f99ae3847afe99367f0198c9ca5a8cb55cf09b111e2eec4fd91a120a337174df8fb47c4b916778f272acda9195a74fc9b9a54c740d2f31c070bc8c26bdd6235" },
                { "hsb", "b4569570140a69b4e4161119c553ee9a2868538f4cf2fff55d3fbda6ef3c2b4fae274df3c900144dcbd7e973b8d84771fa9894a024a1186c541610824cfb92a5" },
                { "hu", "094250f1e11c010234fb521c9afdcdc6f178543155f99bf2db3ba99b80465bc9bfac1f5c9670341231dc997823a5a65004f49eaf592b45c356cad83c8eeee175" },
                { "hy-AM", "2b1422da140057ef8cd1fb8b7c01fadb7fc80d27cc601394ad97d12c4bf324a99694a2d6695dd641ee0d6fde4824f83bc6a23f1d97289bfc640090645ee416fe" },
                { "ia", "4d61259940a171da8c4377e94b3b960d3d743bcc81e5083d1bf7b17b15499f203e5d19a4c5a6531fee07c8700eaa420356a9b436dcb2212bd1104dc754df180d" },
                { "id", "cb654c7eefe505256073ba06e92b83bb676265c6419060fd8f3c7c0e983cf0726e6dcd40e575cdfca10fa306880fc8d3c063c7b6c20a92158264a6907899ac1c" },
                { "is", "ca1b8fab93621d924447f1c3dc3d61fdabcd5c4370ff8db05e801db3c69cb7611c2e61b7dc1579004ca7ceb63f221b06334f2bfd72580b0644bf279a685e2978" },
                { "it", "59867758cba603801143f0b38fb95b3dd23a250789f8b16684d2d022df2293b9f5c0df30e309d6eb692cb115bb0af25c50b4b56eeaa06cfb15c1ec590fd9ac01" },
                { "ja", "bf484604bd3395fd74e064a27675e6251cca67ddaf299d5679f436d6e90c78d649085b2c5eb4583bd60de306273952b147fa14d209653701d5438237d9758212" },
                { "ka", "aec30dcc3921f087109aa4caef6156d12e9a4cb339509e1e7987ac8761a78fb96ec97bb38cba9380999c21e4a3940d8c0944210aac9b79b147daa7105ed0e893" },
                { "kab", "f7f6b4b47635a2bddf3a80cbd31aed0e8b56e89f49ccff4cb1773844dd0439df17820669c9a3bfacb991b40985a868537e3bf9f22722f4b4750ee442f3b73efe" },
                { "kk", "e9ca50294fa8b89e0d91a8ba9f6521567c239ecbc2bef2a90e1b3eb5e807729343df9e138cacab7d1fe449dd2555256431328621f1a58bfa94b1f6d08d7f1952" },
                { "km", "3af1460b093b4be23563650fd2bc5e9ef789d8ee0647c67458be6964c7360b003f519b5cccc8b28fa576e740da1fdf5a742617bf9f273a9dd34b94252aa966c5" },
                { "kn", "25c013f117544fc4d5f037b9460a3cb1cb44004b5dd4132acdb21169af0d909067dd5118423398836d9c9e76e2ea1eeabe358769ac7737ec00aa9f30a3898f5f" },
                { "ko", "30b853a29e8eaf671b93ce46d90e6812f07ee2976b7817f7f251cf9363be94ea330d1a48527b2bde6fde6fd89a60159bc5008ad17ea02266eea8c85bab422693" },
                { "lij", "02b3000537122e395934d20266ce5ab09daa3f25265c3656e6239c54d65835a6424dd72363d69e70e06ae032d289180914c1a582a4063522fa67dff6b4984380" },
                { "lt", "f326889ada84ea49af669d2af966549e01709293defce15ecde0a27832b66790a5ae05aac1063d6ff8667ef4a80f63fd98f71f91c30cce6d7b11cbd91edec676" },
                { "lv", "2efdc5d2e5357101b60f4c543964484615c0c2b6a1b09abcff77a64806771916becf285025a611a66fa5d7a2474db4de92be8c614aeb2af2dded87d13ecc8ef5" },
                { "mk", "9459e0f546a87a6835bcbd0a9b4f16c3ecf7e4729ef01378a29b0b2204a0e4a4534e3bd448e71311111b40023da3c8f777dda4910a268b11cb6cd6b0a56edd22" },
                { "mr", "b656797e96c040ce1e09e21661540bf43fd057e8e5c2975f29c0d56c47a8cbf47bf9a77b9756915ff23754c8a6053ed785bd8e9e7b27bb432c37f7a8fd17cd25" },
                { "ms", "cbd6ca5d30d455b881bdc097e507955871f613e6cf6dd99b51559dc46cee8416c9838b924047599fb9ede56772af6f35f6063b8201457c64191d34055e5d53fe" },
                { "my", "d2f1badc961252b782bae3131a6a921d7521659d114a5539c62a2ab19ec2d8841e668908d7de138064ceb5373196ab941cd8958e8ee783d7dc645293cb5d7330" },
                { "nb-NO", "b93e9879fae1f0e0e02b0f86191c947fe174c448d2129123e646ede409e814a07ef7a8bc68467a218cf833bcc8bc408b0feed3130f69e8efd577cb870b0e4ff5" },
                { "ne-NP", "23331207128d353a4661a6dcda4434558c207c790836a603f278634c3d0ea8596806a80f5c21459b6de403835bea1a6f8614088fe8299995467cfdfe7bca2362" },
                { "nl", "8c8499232a15ad6bb3436b7018681a5dfc32d2c94e86f74214d6257812be72463ece81f3028f4deb73fb1d1a3224b4952b0f5d3f44b0e490113159025f235b91" },
                { "nn-NO", "ce5949604e0c3e4c31daceb8d299e35f6821629531a8aca09f85f5b4da493481b406de033f7571f3dc2e7b960a3ac6db85567c5cd1dd71fb17eed04e0eef8089" },
                { "oc", "9c8cfb1350284cbd0387fa42b7044be3d4a19207ea90374a2d72dd28377dd6d75d977ae76e013882a2ef19395c8e0579f9412cf5dcb749d125cb3884f51f4d4f" },
                { "pa-IN", "e8e125f94890f69b3912c3fe471d48b3650bc3e5c537e67eddc689d102bbb89fc23dcce8606c83ac6407d2bbe3fc312732f29d53e3d7f181e9461cc3f55f0900" },
                { "pl", "450fcd3126f12a3071a83633bb7aa5d129ed30b55d2915770bb5444c21e54a9594c47a7bc7df549cf8ef56a352dbbab78054f391bbff76e7f335fee11a572009" },
                { "pt-BR", "50188ae1a628c7f454dc696b087e7aacc1344bff89040de499abe8830378dd8004c9c5cba9467b8e3e2f56639fce9fa2364db75e1163de8dc9719e02ddbebdf6" },
                { "pt-PT", "ff8d47dc1eb137ee8deb675eefeac4b238d55824ac5d6c65fc2cc94c08c075d2ef87ed0457278c7b5c2b2709dc00800a21981df9146c20fc8d52860d38356f94" },
                { "rm", "191cfc934b2c089c5eee2bd4ea67b197ed9dfa2d064d29f849bfe4a5816f11e40cbb975356596d63e0c2fea0b1c5e2fac2facf1f43c74d74db4d6cda6ab83cd9" },
                { "ro", "cc587a781ca0a09f47b1d19343ae677256c1316d8b9e5bf64358bde22e614db40b69b0e0befab4793a3db40c25f9946f1dced712e6957daf75c584bc24f90fc7" },
                { "ru", "c12e83451757694a4ae5910a36a35d129c94569311de86a186384cadd4a38d4c299674c0aaadae0e268325a2771542738da8ee6b7d33e4b3e44cfb02f032be48" },
                { "sat", "e856414f0a380cb0a1a1d43e4bc8309f09ed89b444fd3d12d259f723339cddba1bc16d3a7ab4e54444231cce983ab2f65efa715a951ca31277d1821d800285b0" },
                { "sc", "44c7df6571419272da8f4d1d41ccb8794cca8d4b897a50753e36b4a01318e7982d367d7dbd299be8fc50677a178febaff058a897e93000e21eefe00078e92acb" },
                { "sco", "2e8f7a77ba8cc99cedb961ac7e0c484f471c9035c58cf80981b3c464abc8817a698d74825770455816284c745743c06cd1ba72c6c01bde16f5f9b46e7e87f504" },
                { "si", "73badc6820fd270b507ded3c08a164a61ac06e4460cf43ecf830a4677d4c5400730f617ddda827a52bfe9c1ae38e74df854a1d73c64d3e714555ccbdf7f02d77" },
                { "sk", "199da19acf6c3ef33b1ef0f43f0598630cc900f61c4a9dc4d0906102153cd2f641c97e7396d2e678b03307baa50d5981fd3f142d81d7eec27ba3d667afe58182" },
                { "skr", "c7f6862997dabbee266c54587732752668bba27cb934a8f2ef9918b908803eabba0c0552f17825975f5d32f3f271d92490ec36ba8d5e0a5bb66f31a9d611a629" },
                { "sl", "b812d966567139a10068a35898547470d1d71211cf1fedc13adbe8bf277ee917a7e8ddb0436f01a75891761f3ebd0e7adea8d715a931cfa45f7e6bd882470d2f" },
                { "son", "68a8b2f3287d4d06c38234921e7ff090808d08ab1f1c4f099d1d54b555bfb2196349a50da2997f7b239bc980fb2eeec486de0044df931ddf73a85190dbc5312c" },
                { "sq", "971c3a14271ce7f89e472dc17c60bdd1011bfdc0214f71d6797535800b9f16c75d3d2562c014fbe3ecebdd6c0b9b41995e48a4f626824e269c1bd8a83cefaa5a" },
                { "sr", "7a1e772563716b70341fd7b2257ee84ccacd1f6f2a193bd06845842e1690560aa1ab1134fcbaa15e0f931f3458d06dcb2c8c34eb9324ace127511d67a03aa9d7" },
                { "sv-SE", "fbd3571a43342ee3cb6e0565ea114e76dba482a8c83637ad4bd29f0b04a47139ed3db4c485e5f6b6b413b84125c69488feb856b3209ca6bad74e59adbec4cd8c" },
                { "szl", "c1d853a39d2127e6b84dea94403cf3cf95bddc0c43e53027530cd05f8a842a92852b847dfdec55d1e4f2811c84a83def4e8a3a33a93d38feda2d5169c0792f34" },
                { "ta", "3856af6c14cf664a5f2ffbdff82e201ad8ce2d8110395131268176d3d0d75523b37146a3a6be71d1950f82f5c6f15211c6fc81a9c7be2c75d01a11172eb5de5c" },
                { "te", "f5aee8d441966f41185272b5d0bb52a36660fa43ad5bbe00bbe70deaa00e36f0a3a59536c0a2440956267f98d37f80ab25ec587b94a076beef5be56bfd08e5ac" },
                { "tg", "23a29b234084ced803ffaf5b1e19da1630c45918f112ff4fa4471f9a3d506de2d1246c1f97a2e826450ff977c20ffa2ca9845385cdf690409e612970194e442c" },
                { "th", "dfc9f8810df86413f2ba5968f51192f84a3fd63588317bf75713b050c805e1add52d07fde79354f9ce5166ab339e7c4ad7bd451b6ed26bfacb4b29482c234e11" },
                { "tl", "8c5425f52ab8c5effd2f54f7de9cff37c79fce6d3a91c6210eff031d122db64e5edbe614c510295949cd5494221dd668ee670166baae7f0de23322f478980b85" },
                { "tr", "1fcb792813226cf29f950f4562ce8c3d59b32ee59da59b31f5af82abad537c270ca5e80ab2ed6f84ed3fb2e27b8c0cea26fe8503c65a48f645a449584b8faef6" },
                { "trs", "8c063e0d9e6ec0cba0f08b34ca4fdb6a7bb0bcfb4133fe175230ce8d1fbe49668c659b896abd9dbd491b70f8816acf74e1c6c1286d55aeca947d36c9b15612aa" },
                { "uk", "362d1c4071fca4a149e0b4e5910422103bcb2328ced69c7fda4892e5adcb6baeb90d1288ae936034d8fcf259cbd7fe555e5f5373e5e79a3d858d1a6ad748cdfd" },
                { "ur", "9d3650e701e7b5cd33ec90c4f28e6646cf0d0657d37791953839059bd4115d3c5ecdd7f038c6cbe16d766288a7016a24f90c9b5b19eb685b7ed66488ebba714c" },
                { "uz", "bffa5f2a081b59ed5501373f71145f82c2088f3c5af349efa6ea97a5703063b418350c3bac8c0df50d380657eb33a69f5af497806cec141cf208ee3380e27ecc" },
                { "vi", "43f03fbb13bd816190559580e11fbfc0d432a9ac792bd939fc6687b0184828d7937760ec2bafd6d00d3ed124b355471ccfd1be19498ffc5db264c348c909df85" },
                { "xh", "9758760a3ccda722033e5a63605d65eda43c0efac0c1e8c513b39cdfd675575f2f44692371797629a470bb109b5d55fb4703f151120b1194551734a1729f95f2" },
                { "zh-CN", "5542e0c84f29f2332fa2edecd4548c5e7296c41ca7d1c628c4e587eeb6e7da0cb188dd69164dabd563f17ba9fe4da29b6af1e55a850a361d6e9f8ba36bd318b1" },
                { "zh-TW", "1a436bb861bb8505517da0ef2f772b605a576cd2f99e4da89fd8cab81159b30781e5b320a1eac76509b301b2d719cd09acc95ea69189684335e75675cf5c60d0" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64-bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/135.0b1/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "81e37989133c229c3f7c4308ec7d6455762e3459c4ece991033e1f499ada3f5868348fbb5dcd1a403e928003efb01bf65bb0dcc1b7d93a207d3e66c1afa50285" },
                { "af", "d23658b13f8720f5c91e05499722ef3fb397971ae0e33ecd98cd0c514fe3ee4b635be79c970f03e1646052753f9719f4f124c588e08800388f8579f2f5404dac" },
                { "an", "3a43f3f3e8b3450de02556b1cc274447ebd4bde3339e3b0ebc935aa911e7b2d3c1e00bd961d56c5008089b157d04411ba57aa7bad3da9051f1b3c01c40888a33" },
                { "ar", "49e97025e49f7287347d2544be1fa469879439d94ba7e12564196cb241a2be60ff57d27aa9566589e0a514e7652414b1cc4368807997a523ebda3ec7825fd228" },
                { "ast", "858f7d4650540e5134a932dcf053d2a6ea115f38a7b9add84a4a00bedd7495c5c5b9a86e7917b0398b592cf48f9029162f3787e6594f178a5b76c23d3d98dbfa" },
                { "az", "8aa7d6bd2ad4e78b59675f1176fb3dc704d76ff346a7bd98a20523a73133850b90662a2049f5841a41bb76fc5cb39294aed7098c12c99c8258ddbdd0791e6348" },
                { "be", "b556407842a7c286e304232640fd499a62d8f3c6a26edf72dafcc8c4a62e2e053b97df37439285b4014966413d44acb2546ec05f772f8503bf86c8aa03479acf" },
                { "bg", "5c09f32b735e29f701075a73bf06adf4d1a901f97e26fabcfdb21cda2af44ca04473f17d1bf687930e04b09c97542f64dc0467dbf377a0257a93976a8675814c" },
                { "bn", "f94d86e5a5f43bd11d59e03e988d221ba1eb87b4327bc751faf7f5fd6d8ff4f1fc0c8999106221752b8b8a0bf7c573c406917e5a5286b602a0cf4750051f1c1d" },
                { "br", "f10fd0a95c7b1536f0baff0c256569e06f2d4684427420a1a7a410e0e4c542345ea6b656faf038763ce3d957172c15573d3a395739dc71207695016505f1ad4f" },
                { "bs", "a397f98f3a7df2ecd52d55285ff192db73eeeb81f432e81d71031852f45d4b62ad69bda96a807b52d1df7066d274c23041ebd09b601ca71ac8361de3361b2413" },
                { "ca", "6a46310cd6256db4b6e231a45304426ed550a964ffa9c006f08b179f7c346db4045a8b77631e8f8dedb4ab0b2a26a7bac85017e923f0c46e6e73db0ae1efdf68" },
                { "cak", "1c03268574f2cbd942fb161c16e3eb35cc1f6ef26abdfcdbe78a330587d72eba79d396023b73a92f99364a9f55a3d59a1523b0798497d6955442b0ac2a96a131" },
                { "cs", "4171a9ab2be8b67ec694566d6a81538327dc30c918563a16b7777a4b290b61fb9f578a5ee94212488b8c241cf48f73843de8103e0b796b91ffa57518cdac7e91" },
                { "cy", "1e2e458975e21662d5a444bb00aef5db48f9f18888815a9ff1fc4d69c3aafae275edba7ba357887d9a72aeb16b1ece7a47ebd94ff33a4745a729046f52d5f8ed" },
                { "da", "26ee064b27ad131d9f162595ed42102ae4a48f97e4c915f945f15d21a5142270002b6f204f0eb5acbf76e37478f9ebbaebc0764ba36590a54d41a1374dd40b30" },
                { "de", "f139ac32a26438528173a49514e4e5514d2413eed208031ee4f1083e8397804069b775cba1d6faa0da423ac0943424f2f7b998bee7cedec2cc46cf500140b14f" },
                { "dsb", "0707b49ecf1ea66ae3ec577c25266849c627d5459aa72a5c287b18a03a75818e2e76f708c6a78de31e5039c5079625ca3c3c71b2e544c0c6f68902641a1387a8" },
                { "el", "1a8e90660a1b52dc179090aaf4357cd5415450f9c2410bfc6b115159ad0babb784c9adaad0be0ebb4f8605c66a79008ffd5f619a6db0db8e496330469d77092d" },
                { "en-CA", "8644746bbf7b56368ec269463bd9f2f1d03ba5aeed5e570d75c2246806e06b77bda08a8499074d4bc58b5192edfb2a22544fb929895d764284e5a2a59df47373" },
                { "en-GB", "8f5b84eff443ecef7c13320a355dce555e7fc0fac4e90296948761e1628e59f602e701ea42dcb06c8145661c68210715759411420839e4e1c93f4911af690b36" },
                { "en-US", "0fa58c359258a7d21c3011fd07dcb29c96d66e5d948c2f3e132693f44f1bb2442ae1f6cb32299dc88aa0d8e294343dfe1f3b50ed997dbb84526da920bbb5348d" },
                { "eo", "5e5db708d80d3fcbeee5bd6414f73f19d4da6e735b92e5828fab3638cf6be1becc9df015ad29e2e0cb5a223af630edac526359a0358524003c01d8b76d7c1f2f" },
                { "es-AR", "432dffd9538c47b710bd15695bfb1a4c45df355b19dec3975571d1c50de34b0c47a2f628e399784cb3e521326edc1dd8c77b0079cae2de16b86239d6b273ca9c" },
                { "es-CL", "97ed576c93da814d8a239e35602922ea1dc02c8f88ab0583dab71cdcd299a1cadd66821f0ba68a1a3149616e26542f7fa9f5dfb1b56b82b903197fac20428d6e" },
                { "es-ES", "17d8e2d6b9014fc239a16c468f8a5c123523f8237910b3f51c4591375c03a64a8cdc81227bc38d7a7d9a83e266ad572a88b454cd02a3712c5c00030691402d5d" },
                { "es-MX", "0b94fd6ac5348a570f6f46c6d501aada829243dd21186d94dc58e5d454ebc59c4dccc14b7cb6621ec52cd1e6feb04e9beeb4bae29113e27a3f71bd08112b941f" },
                { "et", "aac7242b0a2bdc87ba232c74d1f118fc054439d809ffffad1937c79af559914168b3b47008b10e4d46a3038244da8d9c8894f964156889d022731c9673e98a89" },
                { "eu", "16df92a46f9bd5cfe28b17997e8adaaa6752cf20d2494d6d1b6623629425cc12dd2f6f1ad92b0a1677c4988b90a75bcee5f9cce223d21d58fcb77d5d0e0cb986" },
                { "fa", "c50ee10c3470180afe6610a64e19f29d981705cbccd1701c56c721772cb83e749413cedcc93e5e9309350647a05440a1691922456b58a819c1debf9adfa245ee" },
                { "ff", "1acca396896f856b0014ffe2cc1822d17e7e50f80f8efa0a252bdcb17eaac316ef51cba0835e990f048ed8526087baa0ba765cdda5910a3cbdac57e4d604bf4f" },
                { "fi", "fec8a6600d3c25f169ba4bc38593feedf758e8ebfd8108c9bd080e11686f58025d3605759f7069d24f08572cdbba5e06052e15722772f8f45cac6ca477d6f434" },
                { "fr", "7a9ba96468c14801e03a0e0260516a1c79ae72b8eb0d7908ec1b0a70cb2e2b990b9642aaeb1f931dba7a14d2e139a661dbb45f5dfdd376ce7fca138d566041b1" },
                { "fur", "7adef9c551b8d7bbeb570736da4a88fac1bf9579cbdbc1621323b98874ba54a0c345c5dc349748e9f8e6eb549fe9644e5b005d11668d60f999f1f8e2f9eeca2b" },
                { "fy-NL", "ce83e09d35ffa676ba8f393bbce38794a87083a80e283b4c5a6913d3e988d734d561b31b19dfe651a50614f0b3cfeadc37a8493ffb75164de819db3fd755d639" },
                { "ga-IE", "0827570bf0551fdcd7e43291f077869c0935180f6fc5b105a3bc52e46185953537153b253c06ae4203574017fa2813180bd4471551acd06a603074fda432dfe0" },
                { "gd", "6e8615a3b8fbb0941b7e245d5d1919b1f113728bf79e37efdbbe6051db30dddf60525d3136d32fbc54ff1b63094315f540b42459e9eb60a98ab1ac7c27e60f22" },
                { "gl", "31159abe54d6d5d0066ba520ff66f8bb92fbbf75e715dd43741adc68658febcb0c270310e0f8a1f01b125d03a0f7291f74ec35630475b9c42d260747d4c97d03" },
                { "gn", "2426cb28a378740e39096146fc05c4a12ea113e26e7b06f5961c6cdc7848bbb8fd9fdba0199455c092922c8b23e730adf9c560d23cf2ac151bfc30725ad3fef5" },
                { "gu-IN", "e8d48dfb4d5816c96ca51dda9413def1da54a949f009a4c392d5a146b8aa96dbe1a311ed691369c7812decc1173dc767e044a18441be400af8cd61d7b80ea35b" },
                { "he", "509217197a78680eecaf93d0831152eecaf71d1d4a7a555b6660afda4959680f5a91ace02321efc88e09165840e6b2f3f63686e61fad9f58f3e81331b47d1bfc" },
                { "hi-IN", "a88d059eb62339f029097e22bbe8e78ceb61c5c5374ff0284cb7c0e856b38ce976a83a1dbda3132524a8d63759977c5fbb1035c9ca3d4def06ec5aacc6903a41" },
                { "hr", "89b61ed9d6599de98f8b6594b10af9774528893ef8b0c64fa8a1c1f9835c1585910d74d66df9adc666a96e78f5a8704945a4c1b68c0a72698cda0f39f9933f47" },
                { "hsb", "eb7ad3ae073a078555cbf8231495e1feec3cd6775913090a3afc72f1bcf7b8732de9fdc35f2b82062b89e92073d12d92ef0cc052411336f0fab823885f3c8b39" },
                { "hu", "5be6d7bf8e81764961db723817f8c348560cfceb934b193dbcc14843eb1fd6d82dfc10ab7f9b430748e7735f1fa26142fe18306a8cf80f29cebc5f25ef742aa1" },
                { "hy-AM", "562901c3685ae5082049d3f114bded43b7c4c12b8c865d24a785ab41bdae451c15f683799a45919736d7e444ccf552fe9d9d979e1b2bc6e73c2d135e8fbdd80e" },
                { "ia", "fdd0d6ed9ee60b58c6aceb67fa3a4ac412023d8cd78935270d48ad3fe68f359948c6b8b756bd3a7290e7391506dcd226855a07cb3c54db61e24ed945265e6975" },
                { "id", "ac62110ab440600ba4347110b138912bd8a632f6bad740738d87d7190da8e522d893b7c3f3f2544a93f02539641fb76ccfa40f53f8ffe9feb1c72f0d085ec68c" },
                { "is", "efd97f30336652381a0bf41b5c27efcfe67bb7d6c4db61d2007f174ca190d7a177525efba0de3148a019b06e6ac7a1675372cc572e0730205ae373ef1fb2e884" },
                { "it", "4ce89103e282ba65e127e7ea31edc86e9f3897410137ec7e871a8ce5afaa6e4d8a61bc34b4a2826e4a22b9364394084af27635ee166c0a5ceceda97d23eb29d0" },
                { "ja", "a17a2d8fb33a333661fbca673724c097f4c2e657c166a5bd742986d835d706100d329238324339080f64f679e702f3d5d40c29b30b98e5156d609df8662f413b" },
                { "ka", "ef78cc56edbefbd27037fa005101c486f2173dce088667f186fdefaef2e500fa63e24722258ef906ca2f5e9cb51cdf570dda88cbc300c708dcd16ebb1f823c53" },
                { "kab", "85d7350faffcc5053e45484741f303edce6066173b64b28801d4706d389bf8b933cd3b5f81c2c36db18edebf18079ee664c72430ec3ad815d622ad1b10832e62" },
                { "kk", "6511a820b7495ecc3412042d86d4e974dedb3bb1f0616f80335f585d6a65621d60c7312e36981d865d18744ef060badaae43337f71373e6dfdb2b096d75915cf" },
                { "km", "1b16ee049638f73f324efe62854cad84b56251325006488396cdfc0a4d9dcbab72b8051e358b40eb9a33c06164c162bda7ff46fee7bcc54cf888886ced7772f2" },
                { "kn", "f79febd0224ac7c1304977c8b980ef1e14a522dd5097c442510c61a764aa68b730c52ccd46bd54e7315af5b77e45baa31c2244d7833239835321654e7916d9c0" },
                { "ko", "fcf1610c6bb70cdfea95b47f2e56a1b4f22bcd40d9c4e01d1d6c7de905fa8740940c0da85c57f84f10ec73460bdd573b99e92af27dcef402b6ba32d077be4056" },
                { "lij", "c49fd16cea782f27bae4eac870d28595bbd7dc052e6123dc9b91302ef2dfabc533c87c5994f2dc7b55c41faf03a639ed509afba28771bf3dbf56e4261ead0067" },
                { "lt", "6cc48f799f65add0876f6f55e9bd2c9e795f5eb8bd8c0c3ac54b461ec3cee59d94ae1937f92e3f0e2c6035f1011e88d8b8477e91135841712825429c17cc0048" },
                { "lv", "bb88d046d9296d672a593f13f13e9cfc072d8d4b057ce1a92bfaf9f4b0fdca8cd9c4aaa0d879bdecbfeb429f115485deef9277ae9334bcf29bbc92c40ea140e2" },
                { "mk", "a25754aa72ad8034f2356e69315f743de215757f84091ad8f557149b76b99740feaa83730294622ca3090f378806db8586f0e5375bebc7b6c2a09f92bd548cbc" },
                { "mr", "ce563fe78af04e17f77db6d9fc45c650caea1b7adb745d73168108ff29af08bc1015385ff92f38ef0d20b5757fb90daebdea5fa860021f0ad1419701170b2c21" },
                { "ms", "2dcca98af987ed7d44ede05edc11d4c20a7672a5add9e4ff570a004f968441664652d569d4f03eb4c24a52d16212c93893a360e16e8071504b252587fbf372f9" },
                { "my", "70839fc2076f01136f0f44acc4bce7ae568fc453808fb448ca6dec73186c8d4514781048bdffa8b63be74c229a3a2a28d358625c7ffaf82ff1a7d25bcfa587ba" },
                { "nb-NO", "d6f6ad958b75cf920a5369e2772f752ab85c7af19cbc35bcf9decbb9e1289e4487abdab71c5f3ab6dd21a4503420e9d42854e28b93b0320bca350b0d1718321a" },
                { "ne-NP", "19a7483ab7bb7099dda0f4a23fb374eb4e5cee0191f3ad35a0089616db1bafd2cb63c52932f34f0a3f9985ade32ff598042faf87a687690bcee56e73349823df" },
                { "nl", "2ee05becfe2e240aa58bd016ed1c063c18390c3010c0b6efd00f7ab2af0ae427116116d3878a19b75390023731a422e72f5e65aa4222f6c25199151788a69f74" },
                { "nn-NO", "df1aa3e410bee6cca6b1a41cd53456cf756eec208a4b08d6c5b458d0d6fbf9270fc70871bf50833c71c5293e7c8426b602e18c624389a88a2e8b06525d3c2296" },
                { "oc", "b3011caa1f65c5a2379ad5c096b5823c3f13183f31f9544b0c05c1b2afd75256737d53d8c53d80e405b52c81df87220e52b14f09f8c5c0c39589bcd4e22b43c4" },
                { "pa-IN", "3d25084debc645c22c86b96cfdac78c382d16b0262e8fb6cd1f60c1776567539c3dce00c2a88b0f3681ba0833012bfece2829171e96af8f0188ddb09f4ab866c" },
                { "pl", "262453657b097d201737a9775ac94ffe630e53dd5fab566be17da8a048f0d14a9bbc1d6e224b7d8299a2aebe5f60863a0ea006ab25a242afba9ba703924fe2ad" },
                { "pt-BR", "adaf415e74f158cac735f7657acde45f2741f153069b3df7538caf703dc440766d0aa5dba9513cb47df0e51cd5eab8e01df817db334279359fa9c6ecae4c246f" },
                { "pt-PT", "d3dc2fb0df652460a5d4a04a9c0f1ba3195297eafacccd60cb1fdd4819cf798f41a1318448410e35d3e86c3b1c03ad02a8b911cee04c335c43098cdcaccd741b" },
                { "rm", "f434bcb4208980dcaace4bff8f73530478460ce39201e3229def77472b3d86856f7e85a58329f9ee13e46302bc81826c86e6c1bd60e0bd38463719e371f62dcb" },
                { "ro", "35d76ce8bd25f5b63c5e30f90e34ed919ae67779534140e48e8b702615795d377de44c0c4d12962e42eddc1f850e669de8c2d0d22203df49af78414a2b2e2393" },
                { "ru", "2863e82e69240f10d57d45bd8561d476608b885498f68f856924c7b8dbc4a6c99f1681385c1c8b48d903bb38066039d79251329e67292779635fe76942850c5a" },
                { "sat", "1eb74fbd9a30c22b8d86adb7052144f4aa114d3e83a90555d0cf516bbb54c0ccc6842cffceada3ab59a3e2a0d929b653e2de5835bb7cbfe2286fa10f6539c26f" },
                { "sc", "293eb4c4b505373f38cc8481e50257e4e84795fa3b25998156528313225d500fdc39ac2b85d0c208fc8d51a62a4817ddd536fa5d79a1e1cb4948a815913a787e" },
                { "sco", "3be74711acca412201e5516f4655eff6fb12c11911118379a24bd594fd5b47798d39a99d7e7fabdacde52c36039d5d9f6c8eb51172c550c3c1def6a77792c4e7" },
                { "si", "e57583d250076c131df178f174fa21af94ced979d5c160800c682060cc6d5f0e393f8c8e1b2876772ebc7f91a4240f6cf579aba5a5972be7bd9abb23491931b0" },
                { "sk", "d7faa421601052aa655058df79e5df5393ca80022b75ff4c40a794c96c21333a6dd7846ebbb099d5f6f3e937eb0d4b0445971841da961a1cffb5dae1fbb97f00" },
                { "skr", "37479c2302edd173b68cb62b2401fbc4f088773589b6c5cea99fc3b253a6be4a599737dd655ccb346b3aea2a99a8e2f68b8383c43e3c14df19ae4cc5799a6496" },
                { "sl", "a344df4121fd2b3c7fe64d1483c40368f0a7a21cdd7e15d887928f1e298964ba8baef09030b67d4de864ae042a60fcc9b53f77369ee1956b8290663d65caf876" },
                { "son", "a917e55b4bc17124a41a1053b0e59cc44d6ffa431b36582645b8e81c66a62883f5700301877f19ea7856a7401bc9e92a8de415a778a8966b1837f4363ba08dcd" },
                { "sq", "e2f101d2df2697843383b6a07a44b67f2e18b5265b3f4a43e3e2f820bc25e3305723ca722ad946797259c3d22b289b3325bca423fc8e6ddb3bb2237161c42caa" },
                { "sr", "f3b8c376d02bb5fe084e269d19e87a6fbda30ba50463a11e0e53807c4142764b0f31be253df02201a1923bab329292f7d329fab19f9d5976bacb00106753c0b8" },
                { "sv-SE", "a7c947a36b7b91dc7bf5aaf89ee8db4eaf49af9a297a57c2cc21c030e1bce4050926aec4b6c80b7fb1fd1482fd3f6ead64f446ae95231dddabaef2b1c7b69075" },
                { "szl", "b69a4585620684b3065482c88a01ffa2d0ff5e178704bd479be8b285fada1128e58f4dcbcfc3dbd33d63948b52362d64ef7e2dbe63d95537d3d79806335b586f" },
                { "ta", "35be9550f5baf4f61a14a607f003506d362892a8997bf51b9b5e0bef3d5ddf8b2525a5c60de2d35e59206f06018dba0b5407af21e70f4c56414838737a12ed8e" },
                { "te", "63aabee2e26cc910a077c256e7db9e1187caf4ef1ff7a944c66ee355ef19f98a3f1a9e785eb33eba48659ee7f9914a4f5791af9937317739eb2e2bec1ee2d8a3" },
                { "tg", "2f0a3b6d1d973abb4b50ffbc73292a1a401e74327bdbdb5cf86e82ab3f3279f76349003e0c195d5f306003e77988831f00938fd2819f5a70df1a9c39f86a1cb0" },
                { "th", "8ccaebf7ca5785a3694350b16726287d59c0fd60c31fad5c0514177b9bb1258e4d81484d01e4796dadaa34f0513fd6312c6e7c0303200414468fe1eee822226a" },
                { "tl", "e9e02523b8871f149edc4c93fd6958a88291a396ce24ebab87973a8774b39fb13c0bafcf955ffd09786e845d3c0298aaf500ff0d5fccc2802b395f1990c167d2" },
                { "tr", "b75075737cfa9558ddf574a4b517f21a496a79182bcc959fc3e42c6d7af58116ee56a89c883a01ae537144657a377ca5932d551828acfd9be3dfb40bb0da3677" },
                { "trs", "2e8c2de78246ae4a41ea3d02dcc2cfe4d37c25fb747c805dd5d29c69c3458642b1172b69ab423370698cbaea00a14baea0273f8da117ec9fb17b77b75fa5ad88" },
                { "uk", "e5ec8230fe586a6d3d39ff987ac60df66acf029f29d0eb2e968a2622dd40b7218d312af5ec1a6874f7b7f58c4df8eb7d1fbe4d7ae60692cc5cd867f7e867c568" },
                { "ur", "c281365b625e8970b4d4f6d17507f56853ce5757442843646de16b3750e64d9b2aa7b30ce9e557592c879efa74b310fdc0670adf6328256002b83dc532e77e10" },
                { "uz", "6e170f8f4cf73e41cf51c28d6627ded0f77d930201b964add68aaed695a6d578ecfff75af26eeae8e64fc813373ce11b13c3da975cb92f753384c565685676d0" },
                { "vi", "f50877db6412123a23a495bbdd408debe981fa81612a0a520bc66d9452f58f2aca30a1ce9692627338ad946a913e3f4bbb93db53af83d9ad0ca7fba1ec3485fe" },
                { "xh", "f757ef86ace1f767fbaac7fbdf33436cfbfc7eddc9f1f51b4ddeceeb4c24317bcb9309f6b4f138e83179a6efe0c98114da3bb17f9f7b93aa057f0eacbb191043" },
                { "zh-CN", "1ca3ff89ecdf728e1c111455e6b18b048ad2e4d02272d2a80e4ba237157855989e6c93acc414be41050e0da395d4525cf6507f24240816155b1e31514e164034" },
                { "zh-TW", "36c13dbe6e78715eddc8cc1a6d7b029d0b23a9a41e7cfdb0b6f34b53ff4236763dcaa69380be434eeaf157ce802b81e36056469c82516616503a1d32cc5fa3c3" }
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
