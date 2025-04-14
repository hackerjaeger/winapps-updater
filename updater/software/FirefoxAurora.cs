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
        private const string currentVersion = "138.0b7";


        /// <summary>
        /// constructor with language code
        /// </summary>
        /// <param name="langCode">the language code for the Firefox Developer Edition software,
        /// e.g. "de" for German, "en-GB" for British English, "fr" for French, etc.</param>
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
            // https://ftp.mozilla.org/pub/devedition/releases/138.0b7/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "56df5e690f22f0bf6907dabb0e02c221cdacace0f4f832179d1748585d6a2338347ea4bc3d4d1b4c0ec79e5f7b8816a2628fe0ec4e748e64167ee3ed060aeb79" },
                { "af", "593442da3500c7580237d7df1e4765b92c200e96b007a5a4dfb6d94ceba8bda20b28d4d1c0c363c473d2b41cbc6c0afe42a6c0ab596033c847dd614170bed52b" },
                { "an", "2487a7da46e4abbb406e4e4e67574cfb4b26456eb92568b7e438b1dc59a5e8e23787494dd729984b8583d76ccbfa941f42589e84cb53b84c34a8e99cd5d6c22d" },
                { "ar", "8dfbc5a835e36ca5993751a91608c273f44fec73da872524be1ac17b4912940a211260a18560bdf85951dfdfad0a21d4b83f829145af0d525d89ddf78a98edf2" },
                { "ast", "75fb0cc3aba006eef2f9584ca753c06be44ee77c6d64f04eee454fb228ab94d913c4a4bb3883259bef9ddd4609fdb7acc5c22e5b6a668d80ac8dce4efd71c6a6" },
                { "az", "45634e4db6c46993c0c02327a35e67154d4daaebb39050a4e4aa20a7b24de4033f72e579bd8e2f7fc302f8ffa99c7c353a2f07301aff45672d4f48571378b695" },
                { "be", "5e5f98b9c97da2367f083ebf35e7cebfca4aa4694e86b80458589f44f845124845b9f621d69bb91d1c8b616ef5d6919a3645c0bf32da7bc89bbf733aa65fb4fc" },
                { "bg", "8198d4836ca0403ed1e9b22e6567c98d6c1537391fe734aa70fc4b0596f70678cbd9cb5411d662dc07e89a660563bde9c0badfa0eff62fa732170f39b47679ed" },
                { "bn", "f3e42acc16e3e07b1fafd773a44b3d206568663f61002953254bc81c6f05cdb19bb57f5b9e6e1f842eb5edffed9a54b1152d41cbba06eadcb1f01d40d47c12f0" },
                { "br", "689331b3a45ff4687d1ee59a2b3c36d9295ad44e25924ff4b2c0d4e672143afcc1bb185493fd01c0128383ac6dcfa89bf720594973a88bd2e91c6f9c9b4b27f5" },
                { "bs", "a24f60fc2e0d7bc14ea6dda71ffd47a1046a07b8434b5490b659f7a2ae6e0b78dd9207ab5834005d4b400e2151b89f009d5061d52505a2a8644a8e1c58c4a7e7" },
                { "ca", "588dfaffcd0adc5e3457922897ea9b9876bf7fef0aa986f38a71c6b2ae7e3b8cd10c4e2777c2bf48e1b548bb0cd82db92fd90b56f7ff05c1e1c501f0ef46f99c" },
                { "cak", "6e563603f88f85fc4b93aab3b339eb781007117469516997992d6093ee46792c59c31bc991a7b2ef9e4129f89782a734d6107e2e27aeaa909e942dd6a1eebf26" },
                { "cs", "83653647555258ad5ce7e0d58298ff3f707a57ff3fd939014b7bb3d0eb495273e6cbd51d8946cadd6f3e44c3f78aa0b9b2e88ede1576c361747820cb1770857a" },
                { "cy", "cd66b193388406fc979788bc58bf1858f08ba80e81847e2a07c5c1f95fe9ff240a05083662f5516c8d79870c59e3315b46eb4efc7607d011c80251596efab72b" },
                { "da", "c71f14a6a589b6a35a286a9db6394abd142391e7f9ff0ce6202961cbac723a4ad84513901c4b08d9463f1ce9397c63c44b13097668fbb925b7f25559de81f712" },
                { "de", "b8e524969684f975017e2cfece64bc55bdf886dbf5d191c69ae886c492afa4448e0430f12d3a4f38b097d4652ea25d0a56556e5d4420452ff5f3a6898551eeaa" },
                { "dsb", "32c5f53100b8f8430e7fe2835b5383da3ac0dc56ae02593fcf1dfb9ff7f0cec11d8d626f87659ae02b483ec2b760e281633508139ead25fd61871dc8eae357c2" },
                { "el", "ca48fceba4dedfcda27390b97bbd404b929fd237bce0023b2589acc7326baca1308043419bc954cee1d87c8317c6a1290b0d066e0d1c2e5e2405f6afa294e4cd" },
                { "en-CA", "8141d537ec960a10492f2c65f6a0f15f7f4bde1b98c8d697e0587745b6ea20031c02dc613ce536399f41b6a47ac31323f9c7cede4339351ff80cbfee931597a5" },
                { "en-GB", "b5861ab13fb3d28de312365654ebd685e47748686f7f22def46b9fd056e741af7e743c098b332ea95ea1a1d770d481f7d14b66d5b2e4f1917b304cb06afd9ef1" },
                { "en-US", "bd8b6b305a1e4d7938a6913b7ba5ce001752d6bd659ddf0b373eb626bcd583e29dc8d8fbbcd97aeb9bfaf34e254250248224f153614829b5eebf5f7d446e8fb4" },
                { "eo", "a370ce53145796e0dfbc04b440ad11dcdf9bcf65dd1808fb95a0b87c9b8062eebaa29b127f58349ac662dbcce9e859c89fef42b48fcc2757158f154bb382641a" },
                { "es-AR", "0cd8ad36dc38eab610cdd34523b77c113737e787cecb61fa8a524ef9d567b953a08ae15f63341c2231c7a37d29aee6ffb29d88dc340870e332222b6ee8f59510" },
                { "es-CL", "e7a4c8d6c52c1754e0f1831ee34145dc3fec0053c1e6a87266d81ea5970fbd8d0d80d7c679ce42cb536399c9b7fb94adb5a774a5aa789366e38b3a8e6e822ad8" },
                { "es-ES", "68d0652afe313a57c60da203635492d4bcb1be7550bef2b052996feb38a56babd6c1b0f4ee108ddb0ec87d9936913afbb1139ec85c7bed836307eb73f04433f7" },
                { "es-MX", "1d20ec108dd36222b78c07e5182c0db68a0fdcc2cce92150945deefef1a268f7a05d9f065b1f552d44a95ad6e7b617c93a2e16610ffb36cfce03235058eae35b" },
                { "et", "065cbcbe115b281ffd39f11930458e830733a5e738a01aadd8b291909b90fa6a6060f7b90944c7b571a657923a602f6fd163663b6316bc58fc9fb5f6279f0430" },
                { "eu", "511c9d8df9472f5cc682006d5b756fa6f04555ed59eb2f7ab4edb8a94cce7d4b510e72895e0cf396d15c35f63227bee0b6ec39baaa0aa42bc2b1eb7ea4a8dc6f" },
                { "fa", "434d64aa84828d65d816b20d1d5ac8d30daa0ac01a96bc329affb5055b113ea80abb4965230985b2bbe7b722a6af9c777135a3279bbd97d7d096a6abd1e18ae1" },
                { "ff", "676c82b0b5fea3e6975995ee3161a7ad864a5ec8b9dca5e13f86fb8b6d760b2511fd73a31a9b14b890f39d05eedef60a8313012d12c50dd23344681a859bd4d4" },
                { "fi", "021fed1c22b2369a6a144f4a4a2f4eb9dd715dabd3076766f731e10d50a9f05b9ce5b39379e5135dfbd81f14c078d1e1e4d06b56474a9b2fc70cf0f25d9b814f" },
                { "fr", "96a45903715d5a96e5efd656dbb7e10c5ce68eb25a464c0e293f015519d7888525356272712491e1d6c5a0a21abdbb6a735a1dc0eab82c6bc1859c215faed9e4" },
                { "fur", "f68f2875014dfef6ba4c2617f9fedf34f702e9736d65e0229b6448dced58c577a6e17200be1f996d6e8a361ae8e7db37ee3587a172b5cf934d9672bbb3363b98" },
                { "fy-NL", "87627fefd39224d9e54c739700be71e951d2b89762500101a692b9dbc0f7b3562cd484a8ceb077cee8b4718c09f7bf80c0dd35505458a6f6ca223664cb01aafe" },
                { "ga-IE", "7fb6fd34491b9ee0efe1b73a8c9480762580d9169154c81a0ada43e4afbcfb888396e67e5de7c19ceb8f5bd433ffa29875a136a425e4536b8ac38eae32ed39c3" },
                { "gd", "ae41bf1931b79c938f8302148d02802ff310bd95ec31462b27e01683bfa2ca2c4693e4193a55c7e4e1f3a263a05660a94ab30997ac1961bb430f83ffff11f37c" },
                { "gl", "7446d50da6646bdf61f62d107c88b50f79d2a20d0d7c35e878281f427b81ef4530c19468a5b4b5e96cad89e1d7925aeb53a53d5476de9a41ae0810c4b4cc40a3" },
                { "gn", "7c247ec75e19e01cf366c847e83446b1c28e9e542f8dcc4308c94dd1b2dbac73efd830bf943e32c29ee398636974a7d87e5a21f341f7754bc03378d942f14ef2" },
                { "gu-IN", "1f58681e2639ee6a775a46ed01a92f93a23da0e06fe4384da06ba8edf95c9442166586bbac75d3886dfeb93b7ec28a3d057f1581a87c61bc3c54a37a71ed5996" },
                { "he", "3b3d9a95c6001a2d7a1acf4783925967baf86046d27d99605779e2ca5975f360442265b4624fb4b5d8b7e1c1f0b7f9c54788887ef40261a21c26e7048b273133" },
                { "hi-IN", "98da2db5a5ffa584826d6a9b1c99b6c6bc5f81dd978428cca9fc1168b6c1785bea52c28d165169ef8333c5371e146275c275d624146acc1faedf59d1712b9032" },
                { "hr", "39724ab2a762bd39cc3587e53a16b21394fe19d523cb0d3e8d35b9e9c0ffec2405d7c38f46821ba326f36ace5067100ce2d8b5ab49434e80728fdde85cd7582e" },
                { "hsb", "9bb59d6bd045a4088253dc2d8ac66d7b457ab66339b4a2c0ea2008b6dc7bd543842f459e79dd8b7fab8712e607c126703e21c99e5e765422317a3b2152c713ed" },
                { "hu", "3ba6b552cf131113c2c153e65fbc4a184cd946db71d203d346a158d6a5fc06f8d236e5cfd530c42d1600702dbb4c3a189aaee6f9530836190fb7a5fb38748bdb" },
                { "hy-AM", "061c930129ea579cfd4bd6e5c0ef357dc6bf471c0cb8bceb1525c85ef8d16862692aee945bca8b7953c47236bbe2291a3aab212fe91313f8a279d1fe52c2d535" },
                { "ia", "220d1f0524d796bbdb8440295ba6f5c082846d83866914b29dedca4318646580d12d2ec871aa93d6141ce3a4dc53a00c87f5d69a1af4c8157a58ac9f9640583a" },
                { "id", "92442fb3288870b45db7b45e477c0e27aaba4abcbfb48eeb258efe256850c7826848681329ba248246bb7f01f25bc1f3ca056fabce013d498317e29c71e56ea1" },
                { "is", "4026a4220948da85fe1fc54eb749689a6938ddcfb31a56993715a747a3713e108f4f2a93c0793d006254781be6584fa7f23803396ca73527154a0e026c8fc98d" },
                { "it", "1feccc6c8210b3e293410413902ab998f2175bdb09e58367f295d68a7e17463f99fd0fdfa9fe33c7863f6e82499b65097810d76c6fa16619e78a90e736856ba2" },
                { "ja", "b679c96313621a0d5a59dec1eca92990d17f2de5e0c178658f76430da7b117f459dfee8318968e46858e1201bce8126bbccab2444ebb4e102a88d97631341c2d" },
                { "ka", "c270bce7419892f447ae23e6c0cde600569d65dcc7885a29614c2d206bc2f97bf17307382a1bda32b476ba4b33b9233e02859bdd4722ccf984abfdaafb49aced" },
                { "kab", "1078f9cddb4c963191c8883d09fe3e6f86bd470070aad35efb012926b3a7066476ddb59a415ba8a15d8acfe3d9e9e829bc02d4d5bded6bda530c0ab403fe12a0" },
                { "kk", "55471ef62225664fde65ab8ccdac43f89abe9c313ac72033979115e029193871a8e727f0e3dfd3760b2e9b893191dc19d33d11d9d4b0c7f8f0c453972e7be688" },
                { "km", "3985c55e65709865d8202df2085a2ba45cf5d2449a65d0a8199b739de233fbd59dc2730b28c69d632f2cd9e2e7d0fdc6a8a78131d6a1eba21dde9962d4256cde" },
                { "kn", "229f0acf4465368f2abf96327c5aff7f3869993fcace80a4b8463a3aefce480ac643cf848dd7d4a02d56d0943d53be21384ee5c9a089bb8795654f3039fcd9f9" },
                { "ko", "973161440b54657856ccedc22e020c0e7d1ac7c0f0a84bcac145b1528a73340cdc75e7703c009e6d6791831d77019f8c7cc557d7b632f66c1995ea5a8ce60ce5" },
                { "lij", "ad256119287522fae3f57d11d8232e47ceae88161336eb6d44d33566d811fd38dd9b63e19233a765c8a501c48d630577b0e9e4caba6868b8086f9484146cbfc9" },
                { "lt", "37387a8391aa32dd7423b0726a2288735ead6523b11c9fdc76ef54767e1428026a0ce6b040aa73e30498e89bcf393a7db3cb163b28e58b766e27b81e98cd03f9" },
                { "lv", "e40d5af973c1b22212472d17ade54677005df2b8238195f5c737b80c4c15fa8ffc367d412bf0ab834b6ad274cd870a320225562cf70e5cc4e87ea389883d7139" },
                { "mk", "26ab88ed1c4cc0b210608475b1e9435f5cf72f653dc97f911ae1d610fa0731fce26636de08f104be0ea9a4b0e105c6cc5434a252e620abadf6283d1cac4f6196" },
                { "mr", "3db9c678859cee1054bbc488c018bee88aeaf67430e5e19c55e971cdfc7b3de354db99ddc0d6492542376def07f1217aba1f2e2b0baae75858daf6bc33fe676c" },
                { "ms", "bfddbcb8f6fbab2089c8ff89b4c84d863c38afba78771e6ec9c79c79cdea53eb4e2f0500c34e33b0d1ab6144e29426c2aa6674aaf41b63895f3908a7b309cb14" },
                { "my", "22116af952e27012ac251ae90fee63eef23022fca9cd200cdd8bf36ca82a036f0eb2c039e524ff8362d3a30ea9fb74f9a550b4b8e4d02c088892a25df0cb7787" },
                { "nb-NO", "9c41dc36e6dcc8af4b3d6ae3bb9efdb956e898bc66537fb9077c6120316ce55cb29ff3e8ea9f406da9a43653545cd3ec916975b57057d90c0102e8c59f8524a7" },
                { "ne-NP", "464d8cf9fd54e8afeea66461da0865f9738945b42f21bf90fffea377fc4dbf9cfceff305e8ad56f47d41e4aa575e1dd9b68daa752c36c69322fc9c3c39e2dcf0" },
                { "nl", "bd015e55e6a08f31a0b6e5ad47861a4e3a3fa3706d5aab53d1ccd2c085ec776c3d1f029133d9af61393f0e4fa8077188fb68969efb5bbd606ca7351abe8fc6d2" },
                { "nn-NO", "be019702341818a79095954ef93c55079c5a3444bc71ae9bb42c318da08529537e093b707ccbd3cad4cbd5cd3f810ed254ae3c026ea2aedc498036803082aad8" },
                { "oc", "6bda26120a1af9c9b12a847ddb0f2f0c211ae9cfdee543a64d75ca31a3b1833305407e0c0cf1c8b6b822e4bd692bb6e595a66acff26aa12336987df8d98ba1d2" },
                { "pa-IN", "ac48b54661705db7f573a719a2b9f22cc6b41e82c3faa29834177e90327757b0426cd3b6b2e524d786bfe090a70e2326da4793cc59eb3056b25ff24475c95cbd" },
                { "pl", "9d1151a7f51cd1b7fa1448a65f64ff7037b8bbf8b0a577ed87e90f23378a6a0b060794ab86cade1dbe1a141c99c4d850b1238dd7543bcb368514e93d24808d16" },
                { "pt-BR", "e2e5fa53b813477761ee22029ba47e9e6b9ff9f5aa6e672f5889a5b43759bc3976ed77e55bfc26c02f3ce72fb92e6f8cbcdbd29ef8da8233478a92e8dfad4d65" },
                { "pt-PT", "4c0c18a8b4c217af08b442302fd7fe46e1852578468a531e66c475d7999a4084c434a0b922ff3d74f6102c81f4c5fa1d43a7eb2718b4fea4c50f8b0b253e21a1" },
                { "rm", "e87db32314fd5993bda277c2225399c2455f5f8fecb3c6bfa976d9a227c204988afc15eebd5aed8c5bfcbaf186d106b61caf1d81ac83bf42f6bf4773f5aa9366" },
                { "ro", "334d1fa0dec73d02b84f61fb212ed90bf1f8b7c926e4c73d738066d008cc7529a8bb3935d29fa198ca7895de5757b367e0691b1ddc6de48b3de2a6afc8c41304" },
                { "ru", "df8264bf0a3df49ae56b9028a05246afe11d2f82d6da99d548a5bafaeb65dabf9dfef83bebce1e5a918769839fa568d107021e7e9e98e2e56941e7d3345d8d36" },
                { "sat", "071805556a6d60ec6679ce7ef0c886874e98c48cb44d359d39d7f677308cbef520bf3c8bf5355d872b2482608477cc99165f5bb105c1513c9a39c36df27aa41e" },
                { "sc", "0ae63dca2d31f89eb59d00aeab57cac400a233f2d22ad189d9a7cc0a160b41babe05e4c8545f690ddc3fce85471433f60ce7b67aee59a7c6e1a818847b775684" },
                { "sco", "474017d0e067a70d5e4c27aea9ad4e6133f310d9a074ee73d20912ce4e9a9b779ff5f68f2e53893a00a865af065b63e0afeac44496443ae6eb051446afbd3b8b" },
                { "si", "0e545c6453e61098cddadccca8c26ccb7c4b2531f719b410f7ac71c224dc9e419d7c2269cebfecbafb0a43c2d03130cc012da924947c35276dee294a6fb836ae" },
                { "sk", "a005246827e8c6b93294edf291fbaa0053847f6c1ff9886003ec4d1697909c14c6e15a0ce1cf4e118b37691ec772b51bdcd5c919bb6defa8875e24efd16f711b" },
                { "skr", "34f6f23b4b5222d916f98ae7852b6dc47c4c88e6a3768fd8c57e445110a66a50903329fbffb3231e90f7a7eebe684c21b5ce49f0b212da83d743e3ce637cd69e" },
                { "sl", "0d67fa99cc0f0ecdcf27f96739f4d2fc9a5eb65a96f477e2800274ddf078db56b453ff1ea33d970d9ac2cc7643b1dd6bb0772e1a173582f5a35120cf478af85b" },
                { "son", "d569e0fad96937a1052cc0ca9df2a31797134171a8cb1299479cb55d222d4d32547de7dff3a956fb1251ce5767e6e31f44b9ccc2dee715f441becd3abdb90fe4" },
                { "sq", "0b245bc877e80cfadc33af936e0b1a714eccdbb171cc54672180d802eac464e305c43af5a081181eb62536f5cf0e5f335453e92c928ee45d04770b23e4a2df92" },
                { "sr", "81e9177f30c76883576c7755f18bcf408ee9aea776576d9188e4e54fb2d06c378b1d75f1521297df307aa817e2e72b27e35221e6ea0ab83258ff184ddcc8d23f" },
                { "sv-SE", "9c15ba27e3abd7950b3623f8caa4cfd9b25f93c9a4a1bf9b800b5208d93129341d82e79adead5c23dc1801f13ccf99d83b2273bd8d36438d4011c4c9253b51f8" },
                { "szl", "feb3a8be0dfaddcb24e44e4fe542288cdde76778f201b8e18ae6f4ca830953a6ba14152a5c3d4948a86ba7fbc80e9506cfc17fdb29bf60817c9c05ad2e991e70" },
                { "ta", "f28095dc8e807a3ba64cc13ea628a61ab7a5bbb5a65612757ce6b32b3d6e1f110706abe670945018656f97be57192de3e1f51fd8c691c01ed4582c84fc4e65f6" },
                { "te", "20f463301bf8286b3aa5f545f2bfbcb60aa35cbc8685a94e3241d0ecc03cc2baed6d7e4454440626b3640835d009f9ca5302b50cbb5e73d389d7529f8b965b5a" },
                { "tg", "941fdac0376b6504e0ea617b63c79ecf140e1fbe6a43b07ba378bdd5f951d4ced3ebe3f681456e7d5bed55605358c78713c1c61045391ad04a51b145c1e5abec" },
                { "th", "14941d5a62ed35237920c6bae9ea81ac8ac271e941cc7da1ce34b40cf1002246a03947cdf6aabbd1f0e67f82abf404d5a983946b34692025811ebdbc290cbf5f" },
                { "tl", "8d1abccfcb5f0e843873695c6bd0ab393ec66f49527fddf7052fca019c10efe6a2d5d9241df4d971e84ad61b46b0b69529081d5e211d309f9f55d86571edccb4" },
                { "tr", "5d8a092e5b91ad9c5f82a08ee775fc3501288993301d946fc7ea76717ecab2fb6553c5fb6972a50db27faa5e95025bf5137e35b04ee66acdc30ff8ab2b0043e7" },
                { "trs", "cf8951941003a6858b009d359d87ba633a47b52d37af7a22aaaf1e6a9ec1f2b97e78ba9b6ba5fe3af53f1b3088dd1f7714e730b0d3224b50f903b5ceea13aceb" },
                { "uk", "c2cf29f871c6c869bd34a499b92865bcd0432da799f7f04bd307c1f40afe4a79a112218d5fbe4c5690a35b4f3b20269de8bc8e98f9a7ed87e2ffaee3050b89bd" },
                { "ur", "17af4e6bd07d6206fca34dc142a9bdad6ebda8c38bb6fde6fbb93cd420d25322f4def79a5b4602029df38017ced65ecfa5e5600d9b4debe8f2c20443a5c98370" },
                { "uz", "f170b376954cce72019f66ddfc34850f28e01ef932155ea217d793588351410f4b5be28d861ed6082e03af6271d41a0397f8e08dcd3555df3f5cde33c7b1d6e7" },
                { "vi", "7b7768fbb1c4394c76670b1f8e5cea56b1132dcec9e8878109a5bcb0ed3e229486b72771dc124e29e3773583a13c360a8763df601de24cf86596e601b09cf994" },
                { "xh", "c73591a3026ff95efd2b3f07cb07a6e99bf26b51949cabfa2f38c885c76de4185ed0705f6b9a64f08da4e4c4deaaa221aae7d405c0e23e92984dcff48d5a3338" },
                { "zh-CN", "e914e53552f71cf326a0d4a2addf78a297292546b741e9cc3dc41cdafe85d2104f432cc21e01c11d97c30be9f9a82ff866b53bd2469a91e4be03418ecb354048" },
                { "zh-TW", "6ba2caa31864a111ad88d114c87262a424a838a88c1ce01a9d6565c7b31ee0d9ddfdb22ea72e97332e08ab3d97f6bc854836f1414ea176a1ac756a0b372615c4" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64-bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/138.0b7/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "22022f4bec8f859a22d0b6ba075192cdf7c7a1e936217ba84a34c58424c82834aa355c2692cc6fd77028ec64c0a95b4ad9af844879127c49780d1ae7a5aa640f" },
                { "af", "2acd2daf462cdfb57db5f85e03e59288707c323636c2f5313f9d75a4cd9f2cf52ab366dbc2e47aa39f45432ae6638cdbb4c594b8b4009db98db7a5979cccffe0" },
                { "an", "5dd072d4f44ff6bcb29ac96a71e45643d05cc458eb5740bc4625c07b01fb4c897d577afa465ffa697fa5e2e04a758405afb2ae93f15273c7666fddb01aad6f70" },
                { "ar", "9670eca1da39ef5e07502d4722cedc41371d2d93bff0b6cc7aaf208ecbe44a05745b7e50a3e473a23f871da0a53c7f0ac8b72d12809e04755fe569668ef11863" },
                { "ast", "04bddc313218a24534e7815491b93a04583449d7082b3598e1bc720cca3d53a784122d088868ad69d79f04613e3c088de3423da9be4ccb6b11be672a94024c7b" },
                { "az", "86241965d69256506c0f96c3712cc36057aa39095215dc26857934913c7883236a18aac15392ee073fe91bb0efa10764b5630fcaa36ab2604675265bb41a4da2" },
                { "be", "ece1317cbeea98ab5b58ac8cdbdb050efcdbd638f13665a538143ceaafa468f14c1399502c67666f09e4a9eabc8d5359eab3f9d2b3c2e78055bb3fb045c7ba34" },
                { "bg", "a9a3aaeb6a147b5b8ed3fcb6c04a0d0e9cfba302420994fe06d956504a8fe53e1f6f97cc412aa94f1ba102ad4967be937073a46de15b9b2e190c3432832b63ad" },
                { "bn", "f24c58ebf4eb42402ce7cc0314cbe082e2cd835ecd869f9dd09c9f002ddeae83d2cc89f1f33518f3c37abd461ca421f662eaf28b01bd53279eebcb71a9446b60" },
                { "br", "8622741b536c13a34f5589cd47bbb1e810aa19ad5a0b05233cbcf7506b841ed5b5a7ca41c37ffab560970e112590eff7fe9a36fd28dbe0ab74a1467794a5423b" },
                { "bs", "58e34895b192c7866d84d4bed09ba35a74fb7bf30a4c49494bb5715fdb39b5056e047b95ad6b703b06b275ba3af58c2f7e00bfd7572141cb89e47234d0def3a1" },
                { "ca", "287d008f3b558ffcf722059f8ca4f44e852dd133291c9bd73223a9ddd5a68ff7566598209b835c7faedb399c52e44c4e19a1de913730132ab4fab25debd60277" },
                { "cak", "b36579348e95ed0ccd511bbdb8e913b9118835ff86e857f8382a51b6d22ebae41115c90b3e68e0167fb62759746df36992176e61f4c819c2d239cc40a5a0636d" },
                { "cs", "f1011eca263b5750c8b1a12ab17642afc62fd1d6b7714a6bdbcefa69270ae635518ad5dd14263e63e709f2963f4afb54eabd38b6d10a8bb8c9329ca052e03727" },
                { "cy", "1189b7ef15ef80bca01eaf354720e7065bc73a83b45af685bcbdbb7870a20e378b1575e658aa9abe296de5d04fe37cffc6e2cf6bdca0d945552cb4d5ea6ab4a1" },
                { "da", "098b80f376f65e8910eec514861f453559ab0b4c8a32f0fab0b18c76edaf96c14979dd1ecbf9f342da6bb9268330bce0ad84ab2ca7099a13d92ff131019d3bb0" },
                { "de", "4e31bd74f4aad92ea75dbdd060900b7183bba7857d5df453af7934f710646a8ff3e7041798899723074460a3e20dc941d250b9ba8820aeed96593cf7c283daa9" },
                { "dsb", "7338941ccb87d3f648504bf6057fcfdb99a0c33dcf7c87aa2206d802b7109651c1a6e8d964e1f74fcd2556939b91af58324b13b1d1852354506b65d714170a64" },
                { "el", "2ed3da6767eee9b3050259a4660e577f912329850ecab9080e433110d3b704ff2e8cc56db3e9671e0b2e620eba58474e8cfb1eb11377f7954fd7fe382252b9eb" },
                { "en-CA", "6cf1ab02d3746ba3297cce736ca26948770d2b24dc3ebe2a86714263a83f1caa4cb42eec8e798003a394d75c6ad54821bd88b3f8f59989cdd50b5552a0ca3573" },
                { "en-GB", "c193a26c1c8c2d8d5e3ae7ac1ed87d03a911f8586550660ee92ceaa87479aaecf9d900ac74922d7ecb3571260bb1329ec542c9abb25abf013590bb6c9d78316b" },
                { "en-US", "0449001c7da00bb07ed878054fec943574c0ea1b6062d215a3cf056e8bacf70da95aff5e7ce47c6a4c7a9e38f36433871ac14b3aa4be1be50682432d31c92852" },
                { "eo", "c47efd7e361d1e78290e828d8b5bc9f27209a11805fc586b90a9fd5069edd29bfec391a3c5504f9d0cc22996f6e7808bd3ec0f1a83330f84be582c70ec36268b" },
                { "es-AR", "4e7c384e293d2808ca5e79399043cc0f573d1c60b146f346aff66e4a57e1d7c4cfcddfb7a450b7a7b8a5b133618c229dcf858809ec89b863439c89d04ddfbe12" },
                { "es-CL", "fb2611ae85ca7af279143ab7c71392a67b166b02a0c34d6c5cecc6f52ad1ebb055cfcef23ecfeb2a52742cbc080d66cfab579cd4d638ffc5e510a4a3ad20c640" },
                { "es-ES", "a29fc508104087d62440ec1b5786f7b943ae7c0eaa776000555d6a4c5b9a7729195b0923ad17bba935e15e8835c390fcf159144e1d5270039c7dd0c551045739" },
                { "es-MX", "a52f4ce91c76f987621db160003f519faee1a3fce7652c25b34b22a94582e58e79503f9529f9abc16ea7afbf0316b7862935a2126b2b8782273f10f6488286c3" },
                { "et", "e20aa08231f40f74d263a6ad0d65980e173766deb351492fe2af1b816d7fc08ca91bba30ee785d0ad66e27e615dc59492da918d13d698124dbebd022087b45a3" },
                { "eu", "c2ccd56277843db5507d0179dc3f4f74678d45e61307d95e1563b26e6959d9acaf676099178e0b750813b14077b839f981822b5525416a8c8cd12b66fd0ae090" },
                { "fa", "2034d44aa8a8fd17dfc20cd07e8d7709ad56c1bcde52e7a0f65a7d5231cbd8f112e4961519c69d431785e1275baf1a9defd5952e7b4c3b77e4fa156ea684c10e" },
                { "ff", "a1cbf5beb1127f4cc5a3ef4c903ef208cf7a844edd69eda8bbe0c432bc69a51be670b537688301d084eb1d949a08c271e37688c5f1f3c44b445e751c51578905" },
                { "fi", "c4d91e0618e6cd72688e9dff435b96ab68aba1a222db0ef587a6435364ee435d597a45c5eed9835415d1fa748d3f8a9e568ab9175ad31a1a35991c75e2b1e0a8" },
                { "fr", "e2a9fe797781cfc25e792139edaa5dcee685126c0f85f3aa97cd1e6f4b02a0736ba344c82eb17e605772daa324e7198bdd61179438c6594ee39a7177db09b277" },
                { "fur", "c461531cd037b415a46f54062699f8a17f1331fabbf27a93d949d51c7b5079c41ef9a46f92075eea2fad55a1bca43164072f37f1b00abbabb4927f4468c5c134" },
                { "fy-NL", "fce990e4dd8a42af10fd4947128be6db558f86ee89bf057986bf5bac02dbd0bfe8cec8830f83cc41a1dd89565587cab684d744fa4f540db61debfc8fc75587f0" },
                { "ga-IE", "426c0de9a5edb04bd66af84f5d0040075f731db9de412cca2209fc6e0c8a21f2f4e9a6e342f02b949a15502151294fe79e07bc87497c602140359ed730b57200" },
                { "gd", "3a5ffdc7f8ca06964439ae8e1d6f6f857380639261d21e117b48981a5b60704e13edf51f5ebf669bb3f90d6048f5a2cda0d6886b1cc3433171cbf37257141b53" },
                { "gl", "80a7e57bb112b62c3273000805b6131d29f5fbec09e3cc1abf676f5a03c7bd1c4884f4767c3bdb5e468553b8dbeaf5f6446cb780988657e442edb83ec45bde83" },
                { "gn", "dae1f2d3baf18d894b54e24f0aa24a403c6a61693b8c75695c6e480d4ee3393e9eedc5c796d6189c41238471c1011991d77d59717bc980ee19a07e9618305f59" },
                { "gu-IN", "bd43e39e3aee7e9f3a5ba571c3102393b129d11cf6e97f3d6ed768964fb03508067e663fe5b1ea153e810294f3f9eb7c0911481e38927dc837f617d91a5417bd" },
                { "he", "1b5e0d455098aab2a1212519fa92ee5aeba9070a29fc0d008b9d60e8915f823b9e5f151c7c8add76557c9b5db73d573063ac95d6fb2bc12d10f9c80d3233e685" },
                { "hi-IN", "fff9c606fb9a9e818f1ac8cced47dea004813d5f0d8ecc050949dd855e490f7cfc8b029919232dbe7bebe268c0a95fe9ed83dbac4edf111b6c9c685db55ba25f" },
                { "hr", "4d360bf0f6923ca853df0c6b07bbc4875dfbee05aa123391532164264d4e131be2d510ac891ef6f18deaff8c7039ea023bac1db89e7d53115539f8b65abb1c79" },
                { "hsb", "71c6292857945cdd9fc30187cbe666734365efedbd0d5d91f683e441ff5b4c4c8d5e8b293dc97ecdb1ff981de8f84c712493752d2d5ef006cd5e72e43398ed88" },
                { "hu", "7ccdbd0fefb7dcdfccdd072138f1c7386f00ab8cd0fdd4417850b4be3d25ec01d33699b8289846f014526adf11f46469f4f7a0f5bac7d5b196e00ce899955e02" },
                { "hy-AM", "24ee56bba0cfa652fb5d5f232d85cb7b1891a1c509fcc8964eec46fb6ae3168ad26cde63fb36b3516ecc0a689abf0a5c606de4842f927277cc792be67bf37f1e" },
                { "ia", "ef3728413a6f85a3af1909dd7938b3f2f09012d4433f41c11b2d9eafb6e88eb177e93d33ed6715b2d2ec52561dc05aec192f375b97301916ab6f56e12f2e8dab" },
                { "id", "ceb95eab102151187dc92ae88c85ee88660d26843ff3e6d331731a6033b65dd1c3c78dd20f13222a6fb6055ab917c87eb93e3386753d8ee1ead8dd84f23983ef" },
                { "is", "63fc93e3b93a97a3ff2efd020e2e5261ff7a24d000e942c902d60a21b7f7834af4d4a412e893fc51407c307afd9d3498be01226f4d910687e6a0bc2f6e2248e3" },
                { "it", "a84dade88e2fd7a0c49d19e461e0b59668724179709a7c263008043d09c48b09ccd9ee3df1ce88ebfbd459a2a4c43fc5e6d0938a49cdb084155c923502a07b43" },
                { "ja", "dc9f104562aedb6d76bc7d85010f75dc6a2048d7e13a3a55651fe1971851cde7b1ac6c99735b6b2fd2da4022be2e8e3e555070482fe168466a1a3d447caaee4c" },
                { "ka", "3d8b0493ce7803cb1a2f40a252dc867794060e52df876005841880c6d623ec99997197ac9ad277b887e194cd56d08f5073acbd2b656812112f512fe0f32061f5" },
                { "kab", "7b1ec9d3dd3c8f52f8b0ae98b75ad703fc3d9bcf1327159173c488b1b24addaee3e2cd94526a931035e9f321f5e1e588a0572e94417bb28b84c64d4208bb264e" },
                { "kk", "b1a945f6451fc0848fc599110ebe3ce714b419a7ef0831e13723f027279e8e32c0026d1ec136d5feb4bab026e74d0311f8b311d1b68ae4c4d4d1b02e599c87e0" },
                { "km", "83753863073bc7f7f0bdf16e11e316a120d53fd4825771bf6c4cb2dcf66dbf539873834c106817b6f6fa7835c876d2fcc1a503dc85c9cfa993e240d990053c22" },
                { "kn", "a0d95f78c04730d9f4e988760ec3484a3b5fefde0bf88a1c7506fe2612ca2898ec139463aadf00e05edee489cc4d64a13f44c2204841a36d395d10ff921acef1" },
                { "ko", "d90b044462af006de334f12661330e176d181498c98869040e071c3ad36c651697dd1fe6ecd58fca4033f2428ebe8c70f639eb45fe2bfe7c5b335d04aee34a8e" },
                { "lij", "4d1874a138997d7f9ee513336bd4791b1de02edc7e8f5bf7764908d1dd2379f593ea2a516a78f6dfbd4bd275cea5b30bda415a73ffac3c146601ec636b4e98a9" },
                { "lt", "cf3d12e60987484212f32aaa9cecc842f8af7032a55e11316a8ec1179f637fe235d4a02b7a260860a7380829e14d5a82e70fd22f349bc7bce8fd4f74d96fb8f6" },
                { "lv", "65e004047820b2728d748b13e99a31e7dfe280d135bf09644239457b336f5dbf8822e23ffbeee88e4698452280ae76c10c3df78e29b75a8128620a94aa5fbefb" },
                { "mk", "2325a6fd756d7fa3d05cfbe7d56330432f9fa8a6582771413d97bbc6f48f293fd9adeda3a284955e16f38cd41850ba0565144afa797782447fb81d622d2324f7" },
                { "mr", "2dfb8a5079b04649d4e792d1fc6e1d8bf69761078fa5451e97fdc1209fe6000aed67ca933ac542249521b384964af70fac9d77acbb5f816cc6c9e893e60c7040" },
                { "ms", "7e8dbffcf01f9445194c108b5063f70a5d1bdd3ea6f1eb79e79f69b7bc01b3fb5a224208fb7da908d087def56eb20199e0039684901ae7adfcb0121943b60ab8" },
                { "my", "17fa32e808e9954a4fbe67c82c88445408d483b2f59ba9ba8d6b50ffbc0b8c33bd920cc8de8c10001d5b5b006859ebc7f80474839041d76f50ad50a319ef9800" },
                { "nb-NO", "aff56aa6d0b5d9a2204418120e7ed262690d8aa31e8cf54c1cc7117fa5547977c0bd7a06def466c6f5d43b9fda7d0bcced80af10753797d0430f61e8726e76f7" },
                { "ne-NP", "be9aa4d70c6025a25ce28296b11e74eb75292a4caf94a624428c9ff6795fd3dd232fc34a8dda29b3abc14ffdfd279420affcc8ddc6a9d3c960aad55d19ee18f9" },
                { "nl", "e5652263b0268a516fc461ed8501d217108d73a3e6dd22c8383e0f9ffd25197ee2ce0f2b7d5a5b3904dda65687877b946c07c55d88566b9a21983bd8e455ab00" },
                { "nn-NO", "66d182d6ddb1babc482b793efb8b4456af9cf7fc79a4747c0072afbb2a2cd556c3767e9793c6d939e4c0db0a4823b60318d2829bdf4533262537d0bec1cbebc5" },
                { "oc", "3cb11ef338836faa48ecab5f3f9a0fe6be41aa1b5ed98fa127674daaf933725566d374136cde64a2c156f18bf4310af66e057b0c768b7433edfe770b27b7dc7a" },
                { "pa-IN", "410679b267d7961ffc88ccfbb8d459ea1199bc9acf6cbd2ecd1565defd16025029a11c57dfc54a98c80d0b920740f78edab4b4939fcd8ac410cbb51fd952f2f6" },
                { "pl", "931047883851f33b71d9594433674cc41dea2103cf9beae842519e2a18b8d38b9a7d8d4188e7db5f92efe597409c4b43700b4ef2cf33069d82be3ba227b11cd4" },
                { "pt-BR", "cf12e427a946169afe48106731563333da5281c29d836f43821e746ceced6159a520dc05efe22147d99cdbb344a6116a37acec3abce3cea55a1388b5bfc2e4c4" },
                { "pt-PT", "de6cccdd9d3384f69dfbdae592f8fb4952f4817f445a39546fa1686077593e39ce4481043cdcebf51e52d5e8950d8f8cb82d38a0ca984b5d17c30c8773680e75" },
                { "rm", "e11bdb4c73d37dec45a2d2f3288849c956d451d65f8afe140a2c12cd3f4599a1560dd3545c16710e39678c32c8cac054a82311c52d32c8cfa870991da58897d8" },
                { "ro", "05cf44848c5a68eb52cca07d0db4182391b0637dcb96d234de319aff04bfe957b6fbed9de0172b9b8ac90da452fdffeecca035f162057568274a39daf46c615d" },
                { "ru", "63bd876dd2e7af987b7858249c1c26b1f6ceebca6d8ff9d9913cbe20dfbb88313883ae9b253f61a759acaf1e6760f3996330629b1028fd03cf56753ab74c7ffb" },
                { "sat", "7ccc638331ffc67e0e5209f9256b539cfcb8b762ebb7e59ee3a40cc1afcd5000f1236c87f10862e6cefd54416750b017812f0422008a088dd76c1123cb8e48f4" },
                { "sc", "c495d7afee4b3adc09a3866aeb382ab75733d369175062daf482b6c709878433fe87e09f616471ae4393bb5ae805d7bf2e97bf95372046c0864fe2a4502d7225" },
                { "sco", "ec0d212de491196a4041d0779a6fc8eba1bd44a7225a9bcaa3fd951275909fb90d5c939625ce882b955b85dad0e2af32daf0df16902748b658ff7c67d35010cd" },
                { "si", "bd5dcfa8b192c76ef7a7da6eefd6c7681e4bd5a42193a4bcb1b77e825b15d24eda39e66f2b2d42798babf05eedc26693cae026a7c1ff2d261ed9ff1ce7f0ac71" },
                { "sk", "4e6c1c2b76a40ef53ad066ed59b798e910c751d436f4bfd25cff7fd8b59b09db3be49afa02316044af602f9d6ffb52b885121da1ab810a1fb5dbd226f49fb08a" },
                { "skr", "8a6d34a711ea1030cf784ce07507dbe90e9d10510c27f17290d0852a6ccbf1d22ac1e59c573cace49882ff4d79bd2114f482c1fe238c7443a7753966009cc2c7" },
                { "sl", "58eae15f21b185c3c9fcf6a6ad43c1b3d65fbdca679495c4e0bc6ab0579e3f6bee7dd208d6e9f0f323147b26787b019f6cc95b0a90a61edb0902c2b4b3ff9d9f" },
                { "son", "c529209fd7afcce55fe55fbcbf2e9efc304851004eb575cd028eb6b10f69d4fb1a7edb76f068aab967c4b5fb310e98635c51058db8ee94b914673bef20fb7876" },
                { "sq", "06baec4fcb8a0670b95bdd1ef5d8435f5d192bd11e04c6a4486ddfe9f17faa2248f1c7b7dc414d44616b5f03d5aed3dba1bd062219d68b991d44150d96cebb6f" },
                { "sr", "c1eb415fbb4d6815c336c50035e7f3d0407867f04b5213f827a7f255ca8d0e79d6937710d845e7622d9c7729d793c6b4d6d43b01ce1b4d36de7f014c0297e8a7" },
                { "sv-SE", "f9ab3313cabae3c09f34bde2f0d88fd6faab409eb9b7a94f94bbbcba842acfc2f53f924708e17450731d6680faaa358e4b2b8c6abbf16491dfff5a69573cc7c2" },
                { "szl", "dd9b18b96a38b6be5b48da2a43dc243b86cf4c4855095f027958f87f39977805ba6d221b5db381870542d879031ccdec515a2ef46c28f65120d3aa807b37e787" },
                { "ta", "112dceb4c336527812777289b20156d8023f63ebf767d614941f4f1f29b5d9b46f26b54020394c074e404b2d7d3a23ab4fb35af07ec35d727eac1eb90e4f3450" },
                { "te", "9cf52cec9dde70df0497ad5825243fb771cc6cec8c2f55c304401e24d325a79d5a14bb72cb4937b571b4b62823c759f2e5af5deb5b3f21a96054947b086cc026" },
                { "tg", "b98a77f1d5f94553066faaa29169d6b29bebf6ce7473e3a8f7e4cf21bcfc6607edc7e685cd6246eb10a76c24d69058ce479f9bce3091d094194e6e896da7f8c3" },
                { "th", "205b0d2de0847e884ad79d80e70dbb0a4e34a02edafc8c179caa7832686aded21a1d49ee273e6fc4a550c813fff3d905c25a5ad4bbdfbba432a463775e848f9f" },
                { "tl", "7eda96139e8465966e7565290bbca8f05950d4b388def8277832418aab1dadfea5a85e105c4a93d550c29409f226d9c57776cad0e6104d289f90037e3ca258ff" },
                { "tr", "23da0cc0bb42ab9fda4be365cab634f7b7db4afcb47c4b9d0bc24ba1b8f151a5d60a4cc288d96ce322ac69be346c3119ca3dfed1e0f88e3f0bc3d2768a35a64e" },
                { "trs", "c16ccf41612b69f915e8e471b36f89f222e4fc28ab81212f04cafe579d98318be2b412b47385620b7328ed4310e4250319dbe330298b57a1d182e3106f11bc64" },
                { "uk", "0d6483a7953fb27ebaa16fbef6ba56397f0ba973ca632c51d6d075360b3c2075247cfbae132078f7f2140b03ff9ac9124789b1d810c1d8594fdef0448a4676c4" },
                { "ur", "b2f4ce0add16d7e0dcdfdf308e12917755ce5ead0338248581f7a4a8fac1d37d0a4b4811ee3896c0c6bb97c4c6a687e5ed1ddaf3d11ba964f6c68d5de8c5cd90" },
                { "uz", "e35645ee060cc1c685f93925d171153bf4f9277fbf58abc08590744211c99bb1bf4261af5164ab30cdbc2dca9961c32c2c5cd8cc9c44768f930272a7db2f809a" },
                { "vi", "f04896c0461c3640d31dbefd0abb4ec0486105cd6ff4cd0dee6fecc06ede91a0966e2bba02aae8c032c54518b9410fdfd428376d2e73fe1038c277f136c0bd1b" },
                { "xh", "21d4430c96e726dacd233c5ab4d62fa4bbeab0a267cc85740f64a86f3efa7a743352002e3b76e06bc67986551c09603f38462f669ed89ca3d9f07d3e43ea4f94" },
                { "zh-CN", "8e910f928d73b2479d6e657cd3b4fcd76601dcbaa198f9561fdff4b17e3a55aedfae4390f2040a1dd629bc2a3484ce5c36a79e37adcf43eb2e35f3fcab15e885" },
                { "zh-TW", "3da6497016a88dc0c02722f7f990b31d27ffdeacda7f0486804895627bb92e05d09a8495d9fa4c0f4483875a486385f627b20ff6418390f1ca5aa30546f6a182" }
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
