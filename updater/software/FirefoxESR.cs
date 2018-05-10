﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018  Dirk Stolle

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
        /// e.g. "de" for German, "en-GB" for British English, "fr" for French, etc.</param
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
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32 bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/52.8.0esr/SHA512SUMS
            var result = new Dictionary<string, string>();
            result.Add("ach", "587dd3605fede83f2ab68c2b8e97a39c83b71af1e5798c370b0dd4de32d5f573aaf2854feadb1de5a9ead7fe4885fd16c5eb5849f6a45dd589f84f530e215012");
            result.Add("af", "1d892cf16d43c2a89ed9708464c98bcb34168f6a74a20c6d7df7c50de2e55f8e6346f5b4685ff82e0e964f4d0be82f5fbf0fee4e1e76696b6bdcafa886139773");
            result.Add("an", "98619833d4b5c5c1972d589013e58dcf328827a8f9bb4da98ba1932e4f0a4fed6c8442c95eb2cae4c92ec3a3913a997c4752209ea046ab0639a25ab9d11e8265");
            result.Add("ar", "be0729ff28901b8ee84fe4a180e1296cdc693bf30e52598f9255860f33b7cfeea88a43a5b6e2add4f31262bd11e1093f5c17be81b7ad9a3818df083d4c3c1f96");
            result.Add("as", "7d0e1d3e3752c474fcdc78f49f9d574f5dbd284109af9aa9fb308aa003aedd009bdbc57c048cfcf202dac1afcddbccdc08f2425770c9fe9ca266824b0ab7a80b");
            result.Add("ast", "01293c27a91d850c9fe9dc98c06bc7b306d087413855a1646ccb0b91d5d7c16952b8bf3e3102472264c7e0fd56231fe5f35a5e93b98ec0f643b92df4ce0106a2");
            result.Add("az", "9932332de6800e23b0ce02844ad6ff9dba83c58c72b83c4a717d44d11ef03b343e809e39b179d5757ef614398a1d82d488cbdb548b3d9e579a4998c70dd4895d");
            result.Add("bg", "b66e48474ab285f9246843aa777edb445f0be42f2bbf9579f5268fc36b83c2e493703bd1fc9d0865a527e6c16825fd1b8a2b623ac895051437c3641e07be9c86");
            result.Add("bn-BD", "d79285ef9cc14b911092259b4d2c61558b69ce4fff4631d427ed74165916a6c74ca14e478d10f07a63def76ccbefd507cefae36ae0e075c5b5df6a07fbf8730a");
            result.Add("bn-IN", "76caf6da5e16fda33603b4457e5817a2ec671745d3844b31aeeea5db372bf6431f7f2631e9e578ba83e03dd3184d7968a60d98abd591ac1b866cc6c61addf4b8");
            result.Add("br", "189d873abab2e7b28ffd0d933c355b6ad5b4dd29a296ed5b4811ef9ac07ac542669fa893337b25b32fa034c721f9650e37116ef531d345c7c9aab43d15d8a02c");
            result.Add("bs", "53ac90ecb761a35ce21c386049f8883a2dc9fb4e4b32d6fcd4ed1589cb512d9f756e09ce572191c8f258bede4e4a47629bfbc20524e84500f6939456aac00a2c");
            result.Add("ca", "45a102b98110d66f1ec2d3c43aef5f16ea3b38575c8be85673d737dcacafdc5666326a2c312c52705ace6cb7fd74cf8e5e36532b664c6a95a2951f26305952ee");
            result.Add("cak", "23c6354c929a8da06746c05d1a4b2634a5f09c8300ec05527379f7998cb691d9a772f71d93c667752ad17f9e6743117db2bef9cda955a5d98e0d8e8636887765");
            result.Add("cs", "b828eb4a438085008413afeda9250318d247b81882865ea6d5f4156d610945a046fe19615fde86fe1cc2ed46774eba76eb45014a9a7ef2bfe3eb024cee0be9f1");
            result.Add("cy", "9e7f17a95cb346a15400ce5c1cb281647a90cded26891a09e201f3b3e22526f3500398482935d4d353fc08bd75dc20cff8ce02bc0eba496e3fccab33277bbfb7");
            result.Add("da", "3f837bc8d29521b59c1501c1f7f6b99c669aa0b43d763169299e689e548aa2527c2bf5eee0911256a6c897919fcfd9311bf98993d6181b3b988cdb6959a08a37");
            result.Add("de", "81f727ca1f5f777bea62bc276e3aa762d9ad401a1e11f62cf6ba3bfdabe5b74ffaa6b71be38a01812cb1f2c686c0d39325bba68b4a08ca6ac4fe10c2cc1bfb95");
            result.Add("dsb", "8508a6ebefc0515dd96de5998088842fc1fd7b131a81d82666ddb7d357d6cc1c8db02ad76e3873d1922e1d3a2b057e236beac82697249de873dae3c85665d770");
            result.Add("el", "a830704062c666687a800a76d558e19e1eaaf5317cbd3be966245b70ccee1d82291200ddb29fb4f961f368a615a9b046f4df0ac868254035a8100825be03383d");
            result.Add("en-GB", "b14199b7a61e2c6f04e7f100682310b3e16a6e4517f3fc0bf268d8c9402114fc3eb4220199c941af7886424fe35f12f18048b1d78f863209d470f0225a77523c");
            result.Add("en-US", "9509908e7e2057c267b946169c85c1577fbdd3c065a118cefc25d1870a87a37908be4031f1f96f0828868782fe5cb2fcd03e24160e2ebee3161e70af6eaea162");
            result.Add("en-ZA", "9c2d2e4e0f24cfe5e70e99b78e1b394221d3342be5843baec8d83ac65a9f3f929154260c2db02859bb0a60b1e295d428f5c07d34557225f070398c71fdbb9ebf");
            result.Add("eo", "8d3a367a2ec72073e2b4b25bb381201d0da60a7877c5cc95fe8c99f1313e23d0d7d72a3842a22306cf9f89e1f6a01a381b3a465da671389befc3c2330780fa11");
            result.Add("es-AR", "5a8fa58fd1d60585d57e03afc7990a338830a3c229778d820ed30a117191c8b47289cc024151a3969eaad980dd3e63f61b0c1260805edc5a31c557d19bebfe53");
            result.Add("es-CL", "8afae515c0987ae482cedeeffc9d360c857c10346d1d89420a410d4f1c68f6c0c8c795b5393b5f848a51a255a63fcb9dd3e52ce9c5556707723a60fcc165470c");
            result.Add("es-ES", "dea47a0642ae09ec3773baf53ec3dd3906fc9ebb6368f3aaef8b295ca08bed6f0b358ac65e1af12de45fed84f950ee66785c539b35b49b9b668f1fe57222ba91");
            result.Add("es-MX", "bf92d61234db2fb9849fc82590817dd35bdacc841ce9145b80002a9593c27ae7ab21951cc645f8b7a51a28a02ae579f0af850825d0c17d980478283460bac75a");
            result.Add("et", "8f9bf84614dc7ef45f75f6b10d41877b5a0063a4d3d952aaef950f180b9aa2ed79ef4052588ae69350bae0a5c54ee6c79346060887c83891fcadec390fd0c437");
            result.Add("eu", "e2deb59bd1ad5f23fc6e7a86dca88e6e745c58887b4394b3cfb37c327d26232c8d3b4c718f9180c09403beef0bf8299e089f9ebd857baf078cab102c5f6374ef");
            result.Add("fa", "3213f9d715e480ebc586ad217fc0ffe58b32c14cc9522a7c3adc5fe0e2c1640627799c333dfc6da47b7c9a32c3331c0221716a21272b6b14448da4e494e26c36");
            result.Add("ff", "68a48561c82324ca101e012fa79a98aeae267aa91827d743860ed18c637bc64e65c9dae9ed103a821b4711a9bc8ade26cc545710c3e732fe05bdd778cee384b5");
            result.Add("fi", "4d77b23722558dd5b7fb019a6ae958515f075b832bd9f1d457fd5d85ad938ab83c0012e1439b2daeb056c86cfddce8dfaf0295c89a13dd43cfb39d068f3ff637");
            result.Add("fr", "b374c29f3900935343020a2cec3346d4c480d0b3a4f62654a7781172646ee5881379485baa13bbb0d3baad10549e46227a25c9dd5bd42aeab759bd3997728297");
            result.Add("fy-NL", "b225c5aae6510ce7b14f09fca15785a99ce610465f2c9362ac7cec63bd61dd2e4a0db075859e5bf762e0ac23d04d83f42d6b09a039f9ec363609d62881ca927b");
            result.Add("ga-IE", "2975e7629afba7c9f990626659c6c71ffe5c0ace7f1ba695863d873550b6a3f8d5e5268721733d12c6047c62962abe7a542044a1514e0b175d1a42eb4a28a915");
            result.Add("gd", "44e23916d156fd958def0a794b6771b1da14a6c45021b141335840a1f7c9a20cecbdbf763babb5ff1ff5c8c0299b895a5523196448b3b3d813c1e64edc668222");
            result.Add("gl", "299d0b9088c75a4acf9a6d61f07e8423adbaf904c3650b966d1880c2b9e9f862d584a5a46f738419dc20c79484dae848ad9a6d14385ee0548abde8ce22d376ac");
            result.Add("gn", "68701e94fad1d709af12029a60ca0ab0e9db229b0f1915037376baef0dc1533e4092c3ae39d6ba5afb4b1e814d490e197e17cc48cf2ac217e9266c510292a1a4");
            result.Add("gu-IN", "446f078856dbc30dc41f3410939b9e43121855cdef6e4dd62fceda21b6d7f7eb294ed432cc1a62348af94c2f318d0ef7eaedd87fdcd984c7f1cd825649810ff2");
            result.Add("he", "c8464e163aa68142f343610b767b06bd95f30f103e275e7f6465e04bff8a79da6ec0a374db6ffd399dde5c5933fc49170f5615d5f97d3ba4c7001863a4c31bb5");
            result.Add("hi-IN", "10277289c7640fe35912f7ac49fb963955c22fce0abd8a4dbccc30fa216cd43aa96bf0691d6532d3851506812f6a526803f35b0958f86fc62d84bca841955247");
            result.Add("hr", "44b825ee465b8d2d994705005371a30716f26d6669839e690145274bcc05bf221279f9d815f21c6f78555ad3e0500b208e32112f26450761ff8e43ac2b6973e8");
            result.Add("hsb", "f46458a1813aab3203a0d336d385a54bd6ed1a7a5da9696ebd532cd6d156fd05b3de32730fac465925d5e0942509a5e8e572758a1488539aedcee4ab5ff6dec7");
            result.Add("hu", "5a71b50240d3238d2cac036a96b6549eda6ee0afceaa43767a01a19147e922375f4e4467a8c3f1ed5895770e3b94df14d0a03719ca2d1f2396538897f45f1f26");
            result.Add("hy-AM", "d4828ac0827752536a5c1f610582274ca68b2a76d82c2b4bed477b5a426b1894b1828cb05435a90582597e0f1082030165ff68749fdb1430e7dd836a8e29842b");
            result.Add("id", "eee847f1a976794ed89d7474902f1fe598c9f511e680fb2618923a1576276548bbb497797edc9c35bc38767baccc1457e6ba5267e475a4da5f1a62dc9c4e16cb");
            result.Add("is", "e816986580b66f2894193a3e59c8c28733c50f2f9dc4152356bbaeaa56403416379ce8c062cbbf388859c94121f4eeeb18a01e93afb6db44215897bf7418e57e");
            result.Add("it", "f4c0f15e8ad0727417f5eb01f9d99ba235f83744db836c5de73e1b374de5ea226fe77ad701e9c12856af75472765c85291537309772135214c70625c20740acf");
            result.Add("ja", "c58ebc20da367d0137a1fbe4daea10cd5e57fdaee3c5e590dcf48762236e3e124964fa95d8365d7dac30b2f98255ad9bd6adc3d0039cea567e1aba1c5bce8c79");
            result.Add("ka", "27e6535d7f165dbf6c9c1a16c75094119a00ac6cd14dd1b6d72efffbc96525a5e84e976d6abe1c2f441a15e594c6ee9e85ad7fe2fdea4f884e5e3bbbc27a4a5b");
            result.Add("kab", "4ddd43173e2a03f484ee4d5896e9b4d0f0fe48be74dda80d8263ad2a0791372f0603707b48eacaf6fbfba164eacb190fbbd197fc19248d9b4b03ffc2a3c642a9");
            result.Add("kk", "6db2b05bf34d851909c74d04f32c6e9013c0ecfea237e760dcb482e14278bb4e78cea12077241e3ab73e1ee7e26a409ef4bc64489b64619e0393205c6baad5b7");
            result.Add("km", "9e0d167da555f604a3fe49ae3d868297aeac48f72710868d9a9e1d09c0116892554bd44245097774027df8d391d38d4948047e00ad0aef8d6a568c1f26adc6d2");
            result.Add("kn", "1d4388be9d6bc2bb0ed3f672904f9f892aea0ad5d09545e02f0576de4bb8564b6d050a285cbfd2a1fe2ec6f91f295413c26d63beb6bfcddf64c37934548777d9");
            result.Add("ko", "675d9d3d555a4319b7d2f860a0aabbb2bbbb925f0571a4f9ecc4465a3f0e01f476aed3acf9764a8406a888e49e1c368dc59e5e7fcc04b3f0edcef86d8e6a93d6");
            result.Add("lij", "686e9de782038c4e7cc4bf89fcb6d6779178bc7bc5b3b4715ef61490cb74a2f5e66132a13a5af0285af350b27392789c235e1bb119f62d2ada4f3b9b26bd353a");
            result.Add("lt", "8343a140527fe4e93e4ff3a53c06bdddfaca1988c6c06cd9d26d7e479fe08b483937d6f0139af0fe820bcd33b44522e6872daf2f260bd2ae4756aefed6c0edab");
            result.Add("lv", "31fd7b8d5bbad1fdd5121d57dce37729862f76e5b23f2501901d3f30d2272a98b6e07912ff9b998afc6800efcc0ba7479049693b83f1c4f1a9ec56301afc8eca");
            result.Add("mai", "3179222f590fd37812d1965b7732c896f4697208ee09262ca37e77e2fdf885a365e130b0beacef6aa42e217ee259a99b3896c7779a149fdedb1de5c77b2dab8b");
            result.Add("mk", "01162bce3a8283a86b22f467fff28e2b047fc8c3823ddcdf83046f8218bcbb159156cd1fb8949c71156cd4b7140080cdb064e78a8288a5f246a2bbef550c4b06");
            result.Add("ml", "db323263310272e1cffb5a50c86e45cee7da3547bf901575025b237055f9ca607df0b4cb5212c93da3635783b350843e0bb2dfa71cdda5e4915cc6079de69caa");
            result.Add("mr", "ea132c3065138e9574452fa20e7df7715c98c299c5ff998c52dae5711d93a70975a50d89bac1ca73e6c736722e435a01dd3f038676f925a215eabc9a8434b2b4");
            result.Add("ms", "3c3954281027979c6fa0a20a475801874189209fb44ef021c8cd4312bbfdfe1d3b89b40b6724eaf6a7854dbb55a93aa1225c3086c535ccd8ee21cdda8f79fd90");
            result.Add("nb-NO", "851609f58eed604ad777de6bdfbf1a2145361e5ebbff54484aac7268259a8af4d0c3c0be5528b568cb271d4c35f1ce284e27a86ca71f0cedf203cbc34613d884");
            result.Add("nl", "698e4525596bfd5d144a3bf923ca9de6b0d1b2860fb1036c8f0837f1c35c1dbaf447012bf623492f4cf18f897ea1e279aca15df8229e252f8c135390a53f9437");
            result.Add("nn-NO", "43f553737a15b534ac7bf5c345da7249210f6b550e6058b87da0feaaab190b32a1e844cb92416e81d8e2959272eb0b106b9b043aa631e263735d3c259ddfc4da");
            result.Add("or", "fa409c84394ee8415929c8897a8f05f39199cff20a2dfb233ae7b2037cfa56b44fc63f1620646d3f4d0d96d10c5abf943702a05a247137a248f929dc75834a0c");
            result.Add("pa-IN", "27886a72026bbc3e4b9a4e682a5b041a1de245cfe452dd215ed34032c74d1e7c10688317a18ef23663d52b2d7bfef62eb579c3837bdf015a86004b73116d52a2");
            result.Add("pl", "ea3685f911c964d030bb4d4a7eaf1ee131824623dcd462f1274af388988e6a90742e0a14dd6a17ef9dbfd7c26481df072e211f745309dea0f3f90d1cdcd8dfdd");
            result.Add("pt-BR", "02d697b7ea2735ec50d03b6c21a874e3453f86a03665645ad52b19734792f6b30cbda682d527a05886974fbf0c31102c92c7b679402955649c0d83091af05d6b");
            result.Add("pt-PT", "462574c812660d9716056648ab2440c6d8c88e6ba19ce1e8bbdb7be2bf92ddd786a8c9c5bba66f775d4512481e03d2c576e8430468adbe806b29a021d65b4dbe");
            result.Add("rm", "f54a3408ab6cab53ead53eff1a8847d96ac2c3f39bca8acbe883b222e8228eec7f7304e0d6120a23b09c744e8a5c134ebc0b4e60283b49807cb731aa81b09e33");
            result.Add("ro", "76b4314e0889d379b6f53e605a1bc95f3b511f5648dcd17d508d409f551254942cae5265532ebd597052c329012c31fadc747be5dda20f84e3e297e5a3770adf");
            result.Add("ru", "bbd51485ca24914e95979276b54918314d027c22340d988f95b4d798bac7fb1bb5d80fd4c168241c004b7bc4591b9e2459e2288dd512d14c2a2991f6e92359f4");
            result.Add("si", "8337620ff55d3a6f7c03e08d4bbd9883d8e72cf1b227b24d277496d9d1a1b70674436ef7300e4dab469e510b58b310b293a893de103d736d331d4cf960dc3ef4");
            result.Add("sk", "d8f71dbdf3936c963bd210db2df84c1b13d3d29da417272a7a176f8ec385776c974cefd20b3f5099c09bb00228e5a11bbe5db63349cdcd5caae82db8e72d93ce");
            result.Add("sl", "c150a106539165c12df5057619f7ceec44349ab915fafb1128ecd8090255c88cbd9b5b4ab6cad78e83289f16ef5726273ba9479a31a5e5601b9713d469be01c3");
            result.Add("son", "343fdae1db699dddd16e875b3354531a443c11b94b991a41d34ce307db6296932c6dee65dda870fd2d740de0087b2d09b88899954d1fd167b7f15fbbd14fa435");
            result.Add("sq", "617b0ae94af2906302dc62e966aecd934a39b2ae71c441b064a6096002a17afb203d7cfc3100c0a9cdf1a61066d327a1aa211cea765d95038dc40ad10844d5af");
            result.Add("sr", "5f2c96f1f0362b452e907b8887c04a6e170205965abd5a563a41518146b4e3a200bfec98b380895e632a743eea87a9d3f78191ac6e2581f35d7f1ccd4349bef5");
            result.Add("sv-SE", "f51b67934361541ea0659e8eba5cb460c2721ffc193fd65d878f424fc9ca145160c30074734c3aca5122d90fd1c07f717a25ad659272591737c019b01024b04e");
            result.Add("ta", "d57b13073f5bcae8f8fbb358ff8755b25b65bec5d51a04c7c5cce54c257ced43c0cd190e59b668e442e0507f76a6f290b1808bfce0c35cba10a047e444e27eac");
            result.Add("te", "a000ca19a39d4a3a3361d2234d94a55d660b213c7abfdce2d061c7583d66e2c16bfb220baa48584cdb8977c1d2f5cf2b442e2b6c42f76eaea786fe7ddee97a09");
            result.Add("th", "91438218bf95e121d9208aabf1f26e09ac521cb386aefa581669f1f5cf6a1a035f932a3b3d2bd74e5152d40a9a1ebc430e008100ac709d184c04531590746e25");
            result.Add("tr", "99dcfa56d62d0aca1017c3420eec9da2b8f9115a2ad7cdc4e5edbf299c467cd363a3ada6ffaca31e6f342a485ed9b4eee9bdc667c6524dedb93ada59fdadedcd");
            result.Add("uk", "f2d8bfe42996502160b8db85dee6789867039d0a9c1a73e95a542084a14980c1a3953b9fba608c0a7a3ca290a85bb37c5c5b4452f5a2bc9f72845f8a1c52a067");
            result.Add("uz", "7a8ea2ec35ba2a4c5da0e8ad00bd2228130100095afa2c9553dd62c0e8b6139604993dcac1ddfed3b499daa3ae67ac0d7b14655a61d265b03f56f3201232aa0a");
            result.Add("vi", "0c446d5c252504eb5509c25a8107b5c21febd02609546ac541d6aecd2a8303df9965dececea28ba00f50b13220dd8263fb64a9d6b714ce8aea940ccb05c959a1");
            result.Add("xh", "010032b237919c9673f48a520e85c9124005607b507fc6202e8daea7e4e0250426d072a319a69647d8d79caac4d5246e8612e7ad4462b49c9fce201554c4a869");
            result.Add("zh-CN", "dba905ef81f0d80c54876a4f7e8a2d984c75a20f9b916910c24562f0f24adc005141617ba4887f2d5ba3b4f8a958cf685ba8b3177ef5f6f99bb0f0655a6185ce");
            result.Add("zh-TW", "b8c6fd4e73441d41631179167ce2665ca941353779c4b1cd04bed39a336175ea43bd19325e20da95fbe45ec85f09d5cd478a08192108e5c853f3dc9e7760c44e");

            return result;
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/52.8.0esr/SHA512SUMS
            var result = new Dictionary<string, string>();
            result.Add("ach", "8abca693d1dae2c8df534ede6831091f67e681a79dca3931cbecf54019c3068f8c2be1e70314e3a9fcf5b5dee8b6811b631b7364b49fc538d4d1d4da0f77d4c0");
            result.Add("af", "a201359cdccc0227626b25bc56f82aefd4542913fb8fe6d54bde5eba81166321a06bb0c7cceec51ac66ca74255dc1bcda467eb7defe9f511ead2903437dd95c9");
            result.Add("an", "03efbfc54f7c90ce21fb1b1ab6564fd7203d5330c76b4b4744fe9bc76fd004e0cc60b559035ca190926bbf8af9c6ba02ec66a81cb9c99703c0e1df86ac4cc503");
            result.Add("ar", "0f321a5206d191f8b3e5044436616f0e24d9fb478022668d1095b8fc4a3fd5895faf0938cc72a7a51d2b77443cd092272698e031eb9a9674f139db8a2da5f652");
            result.Add("as", "9db967a8221eafae695380de5216be9e2c6aa309383f4976977bf8c44b455d00a0cf905c9048c528aef2aef4a8a0cc81bcee77b9c2c7ff02dc1c53b78fa400b5");
            result.Add("ast", "d250eeb5147cc52a50203c68354eb06fea24b75f4126b4c5c6f8777cb506de99ac41bf6d8f3bef54b15db65b6e11ebd5428a3ae4b1f70e840846f8187bdc4869");
            result.Add("az", "efc62523d4c93661624ce2f295bf98e5fa85c8d971faec1b7869d9b9d5e0ded5fa091e299dd8772d6bba372d994f2c52dd527ac375f9a35e41881843b02c3b1c");
            result.Add("bg", "adec6e3c756b16490c3babb23788f088dff638922635a6036bb8f0603e933909d95a88b31355d52d5b7ffefe82e0a7e0975e82e52f3380133d66fc86b18ca060");
            result.Add("bn-BD", "2a28ae9a621b1abb738888dcc9cca6808ce157e2826b9c88a02a322c4548f3f1ded3c5e7af1a552d43f7013a80a11709e9313b7d7a0dcb7778a3e6e2bc9ceaf0");
            result.Add("bn-IN", "cac3e7f4ff2c2d79d43d23fd2e2e0c6607634dab84ed0f1ab4b057e25e7e2c45962b6985a13b18be9a67d8eefafc896e234eb6740c8e0ef99d9fbfa6abe498ae");
            result.Add("br", "b9d8872b657fbf63f9e108d46bd89fc3103cae3c823e743a61216b1107600ca45be9412ba95d89244c2775c214ab6595c229604624279a22461dacc8407ddf34");
            result.Add("bs", "66b00f75536618eca50cb27304d62b44c192179b0814bd8534ebee121dca1da40865af9e21ab8862c0260de6717a472202f9b5552db0053e69d04377905367b2");
            result.Add("ca", "6519c43bc8ebdcdbbe97da728143341482bdd1fcf6399f1b8c2a6215bdf98e36eb6330a1cd1378559e3145db48ea03c6b88985a8c7d27609d05d7408681797fa");
            result.Add("cak", "6fb990d42cded561486b98f094fd4186a1fb4043c4a5ec53c6aa64ebec0c6054b7cf1952251d0fc3001ca5457420e0bcc578af8c95957bd9c0205bc7390428cc");
            result.Add("cs", "61bf9849f98eb92d8fdff09d01b48bf10ff3036bdf2ace28846b0f628370a3ea359d724876b022eb436a8c7da84696c6bb45008795d6d3f26f4a58b163f8f891");
            result.Add("cy", "a67aa1cd554c56602bc373d89a75759340ff048867f0259fd8668512a1a2d2c60e7170f6d1e16963168903f4be18783eba149c96066b36644febbc077db4bb66");
            result.Add("da", "37faa980b13e2949563854e6ebaac287ff266652f6e8d3a239c13d9ed4f4c3061352273fbfd9b5e6ed8102a8b67ea686d6062fc953e15256bba5f33141162dfc");
            result.Add("de", "936ceb52d9de98ba30ad57d1e56ba0e8c9c8709148c28a413e8fc7a1e28b81f5f7b739c3eb25d04f8bdaac0df11da492650de34aa5fcd937c2ee2e61fc6c3ed9");
            result.Add("dsb", "3a07b2a3f4cf105302460d1e75e9dfefad38558ef4fcca2dcf49f76a5c4c0b48f5425f824452eacb3d51b15255fadb8dc94eec74a5cedc838c6272011e6c04da");
            result.Add("el", "938383abf84a6ef1a49d483d789ad07011773be54d77a3d6a06918e0358f3e45c33e3b645c1be7246c452071e64a0307e87ab9f59b581f1772cb445bea55d9b0");
            result.Add("en-GB", "6f25ee36ecc58be429c27951460df82460fdf43fcbd57a480eb02a8b9256802a8df9c691edfeb236bd498b258cd75f287e8565c3fda1a20d8104d0163e5cb72b");
            result.Add("en-US", "62aef1012f6ba274ac0797035358bbfd3f8884ffc80f9a048cc196e6cc205b09266079dab4e6e815ffa8bffa4384e79d0a4861cd3a75df0c0b759c814748b183");
            result.Add("en-ZA", "94fc48dd3eda5da34e3bf679ae55e03d6de02d93dbe15ee94c2263013938cf080bae08f193317ed529af56f5332658b75d537159f257e78750d7d3c062767571");
            result.Add("eo", "b4e555287f53d017245f2466838d5da51291a20874451cbf358c29286ed0eba4dd7c048ec19c4c825f3b610d110cd7e896f77d4904939b9734d0f00f64b83930");
            result.Add("es-AR", "a720dd4cf780fe9dc1f4fbf887c7d101d1b75223dd838d3713e4ebeb1603856a48803396f003b09843805c1d5042a021988faa30846b3dfdf762536c006eeb33");
            result.Add("es-CL", "81bc9e23d90e881162c9d20397412e2c762c502964a9d9cf7884c8f00fca24673c1ee7d68e939dca4c632a43b3a4615b2b6066fbc37446ce2a7eb99a521c1fce");
            result.Add("es-ES", "64bc990e863198ca3810d04975eb93f75083b65f10bbd5640c2c9fedb0c8407784df57faf2fecbcf083982488c6b6faa92244855889fb51e4511577c1021a8a3");
            result.Add("es-MX", "830b91822581e4fa10f60993c1993626e3f9e282ce6387cfabf6f8082dcc11557af62fdf246530d1c2137a3058d5c3942485800ca46f4efae327cd71448b1768");
            result.Add("et", "ead7a7ee322d729968356d654be6536fb3179f6491dae6227cc3185d52c8b283f02d531ed7e81e4def89c6a859b852a0c59206c13a6573556cc97da2b1ef9f53");
            result.Add("eu", "cb01471d1bbef2325f08d5c785ca54e2aaa2718b04972ed9895a1bd539f9a4e465d0f8a67bcb24a31dc688bc07ff3315ff7f9273bf9cd1a3a1c29e03796d8e00");
            result.Add("fa", "173999c6504da9aa4857cda68cfb2145776e46f0e8d79ccea6da9017049fad270d03adaeeb8af6e1d1249b08f18256b60d32ad9991617cfea718c542c840beab");
            result.Add("ff", "f0a6ffc19ae490c7b6694bf39f036e46b0c40707afa63992dd25dca88aef04f5ae6cfd7663d1ff5136926db40a945afa44f2b64e504ce7aeb34b8bfd492a4dcd");
            result.Add("fi", "1b8126770a2cc720d224bffa5044ea5d912e61c21d3a0405312a38b82368d5de6e2247abc578cbf265d5dc082013898f99cf453dc11553af89fdbb4c7386e771");
            result.Add("fr", "f09ab67d949c1050957a38c8db60a512da3e17361b73db3e71902fc5e6dca5fb2dbc3fe9ab0be52482a86d59f4fcb627d456ecdfe9719e343aa6c4e4d67cc403");
            result.Add("fy-NL", "960860005152e39794c736a468302e359e582aacf163147050bbf7a722a35eb4faebfeef6393557887b6966a7fa4e5497e63a76d1be22212f2e2ed29f651eca7");
            result.Add("ga-IE", "bd2c8e2700678a72884d597aef3646ba28cfd8d442e46ffb48cb3f0a52c25cddc4c7c8e52358fda759b2d99e2834e1d8cb648e42e238014d36e1a2f7e07ac232");
            result.Add("gd", "cbf8d112e3a80bd6fbfd53103344eaff1467f5425c274d1dd00be778f1acebf67ede3166a8de8669cc415474fc12478d48d4cee762520bea206e05c6650d474b");
            result.Add("gl", "13015192cd2d0c9e0ce38d7d011c8098b144a0c54d43ada108eda8c1c03e49d179e249661bd1a21c2a97a8bdbcc04627f743fb4216cf1b7da3ea247ba9a2773c");
            result.Add("gn", "2f360a8d0d2daeda4ce49cc353a57f76c6d68fc6a4bb7b305bdcca89e35a679009cd6154bab7a0bd43185de9b95e71bc2770e8fc087788bbc385dfbae1fefbee");
            result.Add("gu-IN", "a34e8bd53fea68a8870e4bf0e2785a7029500e0a9dbe46691da94940796f6578f54fa9da83031e9bc7e22114622fce3f2726a93ef55b0f0420aaa692ff253edf");
            result.Add("he", "16ee47c54a23aef191570a0a86b3621dc7cd3bbcfd974666e025a97d3bc653101410ac0e51444a7b7f61a1d262a6d568fff39f98dfcf34a1e037e24d056aa33c");
            result.Add("hi-IN", "49ce9b17b4ab0676ef086349c7c69c3075990fdc041143f01519f42d919d84c35caa87a1f2f28c83bc9762a4e6f94ab0ff0343ac91d7edaf8a3b927694faa9a6");
            result.Add("hr", "e95f54b00e6b79918ecb2eaba91e607d908832a12fc6b211f613282affbe12d853a7b16c50a84f244e1bbe9b81d64c338cf13c89a55b07fd9b683622ba008343");
            result.Add("hsb", "601fd3a13da375fa84d447d55542b762f27ebb4648c83cefdeab4dfc4e5abda71de8ebed22e96faa88decea382e5a265982a841f66b60f54975a0f02bb188020");
            result.Add("hu", "64c1630a237caa8768a2c14dc1b953a5ecfbeeba5f63de9fb39787d538a08dab9de8d52081d5562ed6d7476069130dd8e6cd4405f84bd7ab8baf683391388a3e");
            result.Add("hy-AM", "98dd28c59146f39cfc125e96a0b24e79552b0acdf5cb38bd4ba40ec8ae62fc82eb010e25b637a363269f560be6430e17715ae038d2bca8135f16c0c2a9f41d31");
            result.Add("id", "dffe05106227d41a791f9f4e40bf5696457bce092d601792b0322795fecc1ab66dbc39dcef1336745e55151745099795e2b5008eb80a0bc0159e412c0aff9c49");
            result.Add("is", "d7c84da0680ea3b2738eb1d7bc2c753a56f84d56ce9bd0574c03d768e1c7c6034164ff3d23461e94520f9c6a6288cad5d13ed06aa9a354cd32f9dc82e406bc9e");
            result.Add("it", "14ae5b3164458a0db2de1703dec6fe511527073fcbd7062827304df786ae5fc71863aa36a0c15c42de90a5a2018b65ad3acc599ff3c4c4885a21c969201c7d0e");
            result.Add("ja", "b640e854e70c0c44fdc1b23d0eb1568dbd21ace99b33ca2efa39ad74fbd51841c99e3628f07284be125b6753e68f24e1ae6195df40f2ddbaa1657488b4830234");
            result.Add("ka", "be173536b12e0df8b209328e20ff15ba8fd3a3e4681e601c2f19d440aa544dc5d9c16884c0d697f5991427066b30c4c364926ba67e8399c26894a675c1c26b3c");
            result.Add("kab", "91fbdf5e1ce9e14514b6313a9cbf8553a9345a0afe3fcc26e4477287064188d6b0e2a4374138d3b48d30ad880b9403729544b77a5313df21a631ed10690ae62d");
            result.Add("kk", "5f154a55a72d16217a9feea51a34a4fb8b023d04f5aeb6d328e8312f2f0ebca38747da5b877760f70999051918354a33967f50f029b8fa7ce40793357d2681b5");
            result.Add("km", "34acce84a59bd85a2c25b87153b429fb84911175921f7f9f714303410dfc434f658bf7243f44e5ac24721cf00777dd99a0cf54e74b6a201ed21fd75ab526b6c3");
            result.Add("kn", "b2754a04c115edb4697220015748dd4ee21f5c2b8922130154828c08eb00e645a914a4780994762610406b39459ea029ff4ead6025b38069b293c77ab4a2de21");
            result.Add("ko", "c5a4eb9860c2e0431367eae162797372211109944f1e174f0d8298e3db1ce39646f634a54739846a6cb2eaa17b8a91c38b481c9766de1b299048b88f70267aa3");
            result.Add("lij", "2b5ea3f4036bbcafafcb23b6e40fd8da169e7799a63605d810533858c2c7c7dad9ff6d8e613e7e31a8950324f1ddec7d83938aa53dacc780439c7ffbab105cc1");
            result.Add("lt", "0bbde94a40d6903253084836f5fc9f3b164dbcedf90d8ccd3428da26778c65d7efd19edfffcf9c9adde051e52a45da1deb473bf3ff0a7555e0e838e2f9896bb8");
            result.Add("lv", "9d96e35aee2863a98eab512e217e977178fb027e9109d84bfb5373756acc5c053b5b00fee1b6a726161c8a8ede2fb7f8d8f1595bca11a291ade4f9582a7c8735");
            result.Add("mai", "36b78bce9b8bbda56636dc944b22d7fcfddd0fcc4c900923b17bb8683a2a159104a2e350fa1941169589e168d1f49e6db668ce693bc0d507aa6e9a249d52f218");
            result.Add("mk", "9a2c132f91178b418751fd1914cbfe419fdd131987678dc6c922449a8bd6b29db276910fb13a40d553cca6ae907655f4af5976ae75a2fdcc140b62bb8c4e3f6a");
            result.Add("ml", "aa51be2ca45eac0f2c2b8d42bc550c26bfee28b223bf849e9131e86c50d6e2bc18c6d93764f7db0c567ba68bd692705dc85e2a273b4be2c8d98b9545f13fce5f");
            result.Add("mr", "144eb4498690b6da5c44ca18e06180a9f5e8799b69c747858e988875f71ad67361373ce62ceb8e913323ea4af1defa7698a0b38ac0c6651ed1980e114a1cba73");
            result.Add("ms", "36315522e563a3f46acc826f8e038892abc7116110ab1c26322c016993cacd7a75cc65eccf8817ef033818a08863cc8352025c7d33520832250bb058f12e8ed1");
            result.Add("nb-NO", "ea97f96b2a570784c02b9f3535fea6e2b574e2334393a9e15d231b5577d31884e129db58f2e8ab77976086d37d0c8488b2afec211ca6c357be4455dedb087707");
            result.Add("nl", "fb84781d1982ab70a21a5bd4caf9b4df55f792721e6eac9f97c207167de83f7ce1e24cb33214d5ed7fe8bd9e93b2f4f92bb407c20f9a177eb4992dcd655e4580");
            result.Add("nn-NO", "f3f32aed87972b0411771cf33127c5bc4e52c232a1902defd44f7d3755929dc184afbd9b6d1993acf4bce674b7f818b1b091b7e1c03ef073296c99b904b1173e");
            result.Add("or", "ed7de4a2c0728d42e5060773a4c47f116e887e087cf5acb3eb8b0f900444b8f7235576e1176919d27fa0c652c4845a5948f529ace49f4703af134bbb5ddcb5de");
            result.Add("pa-IN", "f827de9057d0bc60658a49f2ee5a5a71ce490a221074180fc414f26377711ed15fcde37367db56aefde9ebc05169c7d704e788f7c068926435f67b169621cf98");
            result.Add("pl", "65e246a42df270b963260aacf5a5deccf027f64ee3534384ab99ab1dbb3de76d1f7246e511764a314d36901e7a7bfa71cc27d7dfea04c0810d0ca6f2921a3ca1");
            result.Add("pt-BR", "d34fec89c0b6caba27bf5297e04fad671f45913fe001e2a80d364493b02b6d1ea89e17f941fc0c9735ad621c6577e07228522eb8066712abd6a5984ea62d6efc");
            result.Add("pt-PT", "f520836c3fe4335bc1a1490da77ab710320ced159019231356c168125999031792e002e0db5f1a6acb7f7b6cd1dd3c46fd927e2ce908700ec18f244b9c50f056");
            result.Add("rm", "6608471adbea8c74a8153f3ebd487ebaa4e6c8e16f8808204982786cb042159586b9458440fcabb662ac27e213d3375aa0c4e42d9b5ed0b52018764a0f173a6c");
            result.Add("ro", "6d38658a7f7d1caebab8e9ff218d23a6a91606c5b4998dc3ded0a95805a29507da715c86dc2cf8881db1a6f6ff3285720845533d23a21d1f67d131ab5e780ab5");
            result.Add("ru", "795628c6e0f4b0d900cad31208bf5a7070ec5765f24828b7fc5ab06c0e84941ee902f2cbf88a6b09316dbfa7b573452a24243c9af574b65a1ff02d68367b3df2");
            result.Add("si", "c8e0348b0ccc7f781dea3c79f79e600cf1da4269567fd44176be63f9d56bdc3607e4f0555b71119f51581a5ad8d425708a0f7509a4d3d37f710a1c7759fd1979");
            result.Add("sk", "68b410a2fcb8bf3e62c34a8e6de8acabb09e3714e6aeb8f8c7bd22abbbcc42dfc48a9a9302dc7b47a71d54bd057a362d79c23a71573ab2e09835a15d3d23d120");
            result.Add("sl", "c11fcaa339328e4b923fb18a66e32cba6dddae6280663cd11de9cc837380ec4b81139ab7fd03f9f4492c01f76ac10ae2b077ef6aa60187a25ae8b7dc78ec8f96");
            result.Add("son", "2c6e937efc80899bf89953e30c0ef7b3ca5d4f05fb90c533eb2a87145f728ff77d312585d55b9898ec6631ada027d3ce2b954985ade972ed83390ea5f17d8751");
            result.Add("sq", "85de817ce8f0a12d435d80c77b863e7dd94f2896aef4afb90868bbf07833fa6c4a2b851bbccacff5d186fb152bdbdf274180c23afdf912925e1e12a0a08c179d");
            result.Add("sr", "6b63df7d26c940fda4afbd69956ac86688c361d3c1f0e87ff5afa99e426bc4c5d3401a2aefd460fdf5f9308d9731a9719a8eca725188ff41324d06a3f127c1f7");
            result.Add("sv-SE", "eff69bc9f67398ddea6573232785edfdc0abfcf4ef8766bfaee04cbe6a90c3206b2401071d3bf4bd5856cd95669403ff2fccfa4454461a52a94b11393fffe583");
            result.Add("ta", "8956ae873ba9a70969a5a37a6b81efb9ba7974a85fad527589d997e7bb61e839316294ce6d32d1adba170de293f5eef815df1901ff672f7f4dfd82c5b5b27737");
            result.Add("te", "0902bddb7ef90e94f2ad3a90607d2d57c911f001c4df99ad5cbc999246358898137e5781ed2ceafe9ab6b56169ab33273e2624a987c3370edc6abbce90b7f3db");
            result.Add("th", "ad75c4b30736e3714cef3d95c8979ab367b9d41d27f6485052d1c9b34bf80b6a505c2928452d2a53901245d84a7e25004b4a28c3dfbed48087857da4a59668fb");
            result.Add("tr", "57e2a1d25f84f1807ddcce6e1c754906b03810822e29f32349ab89861a64f0202d7b6b2ef931a7e349a0322bf3e559e9091d6d867525af1c2138cfb5a55bd7a5");
            result.Add("uk", "146d65ef5c2d07a1741560baed9e0ceb04675c3b1dd3b6c2bd3bfa83c2d9b7f5698abfba02c80d8e7b84de1de17f5cf8b61260786160763fc84fca0372257b0a");
            result.Add("uz", "24c861dc99883ca914d0a0a42ae4f9cbbfc404f17d7b9309e79b034c6f8e460e947b9239e758d42c47d50a0828b7ddcdd94c390cc6c1e23586a80cab5f04370b");
            result.Add("vi", "09baac061f2b49ab409eda59502d95b30d3566a3fb9462db85f26ceab342976903fcfedf31c94e86f4659c6c82cf0cb38fc638b6480fef1c6485640177a1f712");
            result.Add("xh", "46f13f5c582b8d14b2f5b4bdacfdc123700247c0774ad3a338dccf9beedd28658502c6ec81717f3261322864038c4c46ebc04b89b91e2613ff541b8e459c2579");
            result.Add("zh-CN", "0a2a96139f98ae911640d1d7e6d1a0d8da0ece394dccbdedc159ade9fd661c662d1991d822cb83dac30bfe9295ae91e51935791fa4cb49cca6b5d971e440274a");
            result.Add("zh-TW", "7d2b6008345e940f8e949b41f851d7354b60d9c9f6c87fed6dd2dfe1f3c5d309f0f36fc6b1b374ee81b68f8e177ad5f1e20ed78ba0535797788bcdd3e863fb61");

            return result;
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
            const string knownVersion = "52.8.0";
            return new AvailableSoftware("Mozilla Firefox ESR (" + languageCode + ")",
                knownVersion,
                "^Mozilla Firefox [0-9]{2}\\.[0-9](\\.[0-9])? ESR \\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Mozilla Firefox [0-9]{2}\\.[0-9](\\.[0-9])? ESR \\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/firefox/releases/" + knownVersion + "esr/win32/" + languageCode + "/Firefox%20Setup%20" + knownVersion + "esr.exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    null,
                    "-ms -ma"),
                // 64 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/firefox/releases/" + knownVersion + "esr/win64/" + languageCode + "/Firefox%20Setup%20" + knownVersion + "esr.exe",
                    HashAlgorithm.SHA512,
                    checksum64Bit,
                    null,
                    "-ms -ma")
                    );
        }


        /// <summary>
        /// Gets a list of IDs to identify the software.
        /// </summary>
        /// <returns>Returns a non-empty array of IDs, where at least one entry is unique to the software.</returns>
        public override string[] id()
        {
            return new string[] { "firefox-esr", "firefox-esr-" + languageCode.ToLower() };
        }


        /// <summary>
        /// Tries to find the newest version number of Firefox ESR.
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
        /// Tries to get the checksums of the newer version.
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
            } // using
            // look for line with the correct language code and version for 32 bit
            Regex reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "esr\\.exe");
            Match matchChecksum32Bit = reChecksum32Bit.Match(sha512SumsContent);
            if (!matchChecksum32Bit.Success)
                return null;
            // look for line with the correct language code and version for 64 bit
            Regex reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "esr\\.exe");
            Match matchChecksum64Bit = reChecksum64Bit.Match(sha512SumsContent);
            if (!matchChecksum64Bit.Success)
                return null;
            // Checksum is the first 128 characters of the match.
            return new string[] { matchChecksum32Bit.Value.Substring(0, 128), matchChecksum64Bit.Value.Substring(0, 128) };
        }


        /// <summary>
        /// Lists names of processes that might block an update, e.g. because
        /// the application cannot be update while it is running.
        /// </summary>
        /// <param name="detected">currently installed / detected software version</param>
        /// <returns>Returns a list of process names that block the upgrade.</returns>
        public override List<string> blockerProcesses(DetectedSoftware detected)
        {
            // Firefox ESR can be updated, even while it is running, so there
            // is no need to list firefox.exe here.
            return new List<string>();
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
            logger.Debug("Searching for newer version of Firefox ESR (" + languageCode + ")...");
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
    } // class
} // namespace
