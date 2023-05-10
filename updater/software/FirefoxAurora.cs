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
        private const string currentVersion = "114.0b2";

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
            // https://ftp.mozilla.org/pub/devedition/releases/114.0b2/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "f6b6982143c604d94f17c3be28aff71639b552792920a1f796e4ff5c5e7a6fe0ea9279ee47e30aecd2bcb3f7eb650daa5fff144bc867b03e3ec3494f22b5a917" },
                { "af", "4a92f4b92bb3d3414ccfa6f99ec9d51e9fd79e1594d5c90160f00755e64b9e917dbd745350185ed07f06b00508270512471f3f715764134d320cbdd38af532f4" },
                { "an", "68b95513bb0ecf07e71636a1b6a887a5c4424e29150e24149f0462a718422d1b7a5b1a3d981316ed5a7c468c7d638e0536ca68c9356d6a7c537c795cbf87130b" },
                { "ar", "b1dda96d78ea2d40397a577d0cf5f39d0834512ce77cf4cb94638ef22f369c97f5ae625e24b8e70fa5b62a174b9af0a52312a9d659d2bb46dd057cd6c6c5e59d" },
                { "ast", "e00fcc5910823bcc13c390934504644ee3b436d2763d0e009737811808c6bd01dc690cbcfce76e0f27997f48ae6916d006cd36f0b607da05112abca2f4f4a20e" },
                { "az", "2b9faec8a5c6d34493e86384e34976388823989ccaf5804fe6586d89fa45107d0481d0d07186f8f4f00094ea8aa63b9d93d4fca2aad4bdc84f17263695ed9561" },
                { "be", "07520e828346cab964094b507bc32f7248184d4b13db4eef1e84b821a0f4eb87f3c040e5193671fa0c578d30f941aff36003d2b69fffeb4567ca499b591a7b29" },
                { "bg", "213bbb1ed06c88acd1eed04bcd8f3c9b7bd10c1f03e22aa2f9d0a76b1a17ece8536235264e6acf8d67c939dad5517bbca4e71c34dabb5027c5ce78cb4e7b20d9" },
                { "bn", "22fc320e7dcc5f5aaa2feeb48e4e4e788e36483f33d8bd70fe3ce08516171ae961cc9362a75ad746361bf8f09c300aaf303a2cdd40afaa9badc48e158c639728" },
                { "br", "d72f43960101e26ae99594739276ce1f6d26d12324e6a4c909a7777cd72d0a7a5cc1493feddf891bedbc61880ba26339b220d39d5a68b317addc85be6557de0f" },
                { "bs", "1c5f261669e8e9009d8b7b87dc848b361b1139dd0280c93ecd8369e82ab4efa23248977efbb2ab930436e77bc5c0f90616228328194ff0b103bdc3281db2d9cd" },
                { "ca", "e61302a67b0edb9021e794f4523a8ed174f7a680108a6322444904f1d10f858401cf22bf7f6654d7388f8ae5348f41b769928c4be8ecf44f383ecfb8e69a2efe" },
                { "cak", "7e0693f0d713a6bb2e2ce440afcb0f2d3c1eeb33d3e9fd014d83ef304425e3bb930dfa0a8b67822d350325a05fe9ed37622fc480dbd39cdf91be53d9ae85d204" },
                { "cs", "f3ced9137bc5753a56dc89981d873b8be7515b2a3fd9bb208a41e4a431f2a0e4710aba7474bfda5c4b7caaef691919e573104744778053940acabf0213ddea7f" },
                { "cy", "98eac4a3ffbbd8d9c16a5b20a67481c411775f6dfac35a40d76141e9eeb31ac7bad959f55a5dd775b98d59c136b431d4e61696b08351a4a81fbb93612b43fa9a" },
                { "da", "c4d6406ceda44348a41962919acb6fc0edc0fb841ae8ceae6508e5ee9bae42ddb9b44fef35c491372175e2c1b074cb2521d49a406b416f972a724bcc0d40581f" },
                { "de", "de7b5a80deb0b746c4355b5ff34f9a68ab0f09c6d8639ad0603232f865963530e281861cd1ae6252fd9b9851c6ef4eaea424ecc2c30f841e8513165702b54355" },
                { "dsb", "2b028ee94a80214f736fc9e1302778e42a38a6ed8fd0121819336aedd4e5c28e5b7214acdaba80bd258d2d06546822ee5009179cb1262c6f99b7adddd0c134f6" },
                { "el", "5e8aec4915b1fc28f50b6d496fedf0457b75324d6bb312dae18fe3d28015b3a67f3a0c3ac3b2c55c69036f323504e379fda004e7fdb8f3ae603a5655893b4e3e" },
                { "en-CA", "61d1a49f4dbd97fd37c5bdbb89d522447c724671c3907ec4a7834c332fdd07e89acdbc1c555a2802fc7c114815bf820898aa2b621a2c76fa676e79b87d1baea8" },
                { "en-GB", "e8ad5baec1c9e4e730c4df0ffef207449c9f918825d7959b0a1634bc77995557efa60fcd26fe86a615e81fd8370f7b6fe93572115b45e02188172315f5a82d66" },
                { "en-US", "c7b656430d9ae24702170eae6bc650691c4d21d23632e89885d303d6bc055ede0ad6011c6a1aa217bf8bfa7eada5d47268f9c0f8e4832960a2e1611e1a9b6807" },
                { "eo", "26693d1f7337c287fc834c987ffb40fa755650814816154a1293eb466d906791fbb14d52a7a3cbccb81e41a69c570a77b9238efbd8768b3a717e499c88a8dbdc" },
                { "es-AR", "b5d64ded28cf0c9caa61f97f4d81202108786ffad7801a2d1a9a276e99ae252677b57742572e8e099e33b4b5fc4b365e4eb30e28b1a615fa6f37b88d7b916eb3" },
                { "es-CL", "170928be08e3dd5d4e0358104426c6f3fdac2970225fa28a1740709fa69d01d78b5d04728e9a1895a9378531e9dff6f3892e9a56f58616314437fdcceeb42762" },
                { "es-ES", "648b4f5adc4dc8868e2d9109428ea831aee456bf29be5553c92b96ea85db73e11bd692ac30245a8e7797e491531e4b5adafd12c040212236a0acb02806c1673e" },
                { "es-MX", "c8ef0d13d6a1018281eafd8b1a8733f8f79c7dc311c6274f640f49b2ac5f01b6e34240e03ab19a2f1fa704d8eb51d41daf92be71b89c0461f6c770efa509c3a4" },
                { "et", "299347c2f59ed693093a601429f3edc8e1d9848179b23c369aa4049b631a359a691a73a01d01a8cd9f0e8c9b85b26b2674dbb7317c45770f03322247bacd5e07" },
                { "eu", "fa355cfcd5c691cf872dfbada97d14d1292e656af909b1a7c1f05581f1567f7d13cd25883a046799ae13143e80e060fc49a5d6b686d5cc206a8ca8215b83f3e4" },
                { "fa", "ef1ab9d2dbb8e20ca04b2d3d1557626155a2e28c981fce94f7dd0ee769c130b24c461df7e96ea384644e3aecc60ac726f06b2491d2a24a53e2bbab4e8f68572b" },
                { "ff", "b97de4bf9c2cab2be4a46a1203c185e713970fef5a9fa3ac3a8261f5cdc9110d85a54370903c6e3a1864074832c3103af1d62e2edf149311e10d93eb0332819b" },
                { "fi", "5846758fb373e59b936a801137bf00cc0d3539440791cee5f549681872c9f40f591fad6278f2a0b1a13b9e3b5cd2a3bcc1cb61a8a0391996f8069cb0e1682d6d" },
                { "fr", "9ae2461fb184b14c62f9061cd3d7ef35ddfb5a2fb344eafc98df00f049a83113a1a44e516bb97cd5477c86484a28293637995af57224bfcb4a8b81a6797e5f01" },
                { "fur", "4143648f3686cd1c1a69a298da1c5d0a262c9b4372db938ebb36762fadf1706c067d688527c3a9fd67bf3ff9e6a4e748404e261e0af33e30946323ef92cbfec5" },
                { "fy-NL", "511a509decb805a5db7d1f1a62caca314a0af014a9f1de9dfdba3aad949fb8753d613fe56b8299867fb31c59ebb81be322960a7facddada8a7fd675d11a3713c" },
                { "ga-IE", "bde895dd203299f3f22a35f0dfbef502168c037bb2885c6b5f359d60abd6bed42228e3a2c91b7d8731f03600cc90a6b2b5177055731e8a522f79d9f91a23aa2c" },
                { "gd", "922bda84195dad6cdc3ed9ff28942cea610b17dd5f59e189fdf2c1062e44942abd3b9c3fb50a94e0694369b17884cc278a59a454ea298c3fac46e0cb3700ca3a" },
                { "gl", "30eb002f6dd09ef34cc70d74cc7db1a24aeac78e18aa61f41555c8f0c224231689891ead0d110cfcf3525ab9795631c531151dd733ad661a9eaaa4fd07a1483f" },
                { "gn", "cfe81d4ae4d78eab15310d6b1a70809bc47f368e21cee8a65eb159bdb4f8d16e9186fe3c2c8ed64993a7bab21f7a9f2e52d97bba1e8e09e4dac9a41c8f21af1d" },
                { "gu-IN", "6c4f00758b089447cbabc5005fbae50ef0e95196acc6a61fc87b834892deb0ffbf5716e5b5d48ca56bbdcedb3b8c19479b6cb10ea32ee41c69a82bf01d6a3312" },
                { "he", "ce661d93eddf27e1a689d329e959cc7adec63f84c0bf4956bdf6cc0ae39fe0972a3867bc0ae78ac2886f5305174f850fa1e4e206f5efefdbe4866224cdca03c5" },
                { "hi-IN", "e0db4509b929aafcfa26cdfec59062510f23dd66861fcfe21e233e8aa331301eb4da97d25e08e6288bef2ad2f6575697eb1ac888e8e8f5cb3dc2a57edad5744e" },
                { "hr", "ce22323a707c04c42db1564504f8a8443c938e3da4eb56a188c3d81efb9fd12c9a877f34192d9f1d36e73aa5f4a6b03aff3af30490d58208085b4c846dcd300c" },
                { "hsb", "a143bac119af0a3a9b28301fb9d1906d234ea0d8b267f3bece8169c5decc0f27cc904609f07e33978d59b2030c1b399d4b22f8ad198283dc8b301130c6e5bcd7" },
                { "hu", "da881c1f4520e3cc23d2979bef8e86b30b8fee1a4f2bfd1b2412e1c2150b8ae84bf6a2a04e9384f426d19a0ffd0d6852b4b11104cf443ffcab2315191e14e605" },
                { "hy-AM", "161855d8a15d8c2582c5dfe735b64784069200239c5cd0120027ebec0ea1d703d6357a61552d705153b157d30ffe92110727e7743fbaf958abff220a6d05adf9" },
                { "ia", "5cb7bcaf15f7769f1e626888665e8b33063c6405eea3233328fb44a442e1f386403535b8de254568a61ac9e55085edfbd9240d40bc429eba81b15ebf16c68602" },
                { "id", "4caa5bef006e3333e71890fc70826531458796356a5bb795092cf260ec1a7314481d2929c025a4424e23f7ebee1e932c9f0ef1b0b1d59cadd3bada60adeaa471" },
                { "is", "9850627b2ff007d8a5c839a951803204dff40f6e6709e092bb5fb0cdd237849d92fa7bea725444d7d14949ab5d5c6dd00e62a68b14ccb0d327c9ca1ade732612" },
                { "it", "2ac550556baf2cb1a0cbb81ef74573bde070109c365b26dd69f2f213d27e4ffe2555f15825fd4c8dd140002cf8ee21560716623b03ef6a95e4ccdc4eb718ef73" },
                { "ja", "1c2bac9382ec78e3d9f54a8cd2e9c4babfe6a0479b9bdc9d9a8056f063359673394f271f44a5adeb742c7b2085be9ae31332c657d8557debd6de75d89de62126" },
                { "ka", "e436a5507f7d9b6271e6ce81b7fecf1d42929a67553c23303a9fbb920f9b050a4a473a209222492f5ae34dd2097f1ef868b897044f38b54d234b90e1c7c6bfcc" },
                { "kab", "19c80c7936c9d2668286592dd79ccb78466061074e4d6e2b9026442f9c9312928c50727a641fad746174f41527c5f238420d3f8e0e1a4aac164460c80f511fdd" },
                { "kk", "acf182f300c002a2be56db6e02ecf8d08545ec370468644cd09a599c067ef2b859ba11ce2345df3c2b7fb2ba33f0a4e5f01cab24b46a8a40f06bc3a5515b87cf" },
                { "km", "13fd7d2fb9753300c8bc47a4a675c85f45bfd73a1e8b23f18d48f5a3002b498208027f28745850046d12ebc297bd988874809f5e8e22e44b55a3d861399752da" },
                { "kn", "48c53ff3b8afb003fad4e0e089ea5a1a5a4f5d82030d906572475b2e619635fded376c8f09e12f68982df48afe323cab1a7770cca26914320a70a3dc59b1a22b" },
                { "ko", "110a84ff8923e4a476c91dda96aae4da5ddf7320f6c005a7219d7542098e7c4d5d85bb5a9918a21855cd57af7796833d6b2e9929ed9d8e8dd7bcaf327a247c5c" },
                { "lij", "ad7064385dd30c58bc983becc5c051599af86f32bbda51a9bb686cf87d87fcb6f8c3c478dc85074504121a6b2fe083699131a0d9b897dae1fb81399bfcab16fd" },
                { "lt", "cd552391f6d789645500854488ca3068d87fff6dc3a1726eeb438d36ca261f4f27c4b52e7e392bf3cb724caa7ea087ebc7d4eca58052c5e533520c9d955cc295" },
                { "lv", "151e9c4cab2e1d2c7ae34a4641af6745b8dbc5b48342c71caa52aeab70c13412400ad8a6008d7f2b4cfa92721376ab48e5b71d6dd8ddae0e1e9f9c0e8b4a29e6" },
                { "mk", "d5b0c4b9779da97db26912558d03959e25911c1c83d0fd8e1919bfc0065b46bb27ecfcaf08ad795a93fe8d0e25c3ee9d60cbd29da5f5069561493ebbbb68bdfe" },
                { "mr", "3919b768f28831400976f97b816bae199a394a41801a0d5cf2080b740c4b292f407ffbef6585c78a519b1455118616accbda476e6912a0799c6273d289619fdb" },
                { "ms", "7ad22c2d3cd4abc9f63dcd2ebca0301c29ba3a0548e80672d4b4c3d185330c917b20976d4fd2d7684b5fdbe3241c75110df6e085a1a80ae807772c138eb3895f" },
                { "my", "048cebcb9d76e98b85cfa478ba3e876e7975ccebdec07394dcc11b5dd6c69b0d763f0180d558860ef04775642035f0be068f7e4733892ca71ba0257553147bcc" },
                { "nb-NO", "c8acbf742aa68317d0c0e757aedd1988102def1adb62858302380be713f0fd0829affe8abe37b1fcb7a70dc385f1d2d9eecf87156c30b6f2298c6c624c412ab6" },
                { "ne-NP", "c89c3a4ad5fb4adfaf9fc20a1ca9ae4b267e894d88719c99ae0db45548d17a3572d4db94b8ec04f09fdb0c6e5c0381fcf4057820ac37f6ff0ccad609bb562642" },
                { "nl", "d1aefbe95a896e721a7fff7ad76ef789c88a6d5ee973516bcb73efa9d9b0878e4031530f6949d0266a54f155bcc5879b11d315261864859ef5c40b533b7b97bb" },
                { "nn-NO", "57279d9c8c41fa3e84fae9d527f19b93e6fa816b4c7d5ad028f40ff095bfcbeb8d387cc80f5ca29176d105f8bd1f2dd84a03fa4c735863b48c8f1585dbec4e82" },
                { "oc", "b8d5448ba8009ef717b0ebefcef6ec12a4b310c362e4bb8d1a3c3d58c7703c8e97ad3e0165a643f7f3f2dea9d92b210ea4e73632fa630f50bf6de724a5085a2f" },
                { "pa-IN", "0cfc4906aabdd592b29317070d9a648bb9a6ed9515f76c7d36db6d43be1743fcee8a140502a7747df182a4fa850483c733c0103662ee4411d3cc5bfc73fd31ae" },
                { "pl", "078d860d16316d20437c56650da135b44e73214d643f806707d5af5804f447bf4d46e7cefa44be1166dd32ef857d1f5aad26c3ba57725d5cc6eb88e906b57284" },
                { "pt-BR", "8aea4093838bd85757535f4275ddc924926b4e6586c98eb88b80272dfb56243900c2f6c11e26fac9a10b6552f9ebddd6420af2913f126fb5afc3e5793e6741ce" },
                { "pt-PT", "b6167ee956b57ee6c17e362737d3f6e299ea10b3ff96c966510e53685f5df14dab25e9e2d0d5b517c5e3db6cc0361f8c7a0d30f45d1f9367d846f98c5e2d7a36" },
                { "rm", "a712d089de50f484bc7ff2cc0674d8a09f015f4416f7ed24b5b18acc7b036e27568f70d54cd2df29faddc0e5093beda78741fe4d46ead42f4cefc9a45ba11591" },
                { "ro", "2e40a5aaa9bef9ea7985d368c63cfbf7f619d5955e2813e7aa10d61ee80aadf145946d344270735968916bdfd0fc104e09aa0f3de66a3c6db7fd28715412c8c4" },
                { "ru", "cd019d3f520a648417fab24edfd67f910bd80bf50f50c3af45685d8e6e037d30cf8f39b68bf79fbbeac395dca9f019e890f52ecf11f6e59efd68bf572aa8566d" },
                { "sc", "ed1feccd1a37435c951747faed6e0b5f8eb9cdd439af4f933f4423174c51425907dadef1d48658a38ce2a3e2ae6f9da631a919ef359fbfbe7cfb21100e77bcd3" },
                { "sco", "aa44795851745ce5a68641c18d1cd45b9cf00ef20a3f053abce0e4ffc8ec9878761f7e5edd5a8f00788c78d09e3002a0d58e7ecef8c111de5c519daafeacf385" },
                { "si", "63be55c8163df9381eca40bd3ef20dc7b57f05ede803d76f0ac61c13356177ab9a969ea120f07d20b2d014a367c6bdc77c8cb7cc95f14e17abb6af2742b60da8" },
                { "sk", "d41675ea868f35fb467af965beb229fb5c747db58576bbe914ee74491884c68dc44d71296fe3cdb7c669236eda975d273eac1bad6423bbec6b03bac62a1cf551" },
                { "sl", "dcf2a5a2ff1457d3e476748407ad92289a64ad3f4d85473e223d48c2c4aaef30833c8aac2d41b341f277c2fa69f4a07dec712c1901079c369cf6d4236e9a80d1" },
                { "son", "d37cbc66832c556fe68156f043f35a4f2b29ec116e17127632164026dac7ea4bea820251d5afb82afec949ab2881389908ac0ff7c664d3852b59bccf1b76d766" },
                { "sq", "f6f10760b632d0a1c3932e5f6b53ec2151a4467852f6f8a2082ae50a452069fcb486fb028692a6609f5632b7c3fc9d4bdfea6cb85deda82837ed09cff7945605" },
                { "sr", "c9ad4bcf0e92414002ab4ccb9590577bc1d3eff6d2961bb847301d2bf9934732ddac9c1f8f611cc449dac12251270271b7c647640ba399fafdede091650bb206" },
                { "sv-SE", "ba9261e81f673f13f04b07803e324fd9004f6391d04ab89308d6a47c6b9d428bcbbab4e431a7e5851c104f8aabdf75417bd1d1241229bcd67a80a85378bc2ab5" },
                { "szl", "49d7dadc2f8ac17234b77f103e962d9c535001df5abd66a180863dcc7ffeabab86b5768ae8b5c008adc85d0dbeb3dd37b8eee2ed49318e0dec2f2ac7adb7716b" },
                { "ta", "91f518678c341d44588e54058f20796ab32fa5dc54f2dc92a9f0b836b9d043a071a50dcfe31ede4a5d79c3c50b58ca23beb82ff75f34e064a703d352583cf640" },
                { "te", "f5c404bc05106a5dd9c8a43ddf1e40cfda6d187ab80c274b608af80bbedd573eac0cc6931341b5866b8fb5ff784d8fa04cf2a9eaf2ea2a38568c89490172ef6d" },
                { "tg", "aa834997ea5b95db2f4ad6efa6f7eb6eb41deb8d0bfb9ab2f4bff1155b8e051b8a7738df3c644e8e7e839dcd2f0a315660a644bce194a7d365b4dbab11627264" },
                { "th", "84a5196b78b5a36511670ee21c3a11976e2965fa6266e1266505796dfc25e22bd04293dc4821895cbb42015c2e5a70b0bdfc9633ed2fb3561f1a3766c23be4d9" },
                { "tl", "de89e1b5d8a69a98784840991d8444cdd180675d8f21e119b9339bd3a655840d559c8826e056ec92e6519282da542085f87ed66c9f9addfaa543043754a6f76d" },
                { "tr", "d481b53c3293c1b502abc0b6e88f51622aa5e648af670f4964bc6366488c996a3f9686360eda1a93682203d1a5877687961b53672e9e2a94b03683942ca21d31" },
                { "trs", "23c446292ae33adfe832b0dd80841f755cc07274492d20db75e57eb0e7d8c1f8feaa85f581a43cd43895ad50b4e48dd201637210ab0f6f753f454a90a7716c4a" },
                { "uk", "173ce2d8abf125f02ce4bf0900ea99cfbd39e9b607379ef5db65211a3f036d32467b9eb33e1643e2a6845f949e7b994bc8885f8b0d2eb925b8c89f890cf6fb33" },
                { "ur", "f8aecd6a3d89edcc9c2375aa940950e327d06774061cc4f3edb854af36092759b53b2d1fb4632b520354a3e288989e40e34390d1acec6b34f1ba916ed22143cd" },
                { "uz", "b340db4ee1786bb72b58facf5878fbb06751ee996e458a1429f339d8d042e249cb51e58ab495785c197118386d0b4d951f7dba87968f90655d767de4ff933b0a" },
                { "vi", "c61f9866ee71185a7b90866bd2b5b23046526c2d8f2720b74ef41261b62da0b3ff8f1601168447182483225763d66d11936ef32648b34d943b2e3ac399517630" },
                { "xh", "659a56788f775dbb2e1026f085c65e13be8a157026d3169bdf3e74e9b1adb5f606cfcd040748440c8e8596a865975d9ad469601db6c7f50dbcd5b2b4444e93d2" },
                { "zh-CN", "95a3e9252a92eb54fd4b0a8b078d073e77bf1663b8819301789936ff63648c0c3d6630e7dafd07ea360d3ad22f1e3e9cf26456a15712ccad7d98b35fa234a88d" },
                { "zh-TW", "0eb0917ab3873c4e1bf37f415148bfe5db7bc4451543367a5895ff3405b74de4f3d6a83e4f23f98ff6bb7031cb66c4593f6a2180ac1fdc4f74d06e850ca3aa07" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/114.0b2/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "4b549655655261626f08715e6c2ce9b2a29fd6ff2ffba13c356536533d747e2e3a283a1b25347e2746ab0f1a3c397dff6b9926fda6f367ccc2a7bb460edd448e" },
                { "af", "dc257c51cdf5f5f24614f8abfb78298326188c20aff6be48968d1c503db6ca122d8a7a1b0fe2b9d56d3291808d08de3f454f6a1b77973b31261a91a11b5e9ecb" },
                { "an", "5fe764e9a589dc08bc073999a18c57263c4da3e24c65010564c5b6e556796a35e1f3c18793b15332f3c8617dc01b1f4da358cc29e068f4f6c8b5ec4edbe8bfe3" },
                { "ar", "c40f9fb8e956a7f765be221acb9aed5b2703976d61caa9269cf14c93cc568fa625c1fafd34b2de96886f1d3a3408f018b091b1426a39c951f71cb69e9fe45d3c" },
                { "ast", "74889b1a2ca98941d1446afd8e0accadfb75c2c444c4a959aea015a5a10943a401c67b7b3244ac7fd49ebd4eed6592a96eddd9fca0b7a36f9080a1526e643c4d" },
                { "az", "bf94ff0d88ae2ac12c97c78da50be78fbdfe843f4d293e93d8dbde011904dcd141bd293bcfce815ab79a760c44462f25bb7fd525773ca7bb226f03605fa87353" },
                { "be", "0921a352181a77810463d5a908fb4c0dbd8b2d77b43f1f9481583f7ba100609a4319ff5c1e5671ca9f43ed3c8f355ff2ae3825fed53dae00da4a2f62b688d28f" },
                { "bg", "7f887a7a552bdd0a096452c21a22b4cf454a1c7b91c4fe13c079b50d1ee8652dbc91f5d38679b9bfe2b2418141da118a87df8960517a0ef8302ce890f64ed456" },
                { "bn", "09466eabd275f46ee2f31b9d4e5f334ce3cbfafe402ada7f8915b0b7463b48e62f2aef242567eccd28ebd785314419c141dd6156915540cbaceb14123d40a4f2" },
                { "br", "6e4e712f68fae3b3ca041bb0ff8d856026b090277db44ff92c5554f6dbfe0deaea7e498a488506955fd2f76dfa1b0fe22bbd6ff8d55ea9e0a17886b725d01a92" },
                { "bs", "4a3b87443d8203c14a8b0be5140e910f437f7cf48e6859f66c9722ca8cf6d81762e1a5b61b127dfc6e2073c02461fdb1c8792008d3aaf98c8cd106bf4464b1dd" },
                { "ca", "176ae24ff096a9972904a952007ba9bcfa3f30dd54feff72d6da89af1d217cb0d6e0f93ec4c6c457fe6baa47342ecbc4612df1615f1ead994839ec1c0ee37fc5" },
                { "cak", "a03425f791f6c2253cfb2393d7e31a75f1df46901d29c82693dd7fb58ea5870995991f4ad6f2972bb742db968f403fb685e8f7a49a7e5b01b0cfc711bd1876bc" },
                { "cs", "9bb6d1547bbb3fbcfe771bdc3973751448b04f42d9249aa9df99ce429ee44c09f768d8f870a96ed454560879f8aa6ba36d0e14a112ea1697c9b912d43cef9ecc" },
                { "cy", "576e45c781c7e1211cd9366342995912b17341fa8e7687b46a51dc2cbaca358c579d6c0f6d08e71eedfd599b08ffbeb9c2c981f829f278c0c5c72f1cc1a26bc1" },
                { "da", "b499af0d1e54455c6ce7a59aca42efa6cc4db790fb38da61f44773d35c0b5a0433c5a49a3a814e183c5f220936a6773a597c304f52c0a156391e1fa7349b3217" },
                { "de", "f5b33f00d8092c14a9c11e5c213504ceeeaca9f92b8302ce15885e0bfbc56690d98640b8fdf06a81419c616b64f7048ef770d7afab73958f82a7efcd92d70384" },
                { "dsb", "770591a8d6d863ecb1d323900f007bd2f71f0508db050e3a4d7d4e017f82d33c130c861915a0050f8186fb137a2e545ebce19a5146454c62dd4b81cf35e68793" },
                { "el", "4d9f4dee7f653999eda3ce52eb28e4bbd2f548f653734027a18164ed102542fd5c6c6cbf07590928c832945d8f6844a3b80ef6aa0df97b4590e55a4acc7a5bad" },
                { "en-CA", "785a16b409e2db8914363e3f8b315e507c86b555f92946946d7d986312cefb093ff77d7f57cba0cb3dc18a751ba6ba85068b12bef77a72ea7aaad745428eea2f" },
                { "en-GB", "cc63f967504fea934e674b320156c7dd046b53e62e8d20802de4c0e8279807665cb4dd7d3dbbfe4797d45079cc98b94eeb92b690ceb1fed3a9c0b24ab9df25aa" },
                { "en-US", "9af5cd8a523c5d2e30cd1eb2c4c1579b9834ff1c3c44a83541f9dc4cf9c6e75d01c40eac9e45c677cd19fe097228f0d7b17d1dca3f3f120d5f097e675bbc808f" },
                { "eo", "545c65b65d34d06ab9a9382b1f0a243f14c41d834cbb097445126057fd46f313f460ea3ca47dfc24aef18ad3daa95a4306bb474db997a79acaa227cb7f971812" },
                { "es-AR", "9307eebd02fae1304c9119c170b1203521844d5445188e7e04c706227d93b28a5e2d788d7da33e102a09d711a9afc2a168a032391e8a8ee88588ccbc39f2aaa0" },
                { "es-CL", "d6772b803937e13d1f7feefab014db2741b5634d66761fcb1008206cd4768fc630e65bd70d32b3b884d7db99d20468952cc069cc8b4c86930bf74fe3acba6150" },
                { "es-ES", "76cd26efbe7fae97655cbb9c3e0dd468c2fc42b2763e69b050814ff79e7df4a02bf32fd34f5c27b938646aea74c9febbfcbe5c5e6708c1686ba6d2991ece6600" },
                { "es-MX", "05172f7c9d7402106a158a59483c144caba8ceb18032b24005b3d61ed8c97d3a3e690bf1e15bebe36ce822c2a13fc40b08a5ba6efbcae19c60ecdcf11944d4b3" },
                { "et", "61aa8d706960e62d25a544835adb136f5aed8e45f85efa413794ab29919406cde08173d804d5b99d9a706387cfeaa7527bb48241d878b9bdf6678e88a0755db7" },
                { "eu", "bb5d09f51bfa0c8f63acb4cb0e150a83532e9df5cc1b05dba6efa538f2f7d77c66ad59e7103e23b2f1b0ef266aedc51e94367a8d163fb47624fd21f07db54032" },
                { "fa", "61ff2fd8fefd1e576ab41f90f9150ea1a721f4c275ec393553ff9fc768c500b7046edf26ffcbbe49246c05399afd45fce0b9188528df9d6281fa19ae5aef1813" },
                { "ff", "479132ebcd1922722edf0bb65f91ea10965cfb9f42abc9e57c64a934dda84dd5a1ddb4a6d02bed78b343c8a7d356a55e5dbbb2d988f725a3591e43cd67784e7e" },
                { "fi", "62ad93851ae18d48ce4738c21e43d35b5b8b05909b53947f0c42727e2422c9a06b1add064673bc4c76b8799147ef64686ac871c48b959f417ce8d6d0315a11a6" },
                { "fr", "bdbb530e7c2db5db76cf2b20b74e68524a9c99a46d9adeac5844de9537faf784962e61d3bc9b0bb4e28e380f108c0c44e84945cae97005a0ea0e1bb0e3175445" },
                { "fur", "7abea74c1ff16370abb98bf57b82b1d1c1d2273e6f9e960c8eb5f52e08b30483f4229718d6942a2076e30131623c3df72bb644f1cec923175ae4cf85395e2009" },
                { "fy-NL", "b6120d8dbcf070b8eb49e2802dd14174cb786ed34465ed2b5cfeb56210318807d9a874f8e3ad34acfaf85e09f252c6cba7a89a7bf882a602dbe389fecc97a295" },
                { "ga-IE", "2898dc971f2167824f44092b9cf77e0b72d76b610ab362baaf10bc23b4a9f70200b728f716cb15ca104ef04f47a1b31f762eac3c7aad261dd09c7a5f94697599" },
                { "gd", "7d3de4a95727d0de8be836f925230c25da5f6645bc93e5b565fe77d730cd24a9ffee3b68d875478be489db38e13a2fc149828e88639b76408de2dfa06b86892a" },
                { "gl", "ce95499b14a5f90ede04e71c09a21379656025f48067720cd5ddd787788d98e981e18cf8a59d5f09e794f879dd8db7fca57484f4de7ab4b54028d364bdb8dc5f" },
                { "gn", "4a1b5aee4762e28e81dac031fafd572d2e5d76be16ca8594a34bd5e8c6774832902a6488a6af2b23acfe8fa5b78a16b3b8adfe917a40f9820567f6a3c7f2167f" },
                { "gu-IN", "5fe836846c4ba4137f4aa40092ab3ab54c0c4b1052b6005af2e954944d3b1116a7e1aad9f6efae8a58d8ba91db59a9e95beedcfa9294b5735df082676a8d8c55" },
                { "he", "c97f45794cf37a2d26b9e862dd2381d3bbaca764562fd2c4dd615258c3bcfbafbe6b2e6843b63e93f85e477750fd1c370c4741e33d42486a2678f4a87aecb3c2" },
                { "hi-IN", "1bc44dcea7942efcc90bafcb0f55651ebb43b96928b8b32398d26e693cbf75f712b58885db12e17dbe0919a0d89be22c192ea07830f63f51759ea0e210a27ba0" },
                { "hr", "628a3000db390b42f9f3a04ceadc9b65dd686c1acdacd22195bd624665b9ddfe275e0e2223090cfd2540f4d627050039edb7a534d3a98c886efcf94c77e7f06c" },
                { "hsb", "818c9e6dfc5df6853aa0f88e55e82fa5597d0a4437e82a778d4895c7f4c67c3d86f23b7b8218d893c8233eac84b685b65afba4df0fd4e3172fb3807a8f5c61b3" },
                { "hu", "b20c65f088a144bc7cf350d7d7c55914f5e1bd90ddf6505c5acfcc5b744abdc95f03a30ab7bc7ee0248f53a686e229566ea50d6dc3c047e81ed612402876ba80" },
                { "hy-AM", "636a1534fdf3bfe45dab8a8fa7297d9b742e2ed2751851cdb0f20cc9384ecfabe3b9cb9a8485eca7b820ed8a2e8bce97bba20fed8fbec02ca6b199e80567cd45" },
                { "ia", "81bf5bc2c9b58ee637e8e0201f80f1559d94028d93b2b1f97649c6a1cc8f2a8b2eb087571c5f2dae8a2f584574ee0fc44ea3fd2b1ccf8eccff22db6baf37c448" },
                { "id", "afa643d1a6aa7b5bf0f2f9a7e1f1a56558b12ccb68f653a5d76580ba6717f68411b39f2a79315e6d7e45deea47606a92dfa93a631d143fd3d1ec4d022892770c" },
                { "is", "f1b29c36b297273a2bf98062bfa75dc48b7770f7c3639f68be6046694834378ecef27b0bdb17e78f127750cbc11afe831a1158f988fc521a31d140af3d6fc368" },
                { "it", "9dc38ea09b1d05445f4201404476d46fed806207a2c60ed1551057b237d8144b7691f44cb1853ab24de56ee455803a1428e0be33e55cfaeb2dc53ed8dd7f58ef" },
                { "ja", "edf897abe86c7fa88842a03c87c1bb687854b0c152e27a9c6bbf938d7a630f9570f56c770f271686b482a7cb020bbb64b2e2a3862ee6e4506a229d16ae526078" },
                { "ka", "385c7901899b36a2499689e0efb22989ad66ad6fa56604af3de6c6252f77cd48cf958d07ac7f189e633271c24f40b4db15aaeb69d58295834354992e19a9e94b" },
                { "kab", "8a7e755744b83e7da8a5ec8e5b9984fcafb661897edb05fd8c82d4a10110f7067cf8332dc32e41ab229885473dbf3ab1439f289992ef8f75cd674acf7731e779" },
                { "kk", "556be8367f8ec24d6bbd3478e385a0edd6ab83c4e2fdee47f6f05cb4e68d33240dbfda4465d1c5589f7cbd9cd5bafc56ff8c48d6a55e9dbcb0224e412e5f34a0" },
                { "km", "00b792beb5e6f87e7c96aeb6774d210b8342ac0477a239a9d3d7c657201eb520fd0d51ddc8eb4a264a9f6cb0dbbb34427639fdc25a6f402b47abb376c1e6cfec" },
                { "kn", "0d29c391dd7d4ac9f66772ee30971fd1b62a8c5b61824ae5176fd62577d121104264aa54f15bc8298b7d0f6b80bb4446fcbdfc312d987f0fb9088cf2a6cd32dd" },
                { "ko", "2ffd2174abd7cd1384e4dd77deb2a27adda9fdd90a0dbe9c7a6237acc7908d34dc7d35ef67f34c1dd0736b2c18e35995001220a6184a1c342fe9c4e720b654cc" },
                { "lij", "455b6e18bb516ea65c270a8beba852186cec099ece9ed010978c167df7795b01d589a7929852c487369cd139c982293b6fe37080c8f20c1594175977a8286822" },
                { "lt", "750a531b9b74ab0ece8d7f1ff191e157b2f7bca8038d1c3a8f65ffbaf9ebbfa60f62a7ef88aea80998cbd5115f06ad20ac5c255c2feeabd3947ecad121b54739" },
                { "lv", "212de5e2c426d215aa60ac271bf059b724cccc885895dee3e0f621f302d49fc51c91945026f67daaf07f408884d188d2652c23d412a40327d6c3986701afe9b3" },
                { "mk", "7db510668effc807f9cefd75dba1af4418edc2032d296bfc20a975baa940b1b193111976d5617af135b58e2ede53dd725a12cf9f07793a326285cf8b55186df6" },
                { "mr", "c92458ca40b6f4f027b82b87a989ee318d96813a00058b5f7911173e1307d1da013a2a0c8948eadb8fba0358aec53418c5275f1c41a8b4f43c86863418b80672" },
                { "ms", "116739f46b017bfde20a7e2c03efc1e2ac3cfe9be0625a567b8703e0b6c3d5fd1f3ab6070de2fb0c4fd86429f0467944ee43b7238c176ff693f8c9a6c5b705fc" },
                { "my", "c5b4803664f7ebb8dcde4e12a7474cf4b62e1b3abd2db1a20781b682dbf2d689dc49584af61534a0ed19652d0effc9fbe8e7ed82596700bab1a1a8d33d90f7c5" },
                { "nb-NO", "cbe51bd1f1141215ee6f51f60f1660a3d2b4e9b545462e117ba875f91ddfbb6a2f3e727e49ce02fee1901d42b7d203db142e2c6a53f4fab7dbc1b94dfed9c7ea" },
                { "ne-NP", "35896af25b6f5cef2ff5b418e18c4837c14cc215c0662849880ab28f36607b1ffaf8789087ae61020eb31a6597b2047113eab100e17af3bea4731e9007268b62" },
                { "nl", "d1962dc5cb384571b879e575e4627712f5a91cc6ee3edda279835c9249c3c3d757567da41925303bf30c49adbd2802136738a8d38d7368e115b3c6de000da6bc" },
                { "nn-NO", "7d839cd8407e59722352d40566321bf5a46045daf430e83e48aa2028a82ed0baccf22afdbc1cc41e7339e7b48fdd613a7500f3b3728cae5ccfc054d4055cd5b2" },
                { "oc", "35bf9ddace2e6930918c20dc9ca6ba1e8094f86a3bce59041064b99f913a938e3e011963d360385b36a6c9778a4daa3161b487c937f588e848b11f8222ea9a46" },
                { "pa-IN", "6c1ad652339e47a5a032edf793f88e7fe2a49bc672536ee8f883224ce5d346a90ed30aae713ffdc6863f4d01c204fbfff39aa81a30425714209560223ba2f47d" },
                { "pl", "7eda8f68f08ef498a8f0e2fc1ba4951389d420d60fef2c2051dd5e5108cabf9db23d52b4cf2382e7d865ef8f46f2988128bc0b09c331971706c810b611211583" },
                { "pt-BR", "77365f15fe5d2e3bba4d5021462dc7bd10259aee8764ee3e6805545df1e514c101194d94bda618562a5ba15260f68e4f5e5b35b5a7f0b28a5535c2fbcdafa8b0" },
                { "pt-PT", "b1a783b508d2bbd9662468fa45ef0aa6debbd40965423cd281b767a0989a0625c253f20ed65677255dd2bbbd283f978d90c02152654619ea07b949fc2a5dafe0" },
                { "rm", "dc675809e1186d7a6f8b94536f570a40ea1125a5ebbd4cccea85286d297c2a4ffa48dce2bf584feacf6ba96a14ab87825f74548272db6d28670b29ca1ab5f1a0" },
                { "ro", "82d97343ff8fcf9ea11612762245ed3c8074c2e7f8925e0c59cc4fd82eb18e92f1febc0efcc2d36da78cb814b19c8073026139812940c78f11dec8ebdfe7ba40" },
                { "ru", "373bbb16980056dc7b3f538d7b6070af700578bd2b9b4c057b675fa6dc375dbd02ab110a953d7341c410e2312c89375ac3768f1fb1367ef2ef499da3a725970f" },
                { "sc", "acc0214bba8e15dc4da9e0260ed70b77ec0efb7f1cc5e879b4c42980885c4a8e5594d7f5fcbafa5809ee803cb5d1986b5da237bfebb97cfd8563d6b4b71819ab" },
                { "sco", "368b7c9c9834e6b039b743bda33a58415768d21181107527547383bbc77272c635ee3bf06959e3fe6ee9d3eae14907d1fc80bd1da3d7ae1dac2c7a328b5bdb71" },
                { "si", "1bdb8fd9b4c1fcb4f516633f4ce83bc37e60a8c05d70834ee34f2f8fa9fe6401de3bbe4df0d6a62a2d7d87dda133badbc2c39dec876904f151871e87d400c3fe" },
                { "sk", "695026c1826ae1864365f2d7cda7491f45fc03ed454c1d06c1e0884c1f7a47cf0f9e313e37c4a0e0746e840daa796804f07a918989e8ec2e74affb9880964833" },
                { "sl", "a98e474099c5e03cf3438934250c5686e5274149f529785d3500daad93613445f33ddfa57fe006af661ea350abb1f58f7c22daf4497ab3e78dd291971d282f69" },
                { "son", "bbfbe4abc3ccfd461946efba54300c1d4e5fee7aa99766f20ceef52b4d79997b2f82421fbf587c6be5ca706bc31ce9f5cb7c580ed97d08af2f164ab98ff80877" },
                { "sq", "d726a16cdab218fa6afc4c77d50172c92a07ab16f9b823f5766cab8bb024ef48babf3b984dddc42758c18eb71523be7758c5758f31d65c5944db13951c1195e9" },
                { "sr", "66eacafc951d25ce07a2a999e1e4f9058f93190275b47ba5eba77dead42e1e8be57379b8bec1cef5927a172d6b4352fa8b8a52ba4aafc90e4fe314f7d79254b6" },
                { "sv-SE", "d981eacc1dfedb85d6221b50f96f33fefc190df4ee7ad2f8ebf77abc578be9d9f659844af82850fda9132b3da3084b569aa27f969863e4e6f1a1d3ad8121f619" },
                { "szl", "62d3ec79e4a576d5893f3841cbcddfc3bc8a95081c032a8c8a60f759d6d78b424fa2fc676db52391d52a1d87e044f8d40c44f6344ab96d5a4f14419824248e24" },
                { "ta", "3a1c9e45efc2e498efeae6423c13c389c3a5f45e53f4c68c8f4d9ba6dffc73fa79b58c05d07182cbae65dce7843c7a2a44dc590fdc82639e7012b741f76e3ddd" },
                { "te", "345c1e0410ac218bd7f645a4a457bb70c78efedb5496459adef797fa1f448442bc57fd1622613f7e4c306a0c41c8bc3ca437e68d92a7afe969a0288e5593abde" },
                { "tg", "6c1095c01c3b2f7f9bbf31a0e607ceb0349fdc2a81f36ea9b8ecc3d61f7f1405a43988c169562b1a420ab96e9a3c103de1f38dec23e75319c64abf86243b1dbf" },
                { "th", "a3d91923452de1b0d81c00ffc7dd6b56ba5b1490aae2f106dfda9ceddc038e21f1d434c6fa94049c57be7dca1841593ffa2108076778ba425973a45ed8380416" },
                { "tl", "0de11b100697c7129cba4dda5aa3ea3e079ac2a7c163fa8f771764a5182cfc4b7fb3443846a809e3da6f3973967b5605692ec6f99ad12f4feece00077f0eacdb" },
                { "tr", "aecc4d0eb770cfa21062434be3ca4565d5f9cb2b4c89f8df4fd6e9fac5fa4f4984a12a1cd4aef7e6d6a15ea5146f4e41bff8472763f21d94a58eb8f00c972d03" },
                { "trs", "ce90389d4fe6d4f0ab10118b6b3dd5088d55677be72bd2f32a56413ad674fb4d7a5ba60a0bf3305834d9cb4f49cc0cbe64fe9f80fca90380db3e97d3ba5662b4" },
                { "uk", "1f18197f2702ddf34bf98ac7ae4a69cfe3d7d8037cfe3ab8e830ecb21a0bd5e34e3ae18c7c7688a642db875967083fb419bc13e0923be522c891e0f85390a956" },
                { "ur", "a18090b1006922773f7b0ba6944dd087f9c6499b5502ca2328e82a4fab4bfa1d4f9faf8164a4ba2d1996c9f44f02654984173f9fac20ee6e5eef4ff8636f4e68" },
                { "uz", "db65cc8d95fe5537403aad238c84e8e4abc836820983ff4bed66e167cefc0f5103a5d3e7ca0bcac9414974171c4d48687fc0e301eb8bc6707272db3b6837dddd" },
                { "vi", "6c9180031d854097fd11fee7310839565631f61bb3bd752b55f0765a54529e7c5c456e0dd7620ce14e67151bcb3a0a611b3c2b51d69e562546fa0594b76ecdfc" },
                { "xh", "a92c5cd885944df250730c1f456b82b48b921ecedf40f4907bd5e0ba78e3af4b53d4be35269bdabb47fff1b71e798a319b43ee1c7d7262d95e0147323bd04c22" },
                { "zh-CN", "8ba8836ff0a2ee2c0bbfade7416ad73c8670b37c738d8ef5fd8ff80ea5d89699792a7186067beb4aebfa423584530ad9515ca1c6edfdad565151425896a34e61" },
                { "zh-TW", "5a740d184dd3ad6f279033c5fbc1fc2c6b88bbf87561336921faf9573a06e972935b60e3bece0359b1309bb6ff3aac86a0979d92d80f0f68bd9e50f87ff4f286" }
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
