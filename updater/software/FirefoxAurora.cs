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
        private const string currentVersion = "137.0b5";


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
            // https://ftp.mozilla.org/pub/devedition/releases/137.0b5/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "e33e5d0fd98c578eae48b8530874ed13d57508cde6804dead5082e44e133a15a1bc502335da3cd9a4382015eaf1631ae1c5e75ed4f97404b6f7030e5f96cde45" },
                { "af", "bb8ab81a88aec688d95198166853bc9403c46e7cccdfd74c2d1959b5be757ff38bd14bf2c534de4259feb071e37d9c71901a29203595f62992df71459de181c4" },
                { "an", "b5fc0e89337b98fe6638ceaa2e46fa0d05b9d1e9306c6b2a974d61b8006024af48c626b4e463f790a30388cde58471ab75b42d2166c9dbb67d3d30d21867c91d" },
                { "ar", "f4c49590994035903e8264001929516f884fa8f5d3fbec842e77240e3d914350c0436bb31d6d9e0333bb7ab2fcdf8218d6b9ceb242bf12c51b4c7e46d1d3aebe" },
                { "ast", "004c40072f6a081adf217b0a23af2a1f18fdaf036c1e7ad1246eb4b1f4560890c457d80fb8e888e50635e9558b70a9dc956a9774361a225c024c57faa517eecf" },
                { "az", "0c56879d0479ad80790381b6f188d0e6d0b24e6ac7287b022c2b5f8a3c0dc37945f42df6c9298ca4ff229bfb13089585c99e5da9e070641de2150ea4c5048631" },
                { "be", "612b0fe7ef1370fe6ab439142e3930e378fc747d842f3131c3da8f86ec002d717cfb871e0af99e2d11a1582f53926499ae258f152de503bb04f6a0c3e10c7285" },
                { "bg", "fa8a3791b963f7cf5020318f2d5f35cf6c53a62103025827eaced372b0b549468f29aacdba2f0795a2c641485ecf055e147743de3214b74b6a78bf33e7fcffb9" },
                { "bn", "353a8794e39e380f0474d632431cd8d0af4eada909e8c3c378059eac6974d288af1f8a03f71d603799b339fa05cd2071afc8218239265334cdaca6c59fa946a5" },
                { "br", "9610b63dbba6ad4328b2e16f82cbb348e21a2020f0d39bd90b4ec7b99582141f4b6d1a3ce1fca4d90c290ffa9cd33196a9ec625eac5c76cfe6da13c9768921d0" },
                { "bs", "5e1a4b8a2536cd513dca0cb65395549a8beda458d5d53467530d91d3161f9431e82bb61068a7db867a7d7283da961484e3123f8ce35a170c15f6f02eda026fe5" },
                { "ca", "800394524cf64416fb68d420d0fa7a9ab78c904f9cdc9226c70b9e24bd6680cfd342a6cd6e0af5f1bf6cebffd37bf1002cfc46f88732861549521267a61e097c" },
                { "cak", "13d09eff3a4fe28ea51f69fca734314bd8b7d69607a0fd2e1c83b13508f70e3de3ec6fa7aa3cc08dadfb1be20add482dad8a35860fb9d2498200bfe0e8cf9a1a" },
                { "cs", "8c1e98a126cfe593755209f5dcae0353e0f3677206a1e2ab9246a28e775d2cdf248ce46e19993a21bc3e7fdafe58f89ecd84d0cbea402b90db849a915199ce7d" },
                { "cy", "6379ed2dc3aa9e301d5cb5424dc6b2593d584a04c5dfe21d9859a474e2ce27fecf02ba4a19144f75ccf70d89f4ad6e5ddb92c027bb17b46c1b687a9ba1762091" },
                { "da", "b8a893c2ef05cabd370bed3370b763a8871a08fd14b5189a26ce33400a7ed820fa5ca55bd9e6bca48b70fb87bcc5ef431d494df1ecd89cde087bb947a507a4ee" },
                { "de", "4694a5f09a4af4d0ae3c1c1ce911a8c98f5683757e51267b466e5f364d039fb53feeb086c7a7367da57369dc1a1d435b0a8a152426a1fd7bb4709479fac17b5a" },
                { "dsb", "da54e88b2f67d9fd2732d65b58ff4abe1d5ff15915e6a7577fc7293fe40e35cdffeade4631e3c2940ecc18ee444ea13742d453acee8ea88ea17e0d501f6b070d" },
                { "el", "f4b497f7893e3b37656378d6eb43786ae644f802be35f8bb0b10ed76b5fdda7d03f846c6529965aa72508b16e4b7ac05a80dab4bec0534463c4b8decfdf71258" },
                { "en-CA", "4b97153bf434692b24145fc52a9f318d230150c3ecf47b9fdd9a7b6d7e896633eba49aa30e36f7f7d745bf1474ccc8885aa79b0b440e92640526fac9cfd7c373" },
                { "en-GB", "8b863c8623cc3c6e3fba05399419fb36f97ce1b9dde36faa05f7a0fae460fa61931770db95ab0c2021f6619eeb8f4d5687e213a6b887c9863eaa95af8531773d" },
                { "en-US", "732267d26e72e8e704dfe57e9d552c771ce76c4394d6ab2d26e5cce7a1033424cb9e24c7110bd65127b657b5f4406a074ee9c043d65d9260f0b37ff635b4d6a4" },
                { "eo", "773bb3f4ad6241cf84f82f386d0e198d1df6e8a096029f162c58cac8f30668506390ffa532f045ec352474925c70c0c65fe69edb72bd55c306a98a24e3c26694" },
                { "es-AR", "a19271a55c9c209d34cfc1b0efba6325f4b8a349f8daf4e226e8a843e9d5d9bd1a61901728c66943a7da5bedb3d463c40c29a9bcb91a566a4ef6065c6e878fc1" },
                { "es-CL", "a082a23b5b02ef2840b552092634b33463576725faa88058ef269ab46ceb7c83df1384fa442c9c2959b6b2e0d47b0292b3199f996e0f2dc6d2f764f63934201f" },
                { "es-ES", "60ce582acf45ee6db4830e92711243f4635b910f141d9c4fc329a2d54d6353f74b27a9ef2d5647adf637c3d60ef219557b1087d4c70a24154ddae26e8037300a" },
                { "es-MX", "18a5483c4141e7bca089189cdfb816c4e9f40f2ac9e8c6b48de949ebdde3ffa7cfa511049b86acddc8bb16c0236f5d8be7cf71c315fb19701c4c29575d7d8493" },
                { "et", "4a42b6d1821a7d58158dd34c18569d23763d4fc108867f8c8e70250b46e5a0e45cc820f4d724edc547c03f64cf298c13348efea9996ccd9f90b0ca0f43674bbc" },
                { "eu", "e411d81f2313b6b1f3bc089558b7392633e3eec0923c7184b64ea1356ff6a4974a9ff27401053349992bb22d6e3f1373f45c09e2bf28e4447d4fe9d5c9978352" },
                { "fa", "e80062802db48a9172793d647520c36f8f8b0b777406c6dd103f69916e14612dcde857791bd2b80a05fbc44a9911f43857ba3fe90efd337ee7a1d03835c01604" },
                { "ff", "37413391790003590b58c296b16d87c88460670fb37c1d67a3041f9af325bea1399c1cc72cb2418c88564f9668adf66f2667bdc8cacef6780837007f0c780f48" },
                { "fi", "2494ccb403455082990ff74bcfbb589f196bc664570127511e5f4de90d650aa6389b0385dd3b4ae605a517847e44f224c958aa43890a8d996663dd6a7307d0e8" },
                { "fr", "ca3ef0ef00738f6c98744b58396b1254f286b1f9382a57f64254d36f32a61a950e05e8304856450baa238f7c8580a7e47bc6b586987c8e6ef04a13eac5740464" },
                { "fur", "5ee70d8e7e9477969b0ba8c3080e094c7b03c74b2c84ca324ec82d08258735a02a892a64115362b678eb83e36c12a67690fec59de8f2104c49e3bc0517d52860" },
                { "fy-NL", "c02156cc8cdb92f1cddb110ac1dbb8226adf5754d480a3a67686292ac4e596e587207483bdd8ce29622697212fcfd50da18e2f8fa9c9303ad7047d07b743afa8" },
                { "ga-IE", "9648b0033da15a043c4e5826cead51db64a2dbc1bc99cf662c232d29fb2442b3eb6c9d9674134b3941f85e5a30e9ed988266b018138dada25c7a2ce564f78153" },
                { "gd", "f4b73e00ec22d13414ae278326e90aeb42d01f4efdd5397798f15591e0e38a70b689d0f84b9798a617cd12e60942afecf5be76c52e96941340a24eb75ba1220d" },
                { "gl", "5f33fbc9aa9df2282c6665611b0e4d80a7814e40d0a1f5d5e32ae4eb9f970d70d5f6ee80eaeb3412a98762983ae89da7ffef9551586106b09780b0ddd02297ea" },
                { "gn", "3237652f8eed516aec9a592bd1f1b793581b3aebf7c3436d7f3130d9015aec7aa9af96931967fec94520905641a67a72297743962fe52086c70f40002a6b6707" },
                { "gu-IN", "2f5656e39a830a245226dde68d23525cec02f751acf32b7819a5e9c9630676e89cc3c557d999794dd33cf5a8208426bedc8d2015592006037b2ed5023a02fb16" },
                { "he", "1d19fe29632378ddeaf9da88482e26c8165f701f5491c3c7cc99183fb8b5981b74fd15bf49e7f340ab2da88bac166a53c35d6aafa0697345538f30f13f983066" },
                { "hi-IN", "df85ae60618430dddeef611a2f879c1a456fba30242942e40eab809cd933a07eb4143bcb63a1baccce8414284bfd06b45171086691576899fbd1874e71ee110c" },
                { "hr", "2aea393c8539a8d15f8b1b392ab94d8c7ae9fd497d6330e88c0abe08f1eca83e4e509ae47ad37962298a2af95c2c9e38da8c24c29c047c2a2a99ca34483127f0" },
                { "hsb", "b6f1e67e51c71ff807017ee3166e92632f1a707f208745f10f63105361aca3ddc5505f3c0e36e2bc0c21f31bbec5c88d023a812e562a49304128289ae05d97df" },
                { "hu", "f3e2647cf8132ae1f5b286962eb3a44386a915a24338499bd19835d8d66f9872e85afdf93de35dccd8cdc37216a127c55b869236f43db540a18892bb3a952e7c" },
                { "hy-AM", "4984190fc52ffaadec6a624719b882e4378753f47b466743608a7aa61e354e0f1503daac7a35930851c7ca7dfb577ca0634635c2654d1ac9e8072f1de5038a3a" },
                { "ia", "d17632acfd5968c1607d9519e9cecb5d7a801ea47a80fc0b03dae60846cdb3aefff83bea6c4a01b76ab7f1a4552bd6bef55fe4defcd1a11a7544d4a052db9000" },
                { "id", "3aefd5e160d089f7273e6a531a88aab71f955aec67424edbc3e9921e33586729e52631e604e9485cff2098f6ab4d791f69ea7aad9a001d753277bea68b842f7b" },
                { "is", "2545c7ee26246065f6db931a76b164854398b6dca0a922b6c3f24b3168eda040a9cda36b1a517124aa44355eff720c97bcde31ebbd29c81b6a37af3949bd1516" },
                { "it", "201594178709a1f42eb8e29b33bb4df6bc2590efdfac2a33d00ec880f17c0fd1e8b06d0064fa789f99f4fd9acd1e57657a3812063dca9e8cb372679720d48bde" },
                { "ja", "1e4696a2ae714a1c6bd82a955fe9475a168eabf4d72f1d00eb2c97ff42ee37ebf6612b81a98ce2fa859bdf9030a6510956f33e1ba2629914eda565f3ee692928" },
                { "ka", "cb75dbbfa840ba7d2ea38eaf96037ac513008dcaeb50bfdba9a0695ed9353db81bbdee7a567a630376491b00d94106fd9539c6fef2b2d4ed0c41fcec7bd8daa1" },
                { "kab", "f2829fd829e825cf83e3429f4ebc6f5c66324086eb162eb7fdaa19f4021f61a41c13d322ceb7cde836c5f911ca9a5bc375cea21aacb16f7907504cc9c0092127" },
                { "kk", "de97579bfe45a974b5674a08bb9f75304220a48d0b04e7dc760568b67d625b22952cba0ac4bc22b393b8dc1475764dfd45365f59ec803564b4655e8bba7f5584" },
                { "km", "59d7b67f0b53e7dda2940696592f3f45dc8ff81c339731f4261dd904978dd227a17f0d29c542046d748595885c6aeb218c388f647582ca12fdb315c7a4132efd" },
                { "kn", "f38f025d0c11465831634f82ba8fa272988216472a34ccda6b673ae1f18be5f65f9cb1927541136116c88fdd3689b0328136b700a19e58179490794c18440d29" },
                { "ko", "6e3ea8d4e0e9565dd1da4eca0c3ff827a416f18767e53d50be31b1814dfa5dc1d8f3897d7aedffbdfabb5903a5d649b55858ca3f85ab75263cfb9911d40a6c76" },
                { "lij", "9bc633d7bb9d0d0aa9325ea00a22c3504aad03b852b9f8fd751de75a69274f3824e9cb9ea17b8ce2bcd7b583fb71e61cee079924fe79ecde7246bce31efa752a" },
                { "lt", "6ed61052216725b25cc444c9e9da2e1f21ea1d092aca4c424101caa74b64bb6229c602b26c02aff78928b26c145feb400d978ef14cb9bfdb83c60ac579481f44" },
                { "lv", "aa6a3dfe0196263c6492c901f4073e8acbf68c40935c06c586d78cbfe98de551464b45401dafe5780f12671ec768d9c5ff8c284ba5933cf70f79c3e5d0508cb5" },
                { "mk", "a7799a069e73f9240628894fbcf2968eb7fe04dcecd8dddb9a25a83101eb456611555346dd2f52c2dab7ab7260f0f3092ee7106b5cdd5762bfbf80641d71a715" },
                { "mr", "1674850a6c8b9e1f08ac37dcd0ad849f3ee23b4a55e8aa5d5f40d69312b39a39504012edd0b60c03d9e45f6ab774f32c803842bd92f570ef2047176a5d482211" },
                { "ms", "483c02ed506b2abb6639347d3aeb39832be6f1bcc9093a1971656df94ed51a08287b2e4792bd97ac6d2f65d8b5ed55ebb4741a95405dd827f35dd4ed5f04eab2" },
                { "my", "fc0b1912d47658804fb5cbc9034b9f78849334a45ccbc60835c1df1b1d8bd0e3bc9056a60aa3fd5e44dd3ee0add0ce630a584a11d2f6a893a7713b7210d69043" },
                { "nb-NO", "a69bf54f60f41fcbd7dcec3a07da656a1756f8a2d5c599d1a39e972263891f91cd7e15b0f69e9b72866b94b7b7f4a6d79407ad7e486c83a055c6b2efc93ad73e" },
                { "ne-NP", "b140e64f6e2f189aace1ca8d88a274ff796642c5f811a0a26f44874e577d94c209d4eff6dc509b59780db2bc9df27b9b24490350b3567f9c6c17a3b862b20794" },
                { "nl", "393a732193d5431e332678151be667ea2344ebfddf22f3242431ad6d07397e8f4f867f8ae2399e3a259c1b3cdf8f7aa38f5180e591501e50db87d1823fc1916b" },
                { "nn-NO", "48b2ceb18e9e411ce7fc8805205e90911f9aac3f004e5657770c39358da5429d64ef7497f8268f88d8587d0abf209606c02aeea195638f15e470cf6da20b23d3" },
                { "oc", "438da08bb2e7747cb03cf3a7487f28fc7ae9cdeb4ea0e2ee3cc6be6b81dad1c2d7c77f4e3d5e886284dad0fc591f647e34e60b1bfdc47e55963705a9019b5c05" },
                { "pa-IN", "327daeac2e01a093a2af26c6f5cecef82b25b888c5c669205932193513c32895ef57f49b3677c5239c789eee314ea1aad2fa27286c9e3e847b69879b936343f1" },
                { "pl", "36357f924607053684b5f996b7aaa4cf7801b278800fc1fcbb162a3fad18a30a97cfe2b321bbb69487c95901a0b91698e2207e14e16d0985b638c4194b3c0029" },
                { "pt-BR", "ffa6a113eb10315c16d66472c365b3341852675175f6e5c2880a43564c9aeb21072eadf69fff417443c9680c71a0f85604011a17718863a79005b9c6d564d4f9" },
                { "pt-PT", "1ed5203c84e7c6c21062a552638cfca9d085619a06f0a3699abedcaa5df9d5a6658d817b4ee34eca0abc502ac8a555100996e0463599f728735ac70fea37516a" },
                { "rm", "de3e0ea5eb3de814dc952459a5c1a45f63cd678cf66e4eb1b8d1d5138de5108b01a208819306fcaf9deb4db02c4ee61b4d0adb5f02bcdd3e149b62a6e2fe9cc3" },
                { "ro", "9b5bf963a0299230d36b8826ebe26a21c18f58fd8c0772d5dbe95d6c295298990580ad4b265e55b200d891d455f39cbce46aa4809c4ec4f7cf793e007194dc49" },
                { "ru", "608632cfd48136ee3c19057d9049f860a431536621e3aa6fa613deee8b78203571ea40569b412772312f3b792782d1ce48cde6f8c39e684aef7aaf85fdf51166" },
                { "sat", "ed70ecd97053a130735de3747aac45da04dad5ab719eb20225d271e9bb07550001660909a0c156b465b38491d595c65e6190a17fe0fe3d2bd2ac854cfee3b45d" },
                { "sc", "7cc7110cc63c9291d9e7098cad20560fbee7576f6a6f9036693ba0187c3af55360b11cd48e7b9d47f1f02856150ef14e09ae147911caecf2a0e62286c8c651d7" },
                { "sco", "ad8dc42c276c718ab2e99bf4508ce44e423a4ff95f39547ebf87cf9814aa68c540275031d2c19d0b4f9181434a6c8446b77d14cc6ae981e2302a6c01e0f9e118" },
                { "si", "13e652dc5beb7aa12dc330e24415fe32223c7352954e484f042d6bd9c650b042de68f72ef9e895245bacdb0a9b9b0e3cbf58a8966c8def43f8ee7bd73f3d7b63" },
                { "sk", "7e71697ca89a76031ff30d048f4d5c549b0d54d4c98f673a56612caa819e2f4e282591a20fd383a81a55827be0c064ffb55d6fd9793355154191b62f181f6480" },
                { "skr", "9d8dca664425da34316e26777ee1b54615412317abefc4c00d77178f9d01ad32196b29e2471df9034397b8d3aa3fd93f042c8dc0c760a8f95daf494bf7de149b" },
                { "sl", "5668a9ced59aefa95b4216b14ba818d92f5765659d6be4dbc10e5afc2ca4e318fa82516d24eb3374bba877564b632d162b9619ecd0cdbd899042d31f87bac167" },
                { "son", "49aba296acfe37a8afe959f9457a043dfb15922810261da81d1f506c69e10317ff0ba40165b3725349003b57dd2c3550047803faa30d4d9b220d905ef303a5df" },
                { "sq", "3d7e91f7cf043882aeef3b94b888bcab425afa547b142e3586d015a70ffbb461172b242e3bec6e6cf83dd3c34b9c42f54a621104e9f9b84eac51710ef1c128b5" },
                { "sr", "6a9014009d9c11ef2b85c85a9cc2a3d0266494dee8ae37383ec72bcde8c4a1dac5b4acc12684ba4335330b5513820ac5018b9a397932059e5fcdd31dda814df6" },
                { "sv-SE", "00866eb936ece9e4c22e4087f3f9f45712a16ab2db670e77554c102281d9982df7f215e1a8e457c47552983d25ced3a33b2074d5f74f0bcf44eb2193b579d496" },
                { "szl", "1c86a0f038d422c60808ef3aa42423d70cbe3b9adceb01d9b0335599c6c6368aaa90425458f27fc57fafe5491ebf2f981984b0a3523bb8a81a8da1569433ef4f" },
                { "ta", "ecd81652fa3c755d64214233b36a24a3d78d9a79716d75792e019afb3715c151f18866403fba7ce6619cd6d40b78b5057f8b0c6977ede33b19c4f561ff175a12" },
                { "te", "af9b45cadd88838cc1ba09b7fda3dd87cdbc4e4949e6b53145aa3750dd8935bed8ff60d2b3fcdc95bab3b1ea17f4a7bd446ec679a8c9862a4984c78f07236610" },
                { "tg", "43411cd45bc2d932ed8bef2b6fa9d2d39adba6bb6612ff0b5e0fbd94e0165be427626a74fe85b2293913e0ca45be345b7fe769361cb48705d68bc948fc5098cb" },
                { "th", "62a688d5b47043f6178fc0c56b6346922fb4f74e9ec7a973bf8b9682c2f1a3bd31537d42699f424de5f4206970497b9f7a10502e20ce84fedc3670532c4f326a" },
                { "tl", "330ffa4b442cb4d5da52eb916bbe5457cb27749e111abacc8855eda82ec57477fba82867d76adb486404f354820497b600ac18788f94e6ab466aef4fdef23ab5" },
                { "tr", "ab9f0b2519e58ec90747255a98773ce2f123a6326e1bfc0093b89d41f01a24c73a62566e2effe424021c60acaa7b0b49847993a7ba00742fd88a7259a686c0b0" },
                { "trs", "b247583db185268a95e5b8a416f3f6cedda2fff07ff446c0b806ae2bdca27ab8ded4c83f9236cf136d8eed42a4d3ddd984e6c0c8ca26da40b4dcf852c1b4b363" },
                { "uk", "a8636023b699b6f5b2f619c7b7930d128de9d8673bacbb179acb3e149b7801a7ad017ff8bf223b1b54d234f15418c7f100c5409ae971dc0b25c227d59f50cc59" },
                { "ur", "7adcdcaaec4cc64d8cb7630cbbf57a17345a9cd8140061d41348d25dc36a5e777d12b0c8e8f41f6ce2691b38e2588fca085867ee0085539b21cd9f2fcd6bcdd6" },
                { "uz", "bd9faa9803e4541e45b132723e3289cac56897acf20754eab3082a6e4cdfa84c7ad4ca2e3ed06531b0a9112f34aa0a1e3aba51aaffaa69151083a615251bc0e9" },
                { "vi", "358f2e5571b015437544739245a879fa35b33530db1a6cda3a87e505c8256e51476c12062cc4628e11cc62685171722ec7e5ce655645d4fa708e6499653e56a7" },
                { "xh", "2e395afe768b1fa2634a99a6584f411560a33c2eeb57147d6b705568cb189585aa0697159c653711a5d10f38e282aea3200fc779cdf3ea3f6c127d98e07ff95a" },
                { "zh-CN", "a322eac3fd1956fe16c3367e3fe66dc31ed002ed6b449fbfa040dda46666315072645b0359e4e447c7344e3d0ae04c08c9c8224ef9338a6b49304322dd7952e9" },
                { "zh-TW", "2950d8315a47074aaeb640b11ddbf7871dac5c5879606c592829afb779113c71fb47607c607201bba62ecbb3fd5445fd3ac9be8f3fbdd10f83a675d456768ce6" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64-bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/137.0b5/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "304ed5e4f4bad865b6f37b453f25ee74207df931bb71cebc927535d2ee6816bce0a9de06a97f86b86b9d666fd4cbd2c42202de07d910c45f901deec8fa0d0903" },
                { "af", "36abd574f349f08361e081b3e40d11789ea68b124350e901b2f6fc71b93a4e33e8b668c25f0a02a3e665c98caece8b65837c45957754b291b964aaf95e3679fa" },
                { "an", "c6488b7d20a95181ede6efd994dc8b4b3ae94b900e54236a3eef55bee2d21815aedfc6802e1e40270a36a1cdf4dd39fb388334518bffc79f0cc0897c6b0a267c" },
                { "ar", "250e184c7668af2ca17b32a1769536f00c95cf80050c37ec475e03e06e1a80a5ebf1e578357a490d1e656199ff25bb6418a1f93c90de9e8f87bbf67f2083d208" },
                { "ast", "5b2ddb2d5d54901aae303e4f65c85c1722ddaf5e38b64c47f842d56b6f1fce49e97f4a57b9f5da2fe247191fbaa10cbd6d0794224b46210ce326d25dccc5770f" },
                { "az", "7488a7255528bddbedda6f3a362690babfbf41b11ad4d81941204fbd755e8eb7755ad09938d31b6d8f355caba80d679d9767005b9199b9c530c7b061f152c318" },
                { "be", "dc2a0e0f6153aee6d08b25ff3ee808ef958a2f48cbb7cbf455455b3052f16c1fbb23eea5d3ba25182f66f8139c8e5895582a140a84937a311a4fcfe7c5abb2f9" },
                { "bg", "8b6fe5d1678279431631b1d5f51e0e3fec27ab7cf787b1efa390e6f1d979d728d789e23ebf0256ca40e2a5a7c3f10054d135c2e5da73459ed10fababbdb4a7d1" },
                { "bn", "381743d0e149329462123119f12993cbc46580241fcb6f4be1edddb03cbe67340e079e69945b156a3cf590e45c7872d01032c7757a33e6561fcd685088c575de" },
                { "br", "04919afbcc98f1d5842ca31ad057f29824a08233c41e01b475fd49a49852a57bcf3a363c75f1e6b42c20c154ada6646d71387fa5f46c1b262148ac8879c98317" },
                { "bs", "4f9c52a528fd2fcf01aaaccf32f4022d157649c555392e17b47a42e3e6cd4e04a569afe52a4af45304cbe608e1d0b0e7535fdbeb6521928978604c14f5265492" },
                { "ca", "f5064c2189f25cc5bb9240fd614b1c2cfd15102ce28b76dfc1f0f8ed1ace7aae2929ca5f18746620356f9043ec2dd1bc31e4f5f959b7de16e3b505e225c45442" },
                { "cak", "00eb34bd3c5aa95027672c44655ff57d932e9be2eca8134b46c51e53a6eb9be81972e7bfb392f44919b2da280ae44b30bdfd0f1f48ee5a6bd5b4c873b5009601" },
                { "cs", "2c6fd9f47f6016622280e158f1d35df79f9e1e3732d88273cc29c49e53c18fcfa212930233b2df89c948400cecc2030d871f4868b99711152ba16ae2ea32137e" },
                { "cy", "0641696b3822c91678b25587d48971e85312194583cdc455c8de012f7f2cf7b597bf4759de895900cbb21eb505b7ce8328c5a2db0e2fe27ebf9bb83f069cd76a" },
                { "da", "9c23acacbbef898d1ea1c03e057e42e315ad9ceeac9ec0957ad4989f7c40f38ba1fb97e4d46a381366ef675a89e3459460fa81d061184726e7ce9a1393d8e5c8" },
                { "de", "727c3d0759c389cb1150a28b519b8fc6b5475870d64895ebc229f5d913acfbde854d292c9f5f17e41e63257e6458b5e1b2acfa8b2ae4377a4a5350f8a3eb405c" },
                { "dsb", "351fbf8ccfaf2e26f92d0416abf076c0b37ce3ed7f8b4c49fa469c69474b29d22396b002f5b88b3d8b2e1b2a57b264408134804d34f5e689b0a3c395b73c4021" },
                { "el", "2cc7718bc74e6d46f88db737bf262221269a0c3d4de50cb635d6406a989e4281d39222d89fd3eaa47584f79c6460af8f0a2a72a3fefbc05b0c453e115e01cdaf" },
                { "en-CA", "a8a3b820d440a788796922adc418f37312cf21acc145d0e4f3d6c32c603596c748f40f2882f1ad0fccfb37ab74e50501dc57b9580f30810a84def5cd6a2ff53c" },
                { "en-GB", "4394d41b4babd2bc779ca1ddc807a0d6c4e03a63d24f2ec1ffbf007794d2734c9c91a33f6d9597cd64f9d209159727d47ff7cdc61ce64059717ed1951d91ff64" },
                { "en-US", "6bf3577af2845e74ce140ce70bcdc41a8dcec750713d2b0b20b33e6ca6a62b38be2b99867ca7aff385c891563237b64779039f4b996b906399d0c8e461e1c60d" },
                { "eo", "364865f4a57c3823e6805224c4a7c1cd5c690cc0c738495dbf75498559e678d0a9011d2e7bb9821dd8ce967e3b4d18ab98f56a5e0730908c086ad1845437a013" },
                { "es-AR", "a4bb3b561b7644785bb2d4ea7b027065c7097e12f70e9d19f2c6463eed6cb911993547b50bffb1b92feea8184a8098b6b303939e0e1518ddfa752d180a7090e9" },
                { "es-CL", "355950aa17fdbe7a627ac42047bf4d05d88ff1d9a403c9e249061ab81f81b72217448f8cae99f64bb3051f4eb126760f03bc884ef727682ff2626a9671eee4e0" },
                { "es-ES", "94d0969623b5dbc86b3a75ea78664dbca16d3a2dfe0e41000fdced6e78785fb5249d83ce0ce6c87b6f0f84c9a9618a00cf7bbf6269149f2f47e6b8d9839504d3" },
                { "es-MX", "df51597d7dcdb144732147ec634e2fe3b2e52053d5112817806355776dc2a56f0e5f938e1d27f33b7d57c2f9f058d9011aa6829d45c1980d1f3c7189b5822fc5" },
                { "et", "b9949b6947d5ed6d0c130d0a9fae887edb0bb35b734debd55b3ac1e59e6f329305d240b1653b2d9c96cdf3abb1696132072eaa062cc2f0d1c21bf96f2849ee2b" },
                { "eu", "ecaf4ebc87ed7f54e8c0f2c03ccaf0b5d06f01ec5b96327a445a030d1b69fa9e767ce2e79b26cfb2d5cd4abe5d623bdba68472d4c4f169df86b7a07442be8c64" },
                { "fa", "5f079be874b7ff8b9fcd60b503228a8b3de447ee8fa04e0cf066a5ded53404cc3d47db1741da51eaf97bce85982a9b4b5ea92c91712436a04c99b221c795543b" },
                { "ff", "d0a5b79bc13c7eda3cd9407c23858a2ff49222ba09ee785417c18064cc5a977d2428b73a5804e0b35989abb7c1663108ed001389aaefe285a5600ef60be35037" },
                { "fi", "da91721e963bd7cdfb657a80614797a7c7180b7fae4e1407021cd546a1cfd07b15589e2bc9d3783bbbfacc642aee886019c9f04c79b835aafa81a3ee46af8f66" },
                { "fr", "f8648c68326f46f06e1e9bc1f12f90f1125c414739790b43902b87a817ce459ef9c1c28d706580a5c40d24c4339e515c3abec12407bf805dbbda73c212ca0838" },
                { "fur", "3047b6796f3febec46aa178654368be391d98bea1e506f5cf924167f752b8f570d8610e4c13c49356e6d008b9252b80cf2002498a1deedfaedca91f5c27a41a7" },
                { "fy-NL", "4a8d76fcc3e970f529e686c2ff1eeea62b911e7e1d121cd6ecea83fa6b37ed35b85347076215fa1207e2aafa7d3566ec564fce29c713080bd1eedc8478873676" },
                { "ga-IE", "fad512cc1391b951e12eaeb29ed454f33f99c199e97aa398148851b5346e4501d9c5c6e34eaa4d152c2cef134c817ab26a3109faa8bac1a06175afbd2933e176" },
                { "gd", "5bd772290c9088d45d0f659a8bc96dc7bc8b15addf30aeaec841d056ed492d66755dce4cfe9fbec9ac42704c538f634253324654b03571c08704c6c1f06634c0" },
                { "gl", "1a3326238d81353ca5b49dda3db4444c4a8ec6fc8cce7768c2ba61d414a182448b6a435b7da4784fdf63754f5dfeda479ef1ac0651db428600c7f2b799f89330" },
                { "gn", "b27ffa5aaed85d3829494150c54da1dc579e6570e3cdcc9d08c0836235e40c7910182f10f90a3d9f265c19234b734ce04274a10c39640d6eedf47cd2070baef5" },
                { "gu-IN", "cffb5f54448245a3cd7ebc3700ed0ca2e2686ff2c534e6dde5ae0b8c7e8566bb1e0ccf41043c4cde9b76d72147ac8cd13ad9d45d0619f5072a2129c105d277ec" },
                { "he", "4a2bf37aa2849d88b4b382fe80f1e2e78196c3d1080468ebfdff42eee88cf0a5ebfdd09641d0c15612c11f8c2b04a283d61903dec9f5c5033cde73a00490893e" },
                { "hi-IN", "00838a196399dadd38aac2190272786360e2ea8a5475a026c3136eaacdae111a6aae89d68e18abc57c5b4348846407d5549e8f8fc59494ff161ec0b483c0c7e8" },
                { "hr", "6f99c05ac4f29b9a7ff4f73561e3940b9a7f15922821373449d50a4f040f283cebfec7abdaeb9d7baac34d9ee53a62e18e20fed1fff99761bd53fe062e25decb" },
                { "hsb", "4f2ca175e4e34caf9d63704c72ab1b9ece8d5717bdcc654e1ec59d41882ef42a69356139f23f460a6ca9a9f4d06519eaafd2da25f92fe3a788e32a2112e93507" },
                { "hu", "c83d8c2ee2ac16e3d1ab64a1b8be627bb26eacce61fdbd07c6b1451e0bd776d5846f7369f0c4c991de80e5cbd2d899fc83d936d77c47df184ad39dfce7fdf93f" },
                { "hy-AM", "11824add408b5394081c647e1e88a90aa3d99f68154ddc9fc616b49a584b0d6902cb98e6e03ee3b1f20480499db6189f22220cbc54133dcab2a70ea7891bb9df" },
                { "ia", "14dc371dfaffdaad0844d8bd61faf63605ea8d856365c8303f6959fe067df291ccf300b1ed36bdd90cfed58f6188d20f5f6d4a65c4c7c405fb4f7312c3f5b433" },
                { "id", "53d62481da9259e1c69332aed5dfd637b08f3cba5c9ebd9d50a587bc3f3ab5ea8e70af3bfacab142bca1ba10ae5bc419f2b91d206b942174728f5e8504baab0c" },
                { "is", "aae70f1244757f8a150e1881187a3629e9c1d1aca78e736179b9f2e2525b783407e9ae29e47d117028721ffd58f3963920ee07433a17306703542df20cf37c7a" },
                { "it", "575d50cbfa60a7c277b59407df7452243bdc7152144f48532111f12bc8ee5a5fe1c08203951ee62eb163b859bceff7ee2a72e4a56900e50794f9dd766cb0108e" },
                { "ja", "38ad69778a03ae44cadb573d7ba9712822741505310d76153b90de4e309bb701670738e383817a9f17bd64d9031813c4cdf947874392b0f29010e8e39ca50be5" },
                { "ka", "3a7d311fe0466ecbfaa4a5b9d2d39a65267c6b6b758bc511335607e2669222a8fcef7198667418e04d883cc2da8a5fd5e6314cd2854cd38318dcdbd4dcd4f0af" },
                { "kab", "d35c6de813646f137d1738045d015374559d66e449c753a075bdcae4b761ed675790949f0ebda3bf85306ba026945b139e57dfc5c7aced5f5cfa6890e442a439" },
                { "kk", "d0e3d7ffa1ff0ecd546de084226a302ea643b1d2ec1106d119e1c2c4354e0a723b51966b19ce2cdf994cdc05397689869800f55ebcec13a5b0f70ba1ddad8605" },
                { "km", "5578bc5176b088cabfbb62b096fcbdbe0a61ded3e3b507a3c0e1ba9e1fa8b743d6f0b76b043ddb13c312cf76c9a5802a870d57788b7a016bd6605192a8864425" },
                { "kn", "0e311f2042e0dfe1a1f54b382dc18e82462fdd6d3c44e4228e001a3ffd8fe990304e6d5413793f337cf539a452133ad1062afa3cb9ea810820127cf78f65aded" },
                { "ko", "77ddfb232ca848faf5550291828f9d6ef6001f69bb8b7d60c3843edf53e312dda4ca6dbf2f6859b67acf0727c0d1cd25db008ab149ad3d0534628ffcdb4234e1" },
                { "lij", "48dc615f9ea73130df945eda4ac94cb871fbe5f420a33a1b0506ae7cce0228d04fafa59d8bd722f4790cf9e407207b34525da90353a7f30e11beaed253eeea6c" },
                { "lt", "3fd79ff7ac7406be8f78bb9d244f3f9a7a315cd6300e06601f51fff8ae4544b02a4f9595a9d0d4bc6c0e0041ca869bbb28a4feb9af527f4133e2be1d7a71abc8" },
                { "lv", "5053b87eb0773da8ef46033f32fad80543e98466af8ff9da8268cd0ae5f35ea997de990c34435c10025995393737a9f0c5c1f27cfbff8d07f16463e74af02a00" },
                { "mk", "a8683a5838284c0478db65d54c5820c9b10b09a3fb27d3e07ac4421b5f7680129cad61f7ed9547a7e31248c809896e7f57679d907ad80600f0d3cfd14ce969f0" },
                { "mr", "1620b0614510a5105416fa3316926ffbcc0d433d278ba12bc6a9d38a45ddb66d788939bc7d013ee582a84e8fbc3f916fcacdc76dd197cd0224441b3071381283" },
                { "ms", "2c8e58db14b7b4db50f6dc5bc75ecb3df1b4d9ec58b5074a261091db6eb8b0c857cb98b295e8aac01fb4f9c85c2db8b6bb87c258199b8572baca016d8bae44b9" },
                { "my", "56951b4d37af20df92c0580ccc1bf5c984dfdbae20cb3b186f71f3fd941887ae470bbfe3adb191794fdb0886fdfaf567991f7796aa47ccd5b770ef48a38f0cca" },
                { "nb-NO", "756d1640571b50d3a8f7a04d4555f8e2c498377b06dc86f4a1a6cc97792ddb70a8912bb45146e2027af1444eb43a73ce532ed0a9b8cfc72ade2ff42176034a0e" },
                { "ne-NP", "42e71ceb4576bae099c668f310497b6f5593d4b933d0196d92c674fbc39705407cd7170a3c21695808cf6af9cf914a9d63574ca19e0a92468a6af8da87e39d56" },
                { "nl", "3a45578593856d96546cfd574017c7b4e3e814c3a386cbf8a48a621f259c8e9ac2803d5e863e8dd0f4e410bfa5d4180e54bd12b31a3193a318dd2dd5eae1bd09" },
                { "nn-NO", "5fc39d1e28aa420ca98176416f06f6f6fc6ab6c7aa080d029d9d947b12069c0f5ac6659646c455506cbf06bc4dbbc6189dc2b0efb31f9412d9d21e4762a68e3b" },
                { "oc", "d99aa63eebf44654aa4beb9b5e1c6cfed890ba521d99e7e2c218726d7cda7208e650cdefbe7db54ad5be55f3b40ea56b9bcd74bc5317ce39ca9c540abb172db7" },
                { "pa-IN", "99a8e4c9b485a94b2f5df7a4d2a64903febe9d8df36b55f7fa1ba1dedb4d16965ef35f1b1ed5fcfdb5c67d7740a0012d4387ca98a630fb5d3bf3adbdd560b0c7" },
                { "pl", "8874dcc154a0c91afd958a6137409c87b230d2290473d71fbd9846913a7937190c7457bd04018191f83fe3c057f8d7246f2125f6a6fce3af415160b3ffe3b287" },
                { "pt-BR", "46e2b9a1ac4a2f59010d52dd1f01c729684517410c509002ead60cc123b14802de6020d6eb55c95e5ff5241e1180697e3556b5b3fe7515e0819122b78b89d946" },
                { "pt-PT", "5f0238a8bf8ed0ba798d97f67a6a27a07a243225a85a8be9af65fc9c583f76e40838aef361fb4385eecd0e4e24f631e460c2a117b0bf72606d4eeb99c79cc9cd" },
                { "rm", "86894cc3695845fc8438d78b4451b65d63e0201b71ca3d583623ccb3e1e8d8fe7de7500811edb0431e2ade2aa9a77c6f19410c11877266cebb2d398d2b5822e7" },
                { "ro", "f06e6c091a6cff2fa1dfe86872b5de207dc5a5d63c88f847ca187c06821afbab30d1b7fe7b33414e2d27eb2e92965987b616af41082790adc0f933ba6c16777f" },
                { "ru", "eb5c3db3236ca4d74cacebf34b1bc28ae124232316a246d383d928c43e1e6843b12563a5484ffd90cc2e6e797398b3c85ba637612f8bc7d388a57f027e84e8d7" },
                { "sat", "6c0d320dd01fbf78cad2ca14f1b61a02e00e5565dc9803624f58f8c35bfbe8d843d8c1042bb7cf50412424cb3b63122f51bf14376f86842f15c58ec7b9a19fb9" },
                { "sc", "c6c6786a9fede815e259f0166922e00fd1d8a52310bb8fbb0bcc9dda7d4d8d44c4e47e4042eb3566d69d3be03e781d7ff0bd438943437a5eb5ea2f9327d6fbd1" },
                { "sco", "0b2f0742557a34da318a94287317e943fe1c4eb423b69e62075c73d2e2950c0edb35c872140cb1ce07ff9ab7c8fc032866f56d7dfdb1b4974f0ccb934b42d505" },
                { "si", "1be5ef4f8c1b438e706640e048679340756514235dc1c9311a25d97aaaf4592a50f3e44a61cdc5ac777063fbdac88197ed91fae38ab96a8b4c696a9266768484" },
                { "sk", "27f79e8f38901e16fe8183618a625f78f24957f50e64f74e80f59104884f82e688264ee8b737b0f4228152f2e3305f298e3b218321e0c53f484e2e63ea724dac" },
                { "skr", "22d7304d6a27e20e7308903285753cafb7f31b22a0a5ebeea8dcf523901cf738bed2c6f8852c6f11fc63c803f8bd37e54f639fec5e80bc3cea2cbc2582b637de" },
                { "sl", "ed649030f87dde926c1402b6549d605891845f64161d283b87f26c1212cda29ee695fda38ec7de19f0ab9a3ab88328879f97578ee5cd1a8b650b4329df5f344f" },
                { "son", "a115c45aa53ba63de6a7067fb93862f6294fbc5ca1fd55d11f82dc890ffe1621dacae3b7de6b5f8795add7a0c8d8835489061dd22b7771a7b027d3f9671a69ce" },
                { "sq", "a3369082e2e88640b5d130d0a2f99bf4fd67be716165254f726ac0d900c10eff30c2b12d446f574fff32e0d97bc4a25d6cb8754ebe41bb384b260207aab3a040" },
                { "sr", "7419ca078068c7e9b32524e0552702fca345dbda04628df42223c71ef09a3a822075aa750ae959db074de08f6048cf6ce619c05b436b31564bc42f923673da5f" },
                { "sv-SE", "e6b62e04c7212bd5668b2a7323ba536cc7ae65f22a6bab440b8335ed0804798bb36337446d5bd50e9fb556e3d9730970659e2a36465d09d58b8ead1eb6e63d52" },
                { "szl", "4086bd9fdb6e90544fe4b390f8c8b3e5f33754eeac7c4fa3787331cb7ed1b20ca6d7bda7bbf90872853f3f9e9781ddf1021a61c4de4b174151d76f95808e3af5" },
                { "ta", "62478634074068cda5b9ed6e73e3c6d60872a0ad036dbeed35cd56574582472141b80dc18d4f673a1d22a7e1f63217ce187c2f79328ea8c804d58733da8a121e" },
                { "te", "96dbd5fe0691d235a4005b105c035b52308d711eacb33a6d8e9fe788511ee8fad1d725c56efb384197f528c06891a38853bcc57f672b241ed0dfb22e1d4d375b" },
                { "tg", "548c77d502b98b0bb9bad15c705f14f5832e6ee197fa688e5052cb1df140b0a6a6303dc881f24b74fb52fcc2f18ce81caa8b0ae240d706954fbadd621612d8fc" },
                { "th", "e636e6c8e93dff0187919698eb7c6f3702530a22cb541cfdde6d79d4375a5ab1cb074782893702f456ab446345a642aa2494ee62eeaf33b9b284b201ae38fda2" },
                { "tl", "7afec0e34b92f8e52f4a571635b4ec98f2c7f9b54f4f6c1f46a87dcb7948940105320eb3d6ce7fc69d8fa0620947f3afb9a654e9b0332baced4182107d9ce2ef" },
                { "tr", "ba87d31de3499cffe89487df5138b11d00b58d269c89f6afc2024fdd3c6c9e1d8b7c41e3c1f4d1ba8744fd86307a14bea0bac5a17749dff823902e84ff499a10" },
                { "trs", "85fb3cf0a670c2fb03a25c2d44082c1ff615838a09d87715d276d0716c84276964498a2ac193b3f44c87668cc238eec769e6dead8738f04ceec455475f097cb1" },
                { "uk", "6e71c7132585bda4c19b0c4a3c8bde86271653a30290482cae0327fde370efe4ef0f8548a9d3eaf18ddb8e89b75c0f55e8e67fb6498965cd6db9536fe878de0b" },
                { "ur", "96808acdced17742d3d23c5258440bab294ca2b1119c1f9af0d4234481583a556df2bb4dad623182eb8ca98abb07e765d64e9d8dfd16a2a06122c93db457aef2" },
                { "uz", "b5eeb455463052d199462e6078a29cb45f1ac021a2096abc859b3332425bb06de32b5850136054a9fa17cceffdba4a70d78e861002f58c6a2d8171c95f67011a" },
                { "vi", "f08058bf5ebadfc577e5f71eadd50a805035331d6d121f5bd84071400b696e9623ae1af243376b107649fb9769c34ad691c088630dc004ad815a24daf1419d57" },
                { "xh", "63d41361df3be9fdf5c624f31dd5a504f571d1a2d802ecd76dce34593e5ec283cb0f434e4d918225e82384322ec2b7bca126b496175c38fe753cf647d73cc89b" },
                { "zh-CN", "02fb6f707a8586abb35c7be5e95d1b8f773f05e94cdbbb7810e256561afce976634da6958eee1c62c93ca499163022f61c017c094f2efc35854c9474d252e8b2" },
                { "zh-TW", "0e84b50bbae1918b0f49a965d51f456f239d991a5788eea84f0b1f6cf1837599c352dec74512579fefc7f277b31249e7413aa082c5a7d31264dce870bf2c75e1" }
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
