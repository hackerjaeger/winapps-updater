﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017  Dirk Stolle

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
using System.Text.RegularExpressions;
using updater.data;

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
        private static NLog.Logger logger = NLog.LogManager.GetLogger(typeof(FirefoxESR).FullName);


        /// <summary>
        /// constructor with language code
        /// </summary>
        /// <param name="langCode">the language code for the Firefox ESR software,
        /// e.g. "de" for German,  "en-GB" for British English, "fr" for French, etc.</param
        /// <param name="autoGetNewer">whether to automatically get
        /// newer information about the software when calling the info() method</param>
        public FirefoxESR(string langCode, bool autoGetNewer)
            : base(autoGetNewer)
        {
            if (string.IsNullOrWhiteSpace(langCode))
            {
                logger.Error("The language code must not be null, empty or whitespace!");
                throw new ArgumentNullException("langCode", "The language code must not be null, empty or whitespace!");
            }
            languageCode = langCode.Trim();
            var d32 = knownChecksums32Bit();
            var d64 = knownChecksums64Bit();
            if (!d32.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException("langCode", "The string '" + langCode + "' does not represent a valid language code!");
            }
            if (!d64.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException("langCode", "The string '" + langCode + "' does not represent a valid language code!");
            }
            checksum32Bit = d32[languageCode];
            checksum64Bit = d64[languageCode];
        }


        /// <summary>
        /// gets a dictionary with the known checksums for the installers (key: language, value: checksum)
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32 bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/52.2.0esr/SHA512SUMS
            var result = new Dictionary<string, string>();
            result.Add("ach", "10fc8fa6819289df2ca6f329f9a3427975ff2092d10b9ce046d8af89be5f1c3b0203e32c74a6f5e4677ccf44aa27d7ec6ad277c0b7c1c7da26913d7fbfe5ba76");
            result.Add("af", "a48eb2263910f47d0f2c1fba3d5653f4ba004dd7a25ff81fba26bbefaabb0cc46fbdd00a35fac3a61cd9364cd52fe6e38a13de115cd240d50af4bb11d04ad709");
            result.Add("an", "6dc1d3fe72f14c7e91d54ae9be4e49fce7b692bfd81799c47ccdc4304fa471a3925222be1394a47cb8a4980e8cb5d49673d651df3e76e884461d79e8131c962c");
            result.Add("ar", "35a21d1a8511674f4934ac6a5bd34bbf352be6ca2f30cbb52ada2f6e52eec400f1a0e57f996e08fdc6867692d511779f7d0d3c2eb2f531a6ce84e5d51b981dd6");
            result.Add("as", "f85f3a91ce52b516d9b5b72c8349c334f6716b657eddd435cf0dac518fa95f9f563a51995a525785fbff21597859cf912c863e53334acdbf118df8a80d3dda04");
            result.Add("ast", "678fadda70557c71375035797f3daf0115c6cfc2b9d34b376facc18eeb1a0817e8ab7107f3459fa9bff7ca078835f71a3fbef82ffc5b0d8368eddeed2ec15511");
            result.Add("az", "0e17425fbd4fd05eb88ed634c76ef32f7b87a7a7fe902bc1b841c25784ce2840301e56335f8d3a4753dce6a405aa5c5edc5c418a0ea9b0a49439366bb36e3a93");
            result.Add("bg", "af8b7d0c34f7834802736546779ca2eb740d34e70ccfdb69a8273c772b79b217ede8af30b60de5c97ccc423489505a967ad29b74be7ad36175daa60a05922499");
            result.Add("bn-BD", "ca00adfc91fcc356776b15806c087d6522f18d684885ecf85610bb67d6e23d4aefd57f02c8365306bcf071b869828627b4c027c894cb4cbb9250ed3f6c9c6518");
            result.Add("bn-IN", "ab3a79284f242dabdb6913a5f83974f05d470f39348b76de282883dadf70b303697bce382d7c6020041c5d3ef5c1bf472fbcdab054240dca82ffd4b21d552151");
            result.Add("br", "e9156f5ada11ecd8f58ff1ff5b233b85fd59c1d1fcd6297f630ba19f468d03b27e39cbac80692f2153999c0c6bc44b92a698cb81018d76331898718b21892beb");
            result.Add("bs", "d37c64aef1a9416eb16934ca077f26f2d0196bc78d468826f2c05154b0c0025445b837bbb17ab1d89cb030b4ff568e445ddaf8fd4676413c963f56c58f13fd38");
            result.Add("ca", "ac2a6eb2a963697e4752d76b671cf57bb94bc08f928ec9c7b884031b71d332a59c6dd2338b5bff6f68fc4d513530042c61c66e62c6505a345505ed7fa567480f");
            result.Add("cak", "c50b57e96eb82f850c823add776bdaddef95399318d8dc799c03fb70737596e3b407d2c5e3978c26cba447aa8531a7d5199574474000029f6a528650aeac5874");
            result.Add("cs", "09bc8d8cbb1f94018d7527dd10c26d66200d7e3321757a33d26132cf34eaf638aa7940ccdaac228b468194604b517789cd53855ce212b9171e18438cbf5f91f8");
            result.Add("cy", "7a20f03ad144b54112ba5480815a9c8f28c5ef6c6b97203aa8671de98d8fe38ec0e0fe8e83a953af969ec0b4e59e0696008c5b5572156665b263eee5ec02a207");
            result.Add("da", "4bb7c4f0a8ab8c2e4e0ab5b941f5c9c6852bd7aecd520fe3a199142e594f6b5b17d6936c4142a0f08ad45f4b295ba09e28639a5ce85c3d9ee2e7db619a678913");
            result.Add("de", "5bb75ee72f88b2088fda2763558ff4ff6657c85722f9a0c6e455b0b23f38cba256a8840e445e7ef87f708ae81217d298423865fe90f48f5803ce5c62b9aa87eb");
            result.Add("dsb", "e64eefac4c7601acb67ea6816d4ffafbd21dc3122273b2066ccc3f1f3abf97e8d578bf21ccdeac53dece982755a789e62ab6512d3f1cb1f49d04c213216c169c");
            result.Add("el", "aba2ac8af9c8238b1ede08370e6d6f4f148c655ac6d2e8cbc25616ecb7ff755f88d3dccb715610f83e236c964f304ec7fe95b6ad5860ba39455d370861086c8b");
            result.Add("en-GB", "934880388b0900f6a95dc353c555de0963c453f8030d3e3db071e132b9646357aee53724022b7c64a146b73df94469d6c888996edfe784663e47dc37b26e55cd");
            result.Add("en-US", "8848d3713b63c42eb2e2df7866e367dee37b7c0eea4e4f3429c7909bf219c04b8d320ccd663ef40e687d0a8cb9f976ddb46777906a37e2c5cf9a1784aefd5621");
            result.Add("en-ZA", "d4473eaf59dfd0dc6bf2789d1cc3c3c97265472745ecdb128da10db6ce84e070fa29d9d13020c72842dd36af3c7d9aee0530aaa5a84a98ce289e4f2d03da9c8d");
            result.Add("eo", "851ab6d8e6385e0a47598c83e1deea63f516af5604da3199db7599a28a28aa42d23ae926f190110ba36402ca9656d17dae2929410f8b932be3284751ad0ed18a");
            result.Add("es-AR", "8418bed09514c67f2003ddc6e5ecfb9d17174a0c4333ffc9618b6cfb053a113b2743e0223d52e618f0f01cd53a6062bd3d65f8047d653021ec9f8fb98bee3b7e");
            result.Add("es-CL", "83b301ae13b6b599a34bbc4460df6a1bfa749e084b4c437cf6fb91163d1670d7247af55d4fcb0ece324126b4446e4e35d02f9f23749b59ec9f3cf3da0964eaa1");
            result.Add("es-ES", "44f91928125b065c68629c5df60d337994f08bd2f2dcb1830b3551602e797da7e6900ae23159f9de28059c49b3cefe25787b1576979f9400096049dbee90898c");
            result.Add("es-MX", "75b4191afdf43689cc45ec8deccef884090a868b5c6dbd648ba694f55986bb84a48e996069592a8c9f378096fcc85fc6ea67b55b66886ade3756a84b8dce4749");
            result.Add("et", "6a1ba219f374d8f1812b10a755d4e0c1c3b26ef0fc77009712bbded2dfa97e0d0d3fa8168a02bf6fa7411304e8ca8408457789d2d122ca144c9c4d03e9b63f5c");
            result.Add("eu", "389b8b9f14534b2821cbfb4249abeb2429f4d562fc4ca52860391184db997e51cbe595fa85b8ee9f8b6b05e19b2fd45e151ba46eb6ce99611092d9d8a3c03bdb");
            result.Add("fa", "95bf4d5aa13223be93347de93baca5c2deb627f41c06b33bdfe0bd0c1f7a4f5fa06ad5cf81e02ca62d3d71a053b00f58f0e96284add29e46ff120c6c8d671755");
            result.Add("ff", "6d6aaeae509ad083db96de3c1bdd3e0e06973b437bd0554043bcec611d05a3c8868d739cd744bde620daefa18a6f4eddaae290f2083115b0428b519db61525d4");
            result.Add("fi", "94ef7db64b49efd6b5698865bfce4617efa2e99cff47508d26ef187ed2b37ebecff3c9116b12113165430875479200ce7159f48fb7eed525e49d581e1edeede3");
            result.Add("fr", "6b366224950ffd6f88ed1bb5042ae3fb12e45c098fe57ad50b9be4cb2e96d10641bed23c39313074385c789fd68a1a957beea881fce011ade569554cea7d9b9f");
            result.Add("fy-NL", "cba10f8156e58a2c3ddad8763773056d3a0f3a5f8c9b6bdbeb4caf355e16ae33a966602ee3a23352dbff02b4a2aa168b30d01aed644ee163598b1510455c128d");
            result.Add("ga-IE", "0b93eba65702de64fa9a5fe6722d9a995a1aa647919c22684d7fe21818679aef272bc2e3f2cab7efb2628f03c4a9d603269767b806725348f38ac781ba89a508");
            result.Add("gd", "4d978eabf9df8096199bbe032499b1b5d2603d9e78b0108b7bfb609ef1bb57192ed6a4b8f34c23570677d461b16cf8a9f459953feb6a4cc24800be37bd821850");
            result.Add("gl", "21ac76717b62236a1997a171e75172c14eb4b9552a22ad1f2f38bb93d97d8f8e67c13a6354eff1e587e1a5bb162c9237450aed9602ffdf34f1f10a14986ef5c7");
            result.Add("gn", "4bdecce6e557cad1990b3ff0f2dc2a6b0683e502ddbe9f0596d346f2ffacfbdd6834724a824e75b97cd2420f307ce6eb6fbc93425148514d363fbc3b820f7963");
            result.Add("gu-IN", "51b21da3e25855f85413c483e241a76eee6a592bc83cbfd73541099b1a213212899ea6a31e52bebccb6c002519346318250409b8626a826158e58e6292ee3d68");
            result.Add("he", "c629e2e840cee1ba5ff63175be10d7abeb4b4a430c6fb62d6a58ddc16607ead5591c54cbe6e15b0cc55953d6b2beee25f3ce7aa1bae92f697dd7f01adc107f2d");
            result.Add("hi-IN", "30b04c10a80e06efc479f0bd1910f54fa0a6655896e140bec16d4800d03a90cef029fc9a9010183f945b6d122149bdcdf18d01f7ac1db27b05de9e7789fcd566");
            result.Add("hr", "f4533618b3c835d21bd06f486c335eac81527a7a96ffb401d41088f973e5308608961fc40a2b320e89ef9265818d526079819ec1219443bd44355d5c98cced44");
            result.Add("hsb", "e23e3d91fbb6a71522f8624d32c3d6be2638e4c945eaf39b2c14cc82f5be86b52f1e59502f804a2e1b8be75028f4d23e8ad90313a676ab4cee697ad82d881b7a");
            result.Add("hu", "63a4ed3e905af3aa8a58a713806db6d22b2c393820cb383da96c5c295309ffad4645ac852d62c3bb6266294a1b8ed529e2e1db01ee36e6168d6a23552ad309aa");
            result.Add("hy-AM", "cc536d58eb8d3732811913e1bac38a8b5166c02c46892f56ce634a388aae3a199038767e606c7b7162cbd7b19b5fcb2058a4facf2433c20d4c964ea258f2c6a6");
            result.Add("id", "8bba4954af45f4c5b671e6239fa8a52bf5430012965a39e2706ba37b5d183b50814427989fe4680f3382b7e5976f99d11105b12c30995d6c80b647d56030a885");
            result.Add("is", "0f6117aa1fa69249828f7226ccebea187fe5d1ccafede378cc27a700ff447accea783fa6e5d747cd83294c8aca07d0274d29546f58120eff91323e0dc42d3563");
            result.Add("it", "d16b95906c16705d505b2e328df9d7e8f93af4e0229e53b387ca984dc5c13f0a9cbcb24495eae13bec9a6c30c01deb6ce933666f54ba1e38849b9f8b782b4c94");
            result.Add("ja", "63c09dcb39d9c23239a6e4a914ab8b52b565aac5ccf94c3c49cefcf364924d3fac923e2fb154a252a38263d3f0c47ad7d8eb8b443d5d74861f4847dae0a42bb1");
            result.Add("ka", "122c85e5306ed6a80f1d29da460ddee327c6c7b73e885fd5c604f2dc6360378ba2992db72022049e75d5323472eda0d3dd240772341b30b28653a831325eb2b7");
            result.Add("kab", "3e875143e619b5746a983f2b0acbcd350f7fef0edd26269a1ad6931d95b4d00e52fc29e220b3991067ea24b57f65868e0cb9fae0962e787f0210deba5a451c68");
            result.Add("kk", "35d5a69aa602599c0c038825c3f843a566136aa0d68b8ebb7d7c96cde42866d3e8cb7910c1d7c980234eacf6d47c79351f874865ba8f78e4948430c4b16a73f8");
            result.Add("km", "2abc7a48ec4f6fc333b46f72297c520b51012d52f5324952201bb98962221f2833bcbbe894683e695b2826eb77992f1e1d8322f02c208f10b8ced43f3af59e60");
            result.Add("kn", "672504d58e7b6057a2e4805080f789cdd50c7a9db920e8217c3b39702fcf14843cf4782074318aaef5bab9a7618c988f6542ac1614c60218401b031af604830b");
            result.Add("ko", "bc1f0db54c51332ff989be3bd9f245a8a4bb13ff01ee5ed426c951a74f206d4b534148a38545cea85fc7d1b2c61e0f18fb2858fb0629dce2c0584c307d2787d4");
            result.Add("lij", "876aa154d5dbefa229308ab67f1bf6bb384127626e25d463fa059da5f2e4f078c68d2b9bd3a1d6bc7f38e12464458fcd1e565cdc4c2df6167a6533bcebfab349");
            result.Add("lt", "12013f569d6f34e04ba0fd08e72ec5ae622e157b8c1f7ab460633249b969a7769a105b398b8188296ce971cf0d4295b21d9858ec91533171c8f14cab8efc4b20");
            result.Add("lv", "4b05a4c783867ffcaca5da7580269120ad989310aee345969b5997f6823c4d541e37835e8af16849b7c06f3509f828cddd84244a9bc42a23854e77a4f455b98e");
            result.Add("mai", "9432b232e62546cce11992b5f47919027584e3ee4de07cee9ae941c27986e2e21ecccf0faa80d9b30a774751b359ddb02513d3ce45ff19d8ff39397196bd8420");
            result.Add("mk", "4b43a991f64f501faef481c43933216cb8f7097427e51ab5b67d144da6b62043692c52fa864f69bd0d3e748cc1508d8cba1c8a83e99aca0421b184b5bc6b2cc5");
            result.Add("ml", "41a54e82a992cd4a32a4c8391ebbea2410f1fb37511226c5326596e5c31d44d5508b99d849f4b573ba2a526b5d908713c477cc88a3149d73b51b9a0709f9fb5d");
            result.Add("mr", "1ad880669009109e3590521c063ad6e8364907340204f53a400fd04bf0befde37eafb919e84b0a26f2c6df00fb48fd113886ef99cd00e9adda36f86f099aedd8");
            result.Add("ms", "5b35293fadc59330bd6974dd64dd26c25f54850f71882b8328ceb72993bc301fdf66026388cc4808e5b379111d5bdef3bb4a182559e51f321f4e32834b0a724a");
            result.Add("nb-NO", "8221dc727c223f4405e9ac7be3258e72275da962c02e55561ab0fc70d13244275acb8b4ba70f237943e7f9eba70a52e8858f90b37dc9da8b79da57bcf5493f3d");
            result.Add("nl", "7b4fb977636031e3e7b1277e6a725f86c24fa12b9a0ec79fd8c6e6d8acf85f892f477c04883f5e8147f595e6e728ecc5d0dce3543b5e127f63c768a1e458e6ee");
            result.Add("nn-NO", "cdaae6e429a7d4dd4b5fa4f36ba31eb3fcd3dcb6a582e377085230898ddcec2c767bd50661f8a89d3ea5d802254d1f75a80e220bcd3e2cfbd5d7e80e2391b154");
            result.Add("or", "c20f2e23b697b8a346484097ac580479fa7db71038b13dded397d4a928fb6489b02202943321527c71def80ae5d09645a88116cbe6a47d99689670861f5121d6");
            result.Add("pa-IN", "583494214fc468a072d0880b3befa3a3c629c669ce36d05bbf8a7412f1a2410ea9f6eb21ab554276f43ac3b04911c13ec3f32ed1db336ccfc2dddbf97156cc8f");
            result.Add("pl", "3931f1ada13928802156e5c6a400a8101ae9094b0ae7cbf04f54dc35cb3ecc2f0aaa133fe6230debcb5de933839ab04d4d1942de54bed11d854bb95dd093c59a");
            result.Add("pt-BR", "62b59f2d7f3387413f5f6bc8daba54fbdfb7d59a197c47d724acdd7a65c13de0d04bf69562bd53d2ae2b51a309d515b4495af4976d5e6d4586330171493ad350");
            result.Add("pt-PT", "66121890ed50010abac271535fbef0e80c65fbd6e6ddd5e3bf9c7509ee3dbfa97740fa4bda463e6664b2681b664817da8ca68d791ede0317b79271d2bf39795e");
            result.Add("rm", "02edb2a00c8bd54a1f05c8af341d229a07550e014ccc56e22ca905017e4d6fd5bdb98ddfd963790110e25005d94f6de09635da273053fd3664a7b0aac06f76a8");
            result.Add("ro", "28392b88d5345412902592efbaaed66f2a3e27c5a75be56970602a66767c890c21f82852d3119ef90da4db3ac95becfee25b073f41d400d889553a5d633be5ae");
            result.Add("ru", "4d23ca25a0e7a1595eb3e446408abe5f33c2f6d2881b08045ba251542263f5ce31fd067d47c5e77170005585b1ce1f844d94e1afd655e2319ef643129a652551");
            result.Add("si", "34bde34c001499ebac377e33f735937cef073fc785ff72ac3641cd2b961c0d5168dcbbd801486173e5f3a5c4bafaa988b4532cdaeac2a3b41305da26bf5d1835");
            result.Add("sk", "775b740ffeb7f4a6391473ffe221545e9d7d9d498218fe6c782f94336a9734e353fe1dba2de45239fa0d819a4bf30bfd6798a45b8ee8c7a0e8d912687ecd8a4c");
            result.Add("sl", "d68ea39b019894436c7de0885feb04cb55ffd5b5451ba58b8d8d37fb68ca17e2d9dbc025e650b86b529ab21fd2e7a9861049d8d8ac738465d8cd9afa56834266");
            result.Add("son", "5a5af46462ef1a20ee41a38a27aaf8160dbe3a8651e0f71727f5f2c99f70f848e8afe132cfc402addf69ec95792f72352409a10c9a0127a2ae13b43bc31999e2");
            result.Add("sq", "8826eee2dfcf4ac3d5fc5dbfad2aa6b8fe0c4b493437aedca3d2decbc4035c73b4256f5ea72c3ee2ab4cb4d5e4ae47ff23b87370e36fe8083308fce9ecdbc287");
            result.Add("sr", "2b2a4b3899b52a9ae6b6dd5cdc7ea3030c67a83464c96adfb83cae8be43076569d14b2f8d2e738a4937b8870e02ab9e98850a2a83ae9965323d119f859c5d90a");
            result.Add("sv-SE", "89c5188cff85aee2432a9a88c2ba625d645c5be061c186c50aca51767d06b29519f5f9e0bb82088e79b2d1dd38d4fcbe43bad3abc5320eb349d88e0948355e24");
            result.Add("ta", "03670d5c161fde0e155fb963343f23b4fe321165ce56132408b5e51b7e450606f2dad4dabfa9f7575992f9e6e2321a0fe3b947e58773ab3e38c2a0489494c88a");
            result.Add("te", "9965b2f4b895dc9b07cb4cef3ef2c6278c98ca71a7993e869c1c8d7cea0825c22e8192703a87aa420d42c9548d970ece20fc300e6fc84530458e11a6455f23e7");
            result.Add("th", "d8b2a84e7b8be1347fa1f1ece0c48a0909c6fceb5db16500d2f18e25d5e127d07ed986865cb4fd61c407e754ed1f5600c005183a3aca90113a3063bc1c211ac3");
            result.Add("tr", "e1628e8588525d42c0819762c557563983ed0ab03a837a3eb9e8de508ea625a6fb53868446a7dac2783d12fa3feeb1f39efa878e28107bd91e0fb3836991ba9b");
            result.Add("uk", "605ee8281957a61256072a7f0daee5e65135388ad5ce065c8cf6e57439fddc24afc2788717ef8e708f482659d112b9d946b0658449080cf29644b8b55d7460e5");
            result.Add("uz", "01f4de2c6a58424de4d09408b53c2fe82a4267c4ffdab854427e33a67e943beb441eba567428099b86e229e4c0806191ed06e48ca6c8a31c6ef120d66408e0d6");
            result.Add("vi", "0f95384af952c5baa3a9a4e67a6167d310e26db745dc505918908c966ee3956e97f01252656c2805a9e30c0f4ff44e07a5f341cf1dfde55d65c9be8e8a2e142b");
            result.Add("xh", "39be2c9d387819a65f896b370bba9f6cb5106f8e8e749d07fd14c2c00328bb2d7dd99913204e23e889f2794096b87e097a27115532cccb49489983808e4cae2b");
            result.Add("zh-CN", "c4c52ddc94765bff1dd5313184afc8986d37f4817d8a431e274f2636c82ab9154439cb3eab836fff5bc223c9be3b887c6c95903b773db0930cafab3580f4406f");
            result.Add("zh-TW", "0aac6d15c81106c799850925a3521ea0d6d141df6096634ff11425e6b4752f07316ae59d6f39efdb87902f3054b45c8c487d1510098a622147ca716d79b16f58");

            return result;
        }


        /// <summary>
        /// gets a dictionary with the known checksums for the installers (key: language, value: checksum)
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/52.2.0esr/SHA512SUMS
            var result = new Dictionary<string, string>();
            result.Add("ach", "bca38fa2dce615e9a81c0701e39b2c1376879588ba5c00eb56a0f36d28b5299455eb8f0aee9f24878869e431bb4254b41d0715d184bd31b12bc21b27484a34f5");
            result.Add("af", "28be2ff1666cfa6580e4c8b5c5ac1f33deeaaa8a49d0d4aa5eb07d81000a623199a77b97d7fac21ea29582072f265612e5ecfd80336981a109594ad80dcd01ee");
            result.Add("an", "2898eea99b6bd4b34d09c88a3fbcdaa16b19f0d6a27c8cd6a3ee004d0108d9ec4df722d238425d09c77e6c8770188861adb9a9deae5b3dc777cf8f326953da2a");
            result.Add("ar", "11391cd6fe14743c89d4e4f7b9bac034eb9c9cfa5dd4c493a3ba267b661dd30bd2734ad6bfaed8e33f9067a6801ea2a6c2883df278cd734819978e65fa01b056");
            result.Add("as", "c50c8ad14b018c338fa75170dfb639a5ea0a84fb0c8ab3b85349cb833afb6df7864a4e8085dda883f08025156d56247bbafe6794acc0c0aa15538b8130021608");
            result.Add("ast", "8e15e53646523a2897345cfab255781851e005fdd7093b79ca547efdb2ef01729b67019c581b0c51087ca39d38919fb0feb6aa44fb02c646b1bc0d41a06f2791");
            result.Add("az", "df4cc728ec815a73880bf0e160f6c6d633e070bca82ea3c643a14be2f6accb1b5920a5391c24566825f453cc97081535a5c388d6e16ed5fed9c8b0eb373af423");
            result.Add("bg", "a2613f1fe49a065963b467678975485f16af6a2ebd2ea3ab32872b424090153d352363c83b091f5465d7b5e56800e2d0b47b7ab02d386c333502f14a1d4ef6f3");
            result.Add("bn-BD", "ec846429c8e3423afabb9f3a3e69891e3654b1c656fcc491479dde640cc3c17ea378353d5cc1fad2d4379de346f74d1f43c134bde210ab7c9b2373b0488152e4");
            result.Add("bn-IN", "9cf1572822f252825d09f64da787563a23d711db858c2cd0af8e6f7c3caf64e6f59499a048cc0585ef2583d9053917b2a85bb346d92ace928fbe345d1d8b8a28");
            result.Add("br", "b92f4b69efab73124fc0112d294465a634d88ebb4954901e3164347f67ddddc4bee9a0418b9a3d07c033961e077548f381cef659472b88ded0ce1170a6d3ea7e");
            result.Add("bs", "394605819ff360a4d7a1905acad461c366dfad7558f1024f0a85d12d541fc4ba1e290142a20ab803dae494953b757a7061f8b938d10994b887711c8110b480dd");
            result.Add("ca", "e54cee2ce32cb733aec0a14d0e10b7825b55d4add16dc17b91bc76cd366859631b11da16ded1981dbb39346634afa91ed6e7a75fdeef90ff4082b4590a0b6e74");
            result.Add("cak", "a75c30be64afceb2f92bce6dcf37a094d516dd9381e0def1db7615d908735d3f7004345ef0d68928327fad99140e5f4cdbbe956bd03d718f876aabb11f7359da");
            result.Add("cs", "83c7626d8a06751c18d15ca4e4c06d115bf841026e6a2432afdffa866ec4b0009e0b25b55df1fd69172d1488371baa2c5a619cc8c88a0dae74fb1a3614acc01c");
            result.Add("cy", "a0ce956f2297e480b15f929a5c7609668879d65ba9338239fa389d0c60be90496caf08b50972a78c9278f82c24366e3f6b03f1a997291d7c9f47961b7d3f86c0");
            result.Add("da", "81a23c7b00b14361d7ebe28ca78ed4d7ef69e063adaac5b3c1d1553aa8a103e5f1859fae2769bc0a6b30091abfe7f8120071a7bb40d7aa582ca7066cc08e4b02");
            result.Add("de", "57c31a41a0c4ae80828b73f004046bfb0a925ac01363c45bd0141d84fdfc12bf4bfd5d0475ee3234cbb432037cd09c86f4d8e42c6a645402c195015220bcf6fe");
            result.Add("dsb", "64deb3d98010c4af6ba1280c1fb7f9f76e8bfc0f6cf33635f7ab2c2e852b2b43134cf6cf0757fca3e0abf657260b1e315fb4392bc1d995219ddc853a5841ac10");
            result.Add("el", "2b7e7fe491a99126e681a327111fb7516937bfc550020a41fc256faf547d8560badde1596bf1e50a692ceb82167f922848ef86d8c6e9c4ff90e947b819926b26");
            result.Add("en-GB", "46274feb32d4c4d6c57874734a77fa96fa6aeead35ba0f3beb324278116a23065a7e25dc2e1b30c75777a02f19c0d74ab3d853823d6fe0cc1fc3c5ec298d52b6");
            result.Add("en-US", "7d00e2fe31b3ed478ececff85791d4bb728d09120f054e41a783f2f7898a0bfdb099bebb6426498ecebd7aa5ad24b619187a48c80094403d67c2c522b8ddc295");
            result.Add("en-ZA", "1e896b49493f80e1b2fa49b56094211ffe53834dad6f80678f7f03c725fb51d25f234fee62078b0d4bb3e3007f0fd890d4ac4234a00fab6be18f3406a61773da");
            result.Add("eo", "de6e19ceaebf504e94ace9b843a931206e4c0b51d33102a9cda31b6551d4d711a2a16ad2087a9d1efcb93a6175eacba404dd0b6b82ba96353e017c25a4a6571a");
            result.Add("es-AR", "92c0e1a6d37bc407b9d9b515ad1a1e084c2b1190a130bd770df3ee4419ea030f558d775829afc887c26bd14bcc98d0c77ea2724c8aa1b617ddb4bda92397abab");
            result.Add("es-CL", "c145c5de2cea0a2de36ed6a2013a46f3026b2eb110e648ad09c018b18c78ed7f5e1c328717e3705e98691ee09603d08ce4decb9cdb3ae162505ae6ee09ac081a");
            result.Add("es-ES", "2faaf6db5181af296f0577b6150fbe5cd312ce413c042ff8c20efca7742dfdb63495a7f802c97cc4b5d4e4145b3ca74ae01d6ee48fe7ea2f87ccb99939740a82");
            result.Add("es-MX", "0a0cd3e9e99e24833f5bb9ad39be2cbe3f17c4e5a24eba323d30837a65062d57227de45215437f797fa66a45835c0340a3af357da747b920009df912a974d204");
            result.Add("et", "e2768a54a3adc545d037a724ce76490387a8872d5d5945191b4ea33cdb1c51803d6ada1d238a78ce7c108052862253779842a6791772a44920d6931b5594b7b1");
            result.Add("eu", "75438cd4205a614b50af686780f3717fcf94e35b8cfb0221d8865e700a27276f539b93ccc4a8df92db45b1ee41543240be3196b9f312182b7744cd29303204d8");
            result.Add("fa", "af37d64b260407e6c7f597df28ffcdd15608e865f06e1c4e6316d6455974f43fb15d933857c235439ae28caee4a81f1e7f3fa3fb729ed35eca42f360ac104f9e");
            result.Add("ff", "e8ca2b49368a0dbde2ed7504bf5a41c82721a466509c9c8034130ec0c027878e41fb7f040cea2ee80953f4ee3a9ccbe303901a2eae85a9ddfb76ad5eced337e6");
            result.Add("fi", "f8909fcc49fc6596209319fa5c2627734de8e7406350145094e8f67af584b7cbad22d50d9a31f9102a833d42957a16715c1564538a3b2909ba191402b7134abf");
            result.Add("fr", "da51c9cf0e1998626dfcff3290fc22758769b6a9f51a4f3242a67aed1480cd03c8959cc61434ae4b3edb2546152106ffc9b394e8d42d68fed79e14014b72680b");
            result.Add("fy-NL", "b9b4f1467acb3d789754608d3e74499dd646d23f6ae618a6b99ee4b0affbd29b40cf4829fe563392839e94de7a76bdf11dbb1acec8a8b9952dcf05685ffb8f5c");
            result.Add("ga-IE", "c4a3760da2de7d6a0a47ad4aa282562e33993f5e29aade4f667a272b8a450b935500bad4b91bcc46f235d2b3c6648f231a5b44082c87d05b5283fb1dae82d8bb");
            result.Add("gd", "e213a576dc4612773f3d1069b69fcd8072df2191ae5640cabe76eba18be54788ecdc12f2f14f452323bb569cee723417107dca41e2c788e43e3f786fbf87fdb4");
            result.Add("gl", "72ce98073d7564bd0feb4406949a1ffb33b3305535dd7016f10de2733a83f603ceae9290390e811c396edabe658cd28cb2f35a9f25d8375f1ba9dcc56232837f");
            result.Add("gn", "1b11f5def2e339323ff30c7642e600399d57d30734d83b34734923033a9fa3875ceab77687b95ad089016a97e7513f8f79f55f10e29fc982ffcb60f9c9d4aec4");
            result.Add("gu-IN", "6d23ca96c23476addc4eea9b866ecdf1f4c9e2456c7de0e8cca990d6e1d537f1da251f8f68b2d309ef442e942735ede7439908284d4cab9062ed7c9e0062fb50");
            result.Add("he", "ed6118dc6956c8c8b3872bf6254863f9c62239a307d5135e5a569373b91d8910933be7cbab42ef4d02f9de11a3d869eaed8c45468775c05f33567e4841148f90");
            result.Add("hi-IN", "2be6856f97edf37a3c4d4af72909e64aff125210243ef0e58c919205d36ad2dcf42c75bba08f1ea6cb52e55e937d78183c7a32c058c09fccb959ece0688a05aa");
            result.Add("hr", "db7e5622f295ae76dfe78b17d19e14aa8997dc781d277226d2f26ca7faa1087112df20d76bcd9ebc229671e97cfb6f1dbbad2ad352658db2e026e02a40f00bf4");
            result.Add("hsb", "6e0c62e1008995611453990cc070b9a560d4e7d50ea5b12be4e1c5764d2aea3a36d41b1d4dedf96d8f76be12bcda7bda003cfb30a9f7b7b5f5b6af839e5ed329");
            result.Add("hu", "f3ccee870097af8db64f4fb9fa4620f891e350441ea93407b12f93e2adae935f102ba36a519d4d36eb116353f72bdff7df1613be6e714050d48dfafb379874a0");
            result.Add("hy-AM", "535e336c8f5cd1369fa1094f89b583dc21d592350fff22fd99fa8d36536753c103d1fe3e79dff50ef967bea6f67b95a4561ca3095b217c9c2b990df64e8e829c");
            result.Add("id", "1a740c0cec8bec1f02289cb8dc38f3572517b89d0b076b476cf3df27f69ab099054d10d87267f1995b298dd36947690cb8012989d742c8cb7abb8b843f468ed9");
            result.Add("is", "a0ea5306afe1ab48a6d5677279d8ed68982de7e37ce9b4b68b0b5ca4895a2929623969a7caac2543d68a18a3d589ee960c2548a7611d38a8e05e08eb862b98b5");
            result.Add("it", "6deb0588901b04346a2d272a8377efe00f31d83177a808d3ef2297f81795f8ab938ee2a3582cd088b008dbccbef5a90bbc938cb61ba3543ef5323065ea821fd8");
            result.Add("ja", "9812acefdfbe9f2b4ddd5905c9fc15daee76530e6ce826febafbdf0d1d761ea05e2b4a540672dfa434e9429fbcc7e672b83fca023bde8ca1a6d770881f46220e");
            result.Add("ka", "2ae5fada38c7be15d3676ad498d4e268a496500bfa899fc6ab7290eeb2cdc6245e594d31a757368714c8559be8d4ba150c626ad03c401d04de82cbe86a5727d0");
            result.Add("kab", "cce4c48bf4a5c2c45177919691cd3ec98389b6a7508388ec6ee235603a845cd20783e6ce823e5f191c610957ce5f47d3f9b82822a172068293967a31ff4144cc");
            result.Add("kk", "39ace15095570db89ba0e9e900999444cbe6b3173406b4e1410cf548555b641e00ea571ee9caa7206b20293fd2f229367e60d532bdd8ff39a8aeba73bf5c1559");
            result.Add("km", "06dc0248a71a964fa652c3c7f476de9d78e95173ac739c9c317630f83fdb5242d6e14fd636d134c07eb71ac27a6eb41fe91fa58f552bdb89a1f8a5fc5ce7d009");
            result.Add("kn", "341ac34b6c4b7ff578d9ba4e0467fa2cf57da62b42c6993b29255beef5ab926f0d68c3f56586c2a3ef32f2e5bac4c995b072baf78b0cdd332c96c324aef962e3");
            result.Add("ko", "d7b31653ebde3c836c788e43138ea595f528f71d4ca3e89af36477960139735fdea52d5144ca4a6e248315a1b1dd305e00158cdca08c5a771770f0f0d55a2464");
            result.Add("lij", "bfed050c61410fcb57df854a12a98b3cbe4e26907b01b9f359622f6b3a2001ae7dc185dfb49263d72997d4977cad6ca4086d1babc16c9c7a538505715d417bf5");
            result.Add("lt", "3937ab23f0bb32da064da07fc414490a3b434f6a912d09c41e16896d3db3ec6e0c2bb635adb475aa7b2fd04bb1ee7c0165a250bf704160e73234888607262030");
            result.Add("lv", "612f6da0c1df27ad8578cf6d6c419e1822aadb9917e14fc4a9c4dc19908e8cbf3e59986ffdc8f99b4570e579e9c92d0a79ab373602023ea6ea374708ba702f03");
            result.Add("mai", "46726a5ea0eee612b5ad0e32814f9f21dbf3a2a863b3c70187bbe0efdced26e4fcbd3381a41ad3977a6bacdef31f0c0f1677fcca5587d65aae3e5c2e83e8e289");
            result.Add("mk", "9f0a9972648623b67f8aadead224c7983a6be9835f12aa65ad400c250ebd51c84b742a3f37dd3be4ba8a1f1e46b52576fa1d2004b81cc0198a80be588290ae85");
            result.Add("ml", "932762314fc6de96e0943581104d9e891b9011185ad0c0250531c7ac9dc437833e2b0b480c7baa6776ca5cb43e484510c299890a140b0ecb12bb542562075129");
            result.Add("mr", "9d7b0cd5077621df312cc59a8094c8ca3b9822558cff42653d289c02031cca6800d773ecf617fee1e7dd7d322bb9f0ca408a1b66667fda27ef2f0eaa84aa7f44");
            result.Add("ms", "ef7a3aa317a225f46a96e4fef411e3093862966eb0fda7793bb7820d947ec131587e4170a55af324b1d306bcb0cc9ac9ce4b9c051368d495ff77c9342f095d98");
            result.Add("nb-NO", "aa56c0afd2b8488ed34bc93a0e75a0bb674c80a5328d13fa07396f2dca9e7a4c56e5b8dcc1328581e84b139e6f9d92dbf0574e7a63d495b5ace00536bc369e6d");
            result.Add("nl", "b0221a2452fca91c974924b2242896de6f3902106f3487c5dc5d1f94bc3541000cee8bf4c5a2da65abdf02c787be4ada3f9f56770643f9b9f52d5221509ba464");
            result.Add("nn-NO", "89b1632fbae4f8fab985d547e64b475c329cd78f7cca8e85cc671d30d6a2f88f170d9bd80e714a6cc79cc217515f3539ec0617e0c439b84c42e7f449ba16939e");
            result.Add("or", "99404fd18b40b7b34b1df0b8dfc9e7e948043166704147ce840018dec0ece83f788f33ec2b79ce204c1f2c92ef104a0a7b8c106a4c876111cd01510a402d5d1f");
            result.Add("pa-IN", "4f8fb68357d243fb398a0329b72a4a001f7fa9390fc340905ffdde8fc19f89b146d39f1ee694b689cdd2294a294340ee7f3672c8f15bd59c80a7c4b8527df9cc");
            result.Add("pl", "45546e8e8dcc94f61684bb298944322655bf3135e77d4efaf5c31a674ceefd2b22eba377a864682c55f257084ad56637ad0e72f62b3f7ea21c15d9d67e21ad2b");
            result.Add("pt-BR", "d9ecc5b6fe5a964af69968057484a7deecb3efa783ac15e7343edb3b68e36df505a901ebb9b2d4135fc35e86ab47dd70904b7eda3121842492ac9bac162b9902");
            result.Add("pt-PT", "d3616bedcecc2abb2a759bc1c1bcd6a11eba1c76570abf0c18dcdbb4147f2c5ea81a9aa6c3ec8a4ba80dbe3d28911e5bf59e9a15af33e0199501738d51346fe7");
            result.Add("rm", "9ba0ed723592521fa1a4f9a4e1c6ace4152ab4e4936fd070914924920e3bb86a6d61f8a1355c085e7d97b39acdc50b61c9c99b75cd51387ba012dc0cf7826f49");
            result.Add("ro", "d51706a07afe4089284a66b4d54013c7a19ad2b9927c5fca4d266a3dc46f1d55cd50f430a50d4d080e8e871b840137cce932ec31def97b9c6b684723831b6d69");
            result.Add("ru", "5ab1ddc81e35d14e9ae357b6c34cfe0fee976bfb827d5481119e0f791d5d93720cdf3c759e8c6fbe9aad4bddb969d969ddf346542cfce266ffad9a7e79bcfdab");
            result.Add("si", "5ece0764dddeb8d514723f2c9ce54d49b7b3034cc76c4020957ed022170d92e5bcc1dae5b14828328beb74317436a6d7553945fc12422292f747ec7ade97ae39");
            result.Add("sk", "3a0d46b8b7cb24e620c324bd0bec058360f329afbb0dcfd88bc5faea89c10d03583197a760694ec31e44efc27bed3b0d917d87a3d876566aa11a67f4efb81416");
            result.Add("sl", "86d9f5e01d8db02804aa7d5bb9eaaf1ecd453620e2ab8746839cec437e891364dcf1bd55abcc1e5a5aefd57a8b4529c22854e7d074ad8dd5a55847e8bc317b32");
            result.Add("son", "b54753b543714deca091daee3be348c66f37c4d8cf3a7ee9ddf3f137dfbeac5d60f82dcbb770ef63961a989a6f32568b0bdb77489e6105f00b3e3b45aa7d2e60");
            result.Add("sq", "64073e8ab49711777f8b6044921cf16aacf6e133b66869e8b008644dca64725ccdf2fb0d8dc35838aa7578c455b20d3a02d378b63f132b130f8ed03f918f18ae");
            result.Add("sr", "a52fb0216b70fb7eca5dc62ffa26381a39c98e40e273e9e76495d2b8c41b70f67f1452b68c6b73e228c4114b8e2346a2209b2c3350225fd786a4813c1bf79d17");
            result.Add("sv-SE", "095558ff49ebcc35dbbe01ed358be93af97b4f974216f0ca3761e9241ac7f918c1d6e5e8a466aa443106e1789553bbfcba0286577c9dfabe606d1fef174fbebe");
            result.Add("ta", "8f4fca316e364871df41095638c046259d4448d34ecdf0e8d838ab4f3130ceed3d0af54356c588389fb5b320c8dd7ee5bff51bb9fde65293833585a7c292c3a5");
            result.Add("te", "06e73f3dc42705604b112aa7b712776fda7320c4838ae9ed7bfb083792d8ec06f12db958f715682555e2291fbc3929719a700085827d1401fec9c37b7c0f2c0a");
            result.Add("th", "b06f41614731da49702fe949916fdb7095e7ad786edc1bd11e23a8ee60ad76f8955ff85474030bcfc1158d58e4413d028e166e5d99dfb57d51b37e285843f3ea");
            result.Add("tr", "c518605c31b8ce00cf849ec97b83692d108373dfb16aebb3a9100284bedfc8db84f972933bd823b22dca58101304ee02c90b9f80c42f028dea38aea21cb0c5e4");
            result.Add("uk", "d61a7b2ec423f4e5e7efbeda4d22c0c1b706681e6df502707a12c7aee6e9dec19c05225aa64767704c3c47f34a361bf6d5157da0b2f82e2ede451daf2ad7580d");
            result.Add("uz", "62bc7b91afe56b5d8b92bbac0711f66fcb8175e503a552db1ef30c4cfd09766e0c7c4d71e1d35b94685313b9521f3f6fa439c875542f633a7ad83f66ea18dfc2");
            result.Add("vi", "7f4133b1cb86fa2182cfa9b3944cdee212305062ff1480eff2e13f72d1f7542669c4132a34d80caad2033821963cc9cc1162a7c24b990a061bf21fe9c3c5862d");
            result.Add("xh", "56ef6c92b1c6864baaf5ce6155897dd5a585d813b38221c307530eb8a3a9b1c4fc4641d57ef43dd299ea93bc4833aef2f9a74b0192caf3de7e43ecd9caf57128");
            result.Add("zh-CN", "f13df5be28a8023880dbe9b0496412c880905bdb50a5054512a361bbbcabed50e9f380aec6b6effbe6d43ae5aaf0599198ad744e2ef46615d6ab0dd75c9d09e0");
            result.Add("zh-TW", "a3baed156be73bc5eb3555965647357dc1ba203a45e1bbe68faebd5da946be14723c6602284d21c148710e52f91c775410159c0e70a1aba25c991b41eaf0292b");

            return result;
        }


        /// <summary>
        /// gets an enumerable collection of valid language codes
        /// </summary>
        /// <returns>Returns an enumerable collection of valid language codes.</returns>
        public static IEnumerable<string> validLanguageCodes()
        {
            var d = knownChecksums32Bit();
            return d.Keys;
        }


        /// <summary>
        /// gets the currently known information about the software
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the known
        /// details about the software.</returns>
        public override AvailableSoftware knownInfo()
        {
            const string knownVersion = "52.2.0";
            return new AvailableSoftware("Mozilla Firefox ESR (" + languageCode + ")",
                knownVersion,
                "^Mozilla Firefox [0-9]{2}\\.[0-9](\\.[0-9])? ESR \\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Mozilla Firefox [0-9]{2}\\.[0-9](\\.[0-9])? ESR \\(x64 " + Regex.Escape(languageCode) + "\\)$",
                //32 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/firefox/releases/" + knownVersion + "esr/win32/" + languageCode + "/Firefox%20Setup%20" + knownVersion + "esr.exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    null,
                    "-ms -ma",
                    "C:\\Program Files\\Mozilla Firefox",
                    "C:\\Program Files (x86)\\Mozilla Firefox"),
                //64 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/firefox/releases/" + knownVersion + "esr/win64/" + languageCode + "/Firefox%20Setup%20" + knownVersion + "esr.exe",
                    HashAlgorithm.SHA512,
                    checksum64Bit,
                    null,
                    "-ms -ma",
                    "C:\\Program Files\\Mozilla Firefox",
                    "C:\\Program Files (x86)\\Mozilla Firefox")
                    );
        }


        /// <summary>
        /// list of IDs to identify the software
        /// </summary>
        /// <returns>Returns a non-empty array of IDs, where at least one entry is unique to the software.</returns>
        public override string[] id()
        {
            return new string[] { "firefox-esr", "firefox-esr-" + languageCode.ToLower() };
        }


        /// <summary>
        /// tries to find the newest version number of Firefox ESR
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        public string determineNewestVersion()
        {
            string url = "https://download.mozilla.org/?product=firefox-esr-latest&os=win&lang=" + languageCode;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = WebRequestMethods.Http.Head;
            request.AllowAutoRedirect = false;
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode != HttpStatusCode.Found)
                    return null;
                string newLocation = response.Headers[HttpResponseHeader.Location];
                request = null;
                response = null;
                Regex reVersion = new Regex("[0-9]{2}\\.[0-9](\\.[0-9])?");
                Match matchVersion = reVersion.Match(newLocation);
                if (!matchVersion.Success)
                    return null;
                return matchVersion.Value;
            }
            catch (Exception ex)
            {
                logger.Warn("Error while looking for newer Firefox ESR version: " + ex.Message);
                return null;
            }
        }


        /// <summary>
        /// tries to get the checksums of the newer version
        /// </summary>
        /// <returns>Returns a string array containing the checksums for 32 bit an 64 bit (in that order), if successfull.
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
            string sha512SumsContent = null;
            using (var client = new WebClient())
            {
                try
                {
                    sha512SumsContent = client.DownloadString(url);
                }
                catch (Exception ex)
                {
                    logger.Warn("Exception occurred while checking for newer version of Firefox ESR: " + ex.Message);
                    return null;
                }
                client.Dispose();
            } //using
            //look for line with the correct language code and version for 32 bit
            Regex reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "esr\\.exe");
            Match matchChecksum32Bit = reChecksum32Bit.Match(sha512SumsContent);
            if (!matchChecksum32Bit.Success)
                return null;
            //look for line with the correct language code and version for 64 bit
            Regex reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "esr\\.exe");
            Match matchChecksum64Bit = reChecksum64Bit.Match(sha512SumsContent);
            if (!matchChecksum64Bit.Success)
                return null;
            // checksum is the first 128 characters of the match
            return new string[] { matchChecksum32Bit.Value.Substring(0, 128), matchChecksum64Bit.Value.Substring(0, 128) };
        }


        /// <summary>
        /// lists names of processes that might block an update, e.g. because
        /// the application cannot be update while it is running
        /// </summary>
        /// <param name="detected">currently installed / detected software version</param>
        /// <returns>Returns a list of process names that block the upgrade.</returns>
        public override List<string> blockerProcesses(DetectedSoftware detected)
        {
            return new List<string>();
        }


        /// <summary>
        /// whether or not the method searchForNewer() is implemented
        /// </summary>
        /// <returns>Returns true, if searchForNewer() is implemented for that
        /// class. Returns false, if not. Calling searchForNewer() may throw an
        /// exception in the later case.</returns>
        public override bool implementsSearchForNewer()
        {
            return true;
        }


        /// <summary>
        /// looks for newer versions of the software than the currently known version
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the information
        /// that was retrieved from the net.</returns>
        public override AvailableSoftware searchForNewer()
        {
            logger.Debug("Searching for newer version of Firefox ESR (" + languageCode + ")...");
            string newerVersion = determineNewestVersion();
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            //If versions match, we can return the current information.
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
            //replace all stuff
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
        private string languageCode;


        /// <summary>
        /// checksum for the 32 bit installer
        /// </summary>
        private string checksum32Bit;


        /// <summary>
        /// checksum for the 64 bit installer
        /// </summary>
        private string checksum64Bit;
    } //class
} //namespace
