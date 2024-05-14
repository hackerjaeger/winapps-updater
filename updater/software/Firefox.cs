﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2020, 2021, 2022, 2023, 2024  Dirk Stolle

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

namespace updater.software
{
    /// <summary>
    /// Firefox, release channel
    /// </summary>
    public class Firefox : NoPreUpdateProcessSoftware
    {
        /// <summary>
        /// NLog.Logger for Firefox class
        /// </summary>
        private static readonly NLog.Logger logger = NLog.LogManager.GetLogger(typeof(Firefox).FullName);


        /// <summary>
        /// publisher name for signed executables of Firefox ESR
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2024, 6, 19, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// constructor with language code
        /// </summary>
        /// <param name="langCode">the language code for the Firefox software,
        /// e.g. "de" for German,  "en-GB" for British English, "fr" for French, etc.</param>
        /// <param name="autoGetNewer">whether to automatically get
        /// newer information about the software when calling the info() method</param>
        public Firefox(string langCode, bool autoGetNewer)
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
            if (!d32.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
            if (!d64.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
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
            // https://ftp.mozilla.org/pub/firefox/releases/126.0/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "26c97249d5fa296b69a747051305c48828444cbd5f202b757927af3c4c020e6d9eef6fb2204678327dab2d383bebfe57d91d31b14ab7181cd0514a9a87f981d4" },
                { "af", "8a7fda8d747a8402af8fc36608852039a84875aa209ab00e8da0ff308fbe65a28a97864dfb877daa78386c032331681eab1f649027fe67794ad35aeb34f5ddfc" },
                { "an", "55ee14e2e71573860d189314678083fe2c8de79b3c10028848a29b753325548715caaef35d30cf5be6760de602b2491e2b2386b9a5f95a4b78a052fb82540584" },
                { "ar", "2c656b9961eaf1048188a640cce55f2c8dff1ead434dbe45a275d5b38fd5f9bcaf16dfcb56370b3b08830b8f01f624de3c6fd5b8989176c4dcc77b1451063ebf" },
                { "ast", "719ed5a7a876a0a40dea86dca633140191849f3207b818894d6fa17d4e9b2adbc482193e1b014085bd3ed64a81485631fb380d0437e6810667c6c57db3ed5f47" },
                { "az", "3c92e3d34a92d125a544e831e2ba6c2f4beb1c9dd806c120dd5ce279b0c2aa2012c9d43747a9be5cfd3adf01bf89446924001458d778a65f75189efe3ebafa71" },
                { "be", "3261d36c40d427b31e311d46afe647b5e4218147c8de74a7feb7d48ad96800237aadddecd80d980e698a810b83412ef9728c11eadd41bed549cbbca212872202" },
                { "bg", "1a28875193a08f9f20d87f307ce5865db4f5479d3b4a21e0b92fe6456a2edfddd1e5b1ac92076062ded2f90dccee565570aa58ba8de94e46638ebc10afa63f12" },
                { "bn", "985d93da10f2c12e2bc68f3fd03e62732c80ef4d75216676dd3b8e00135cc18cac60d501e73153572904393eb65e47bb8308f70cb58b42742022c702a1ebf0c4" },
                { "br", "66887bdd7fbddb8d2fffea678674da2af4570ce48b803d622e334428522dcf72a33d8afdce21f39e409fd6903f66389c45be1d25c7b0e5936f634a9a342afc0f" },
                { "bs", "e0ed5acc2e9814465c8c75cd6e266e4f15393253c7d4e82d2e9e980dec52d7eda5145c1adc00df2cc746663ec41fcf538e64074563eee8decdb4840e25add405" },
                { "ca", "f183d06a93972e3c62adec0931e588d4b0fdd544211ca363f1cd49d1793193bd668e4636fed9745627c5232641e97479edc252d0d9633b4428b752171fe47922" },
                { "cak", "694ca9d2bc078f2bf2a688932e3ac5e81bbfe92fdf848b9d2aa5dc1f13da42f83024972268679d1844c02ba628323026ac50c8351b7edd91c0dcac9ddcd6ff8e" },
                { "cs", "a227428a9c44f4a7370d450722529ace8e65fa84cf18c4d69276eb68ac0453ea78719d47ac04bdabed7c49e8ab73aea4276a25e50011e2803f4138c98cdde6d0" },
                { "cy", "c75cf32d1268092fc35c5931b9230c522a0144d7b0ea1f0807337544ed553644125f596115393b3523767ee75d6d1c5a5abc34f64bc145f23c577de81cf63d6a" },
                { "da", "d79d168ebc55318e37b256108b179ce6b1853fde92bbd16a5560da06255f9c493bdcdc48ca532a85036aa9972c8addb5d010a06a5f760b0a0134b899a408c67a" },
                { "de", "d3e358837afaa3d7c3575561d6141843a7dc65463fc8068a8c87cb2efee81d146a2cdbce2c6bb56a895d924b3e158a7068e635fbbf0561cdd72123b1bd53375e" },
                { "dsb", "a7fb1c0d52de656327355c395df5571c68d630b7fc074c8f4b3a4ad778c464c912c41262fdb6a97b12fcc2c54125be519a020cbede6c3e80bdcb6d5679126b65" },
                { "el", "35dd8ee8c396e27841a167d90666f38f540d1c7908103d2e54565b7ab82be60dd9ee186b2e53e49b2c8f031e5c6cbde16c06c87034629ef8d175e539c72caf8a" },
                { "en-CA", "674a64dfcf6db5f448cbd7ea4970319c3c622316fc37a80e72ea5be691020b8409341b10eb122681b7b815325da1140ae9432290f1c298549e2f1aa862211b51" },
                { "en-GB", "a06e415b93e92d0883c60d52c77a2b32829127ece415284546547a79eb64a12ee33155a93669d7bac4da2030aaebdedfb41ac6f771a07a9b5d8bb13969854938" },
                { "en-US", "486089b1bdfb58e23c585dff2dd18b5d3fa6a1861adc9664e844320a7cefdaa7e31480b71afd96d27120180fd528c18134c649eb0b42faaeb8f2bd443c017b88" },
                { "eo", "3846bdb2238cc0be7884aeb60f4efbc2ac22a55e5c7d9a7d53a496d61cc85bb0f9c18341adef8ee2965380eeee65c06cf3448ddb1cef70d8eea899c46737c2d0" },
                { "es-AR", "4e7b870081535bd3267e1e92b9b07229331985af2654d3b3dc373515b99c66003993241fa4807eb1dcc2d126ef906bc0699e4275cd7bbf564268281c88c315a7" },
                { "es-CL", "a8ec7e0b29f8724faf5ebf437d9222663b6ad06f6bb3207610b38290a0068ade39c52f5e08183c05b76cba44dc025ed419bc7fbec7d06e71ee96ec8fe95880d0" },
                { "es-ES", "768e86f8fa6a12056aa6a4e001acd420f20b1b94c9ee6b62a50a9c285ada788cfe5fd824be0d523f331a3ebfeed4b24046c27cee40e69391be1f71b5d5f7b058" },
                { "es-MX", "fed1e2479ada3f21dde973a82f51fc646a8a226c7c4220c45b1f1289a6cc7323189efcb8b6c1f3383e610dee9ff554dd8f01e2ca33a0641475af9fbbbc276378" },
                { "et", "5352bd5dc8cfc6d4a4ee46bc3e42290399ccd9fd9340b4d0033976d0d89023c3be9fdccab5bcb3b4947172bf7493546a35c0d8a88c1e118bec4d5111bd155b12" },
                { "eu", "b6f83ca8bd3a397177ec76c744c0240564acaafae686c53dd215848086f5194c9f3d4989f157474da0ec47e3dd686efb3a9cf3d609529e8b293c2eb82a2c974e" },
                { "fa", "11ca320db807686797849ef04768626bc921706786599c4a04e1dd1616b1259242e5e9333c9eab9b0baa1604f53938f62bdcadf9e4d549a6f93c342f3cfdb230" },
                { "ff", "694211babf34f94f6df57865a0ba631c3e43520492584d20befbf97af87c920f981613a40c09977bdbd8016d348e1ed803dd31bf84462dc249f38efc2f6d669c" },
                { "fi", "921426404bd7daacf307859c1836e9fe717996047e8dc5a9a8236b6be63daa0f7df2a8c66863df572dbeddfa20198724e48a8aceeb3fb42363ffbcaa06cb52c4" },
                { "fr", "9689996a18c82c2e73c40c354aa99012a5aba1781733ac4179bf0d43fe61fdb0c3ed679daa36d889ce346397a07752cbe1d168efd1052d1ddcf7ed230442390e" },
                { "fur", "245a9e5edf1892865882e6cc785db3cb56035b4fa05324bf7c3d5d868077246470e4fd1a06643522c3f3297d9c3b4fc7684d549d9b1ad48458ae5c8fe516b7ff" },
                { "fy-NL", "a8641eac13993ce822e7a104688531e22486738dad82b0c5324545082b53461bf50681fcf6c2e06a2841533c66ac7a522df24a954f276c97c9dd037bc603c833" },
                { "ga-IE", "dccee8a75ebe169d6708730eb7e1509f60791369373f623bcf6cf6c83a0fb9f2f4e6f47c12a016e43c4e922c67c5369f11e7d9d15c39b7b1abfbd382e5643f59" },
                { "gd", "f962b2aec4d2c66c99a9cd146e8772658dc8da862ba14f2a7d962e43cd19f61c314d49d848198f87064ba817c7c06b81b7a45e1ab6f24dc0abd65481c2458299" },
                { "gl", "3bb0829bdc3c38b661cdd00f44724dd8a77ba5ebb4639dcb498a934957f462647c656d31e7403b70d1fd436e87678c3d18e683d6b74fbfb0f9ce51153a022ea4" },
                { "gn", "8ceb856ffda45a33749841fbcdd0b6a4e84492bb235020fe59d3c5d9c8b7bd861188be1d81b150d01d8c4f892b9763f72598f998f5728ab39106b981ec241a00" },
                { "gu-IN", "6eec2d56edb92b450ad4cc97f9237628eb36a0d87c47d6dac38e929dddaf96e7e3631b9a9414d13e98f6123f10a1f61411d8db751ecb5baabf1a5fb3a07b26a1" },
                { "he", "90ecd2f498ea86dc6506e609f3bdfe97ce70ff8ca3d970a128415630a6180e33b02a3bd7e70307f7909fa381a5dfa9f5705785076615df8d3f7d6972bf882aef" },
                { "hi-IN", "dc1f2d0722e552daa00052c991d0176ead9f82398910a0c6fa4a7e8f9a0f292b94aafee8bb057cd721d93a9fcc7518ad97fc1934800bc8f57516ffc0d5322555" },
                { "hr", "d1c28840e03a86436597eb75c85610dab2639964fc5a30ac9ab16fb467983d817daabcc9e85d8ae7be8c8f17871793c1ae883edb926db649700ba8db401a7a78" },
                { "hsb", "4e4a30c561a0ec571ea37b2a13558f5777a4f5afafe6d4b13c049b28b6268bd7e4bd955a3a8d9134a401a090c4023b288cc78135a8d4d033641057dd0281e344" },
                { "hu", "57ab4d36002a9f0bc69a07363e1378ba15b1c251b314829589e200572b6602f415ebc9847410d2259c5dbfd1342a412a5f956ec2b50eaaba7570c320a386424e" },
                { "hy-AM", "c9eb381bac8acfb16e87cad4fd2da4238db17bc3f9bf566a260e340bca2fea14e6b53e32b5b6d9d8a08b468077fad322caeb58a2dfac1d4ac1c8f854d04209ca" },
                { "ia", "12d0091246a95627c0ef9d135821a0f5c50f9494099ebd8b4b475fe143969c51a489e226d8f7ba9a70319d40c3a4e95db1bc981fab126490dd7a4f69c811d698" },
                { "id", "9ea9a9076a689ef9f06dd33d508f8b11f394acee289b03b5ab4100269aab46547f98a274f7cef598b69b4106f6c93dbf492e3fed9f838048cb56413b7f890d81" },
                { "is", "9fa3dfa5a2a1198339e159a7c62770f1efe0529000f1a7c954122770d1443bd92735c08734ec691784aa0a24e5e612dd187b2441c8e2d66a09df617e117390b2" },
                { "it", "74d63afca98999a60bbaac6da869e10f28f2aabfba61441fc7540397b0a72cadfe2442873dc50f143971e00521e1bf23d06d76ed1c212ee12ace01fb31a7f643" },
                { "ja", "ecad45f846835a6838a17ffa9261fbcad68a729ba7912dc0154f443d545fab037ee9c819373872c73f68a917e2fc42dcf66bc0ff4695b5c2a9fd08d371ff56cd" },
                { "ka", "473c893d1203be2c38b9bc8b30888e47d03c50787f66fd66c89ebeedfde04e86ae5312ab7b78acd0823f1ca9097d221d1cdcd02eed292f58328b5d6bd1c5f1e2" },
                { "kab", "e96ba0d3dd144180ce6a1271ab317b44371a3769f6f39202c04dda96094dd16d356c62cf505dcdcda2fbfb9b13a99725e024d39f9150dd97aaea6846ca79a36e" },
                { "kk", "47520f26398ecf4403a8271fc65b4bd1654e808f974f796161e5676b3262bbd583569040d14ea117419a30f7e0b0c6afcb1921a8f8b673c8f252df7bf61cb1ca" },
                { "km", "2c800f597aca2d2e2dade7d7b39b44fe994f951ab187b4986114310272267fd038c6315848562a7a662c7099ae0507d839b0cf06f1430b40be852f2381dc46ec" },
                { "kn", "c0d20b1fca5bca32c3748cd4fcd2698b661e0485eea63d6b844ed932874a2d77581239e65d636e3b4b47ee623c0c1059b18594716eb03287ddb0979704e83c71" },
                { "ko", "f62b862a6cb2dc4a51c2423aab97cad541747c7624559c228ff1dd1019ccfd05ae035acc766914b6c968d10b46dad5a43c6b76e305a81d428cd4615fc385f2b5" },
                { "lij", "b41862a8e8c8ab875cb2d83039624b4d500e0de1fc2530e61a2405c0f24db3ff803cc0ca7be4394f3dc6c0b2de391fa9255f0357c20c9af7ee1d7232596db57f" },
                { "lt", "9a672c22885963619f1ed2d989631afd08fe973fb4e2876d63d2d499f09d1463eeaf7645a3b79b1766630865e7d23094222a348d42b26693e4ab9a8a3f1af410" },
                { "lv", "4183791bb72a2ea50271d19f2558adb9e7ac457521950f6391eb0572cf419db03361280f62a4b0d99b80d7c08abe48144c88f150a758e68fb75819c6ef699c00" },
                { "mk", "dd65d17dc941d5f034d0a1290e69c985e10e64746a78e055be1dd7beedcfd3e414ceeef11b59bfc458ab2d318f10d330fc021b2ebf6b72e230e558a186c838c8" },
                { "mr", "4aaae96b39d1cb87ffbc790851860e1d4889be6a5b28ca033032c1d81ebe401c9706cdb7ed134e7a765e83a24e0bbfe3b96d20f92a5e07303ddb50d93afc925e" },
                { "ms", "9b2b72677a8fa0438e53d4dca23500dcbb98278f2ea6a51742c61a4634d11e8d1b4ab9e7a4e6d84944dc313a4418523fe7ff33c3b4c71f1f0dbffa1af23f17f3" },
                { "my", "b88504b7a745bb5fb729e7f95ecbadb8d1f616f9b106fc894d6a674937b5aa1a16fbda41fff238147c08b7b2f66b6b44146da8af0d9bd4d28b0fb5c1ef764a0b" },
                { "nb-NO", "78511f90949fe10ddee47ee723e98aff2af3564da5e4991923814b695be65eabba72ce232f098659cdaa19752f205609a52cde612c90498a29117c535094c4ff" },
                { "ne-NP", "5681dc7e6b4ee432d608872b10f4c5d9fccc7d96d84aa91a97e50687689deddfaa8a12d8a776f751aa1d2e398d1fa3e700d5fa10fd1ffbe385bcee94f027928e" },
                { "nl", "4cee1aefdb925a507590bd9be4cd2ba3cfb44263af2b1ca0faadd30f79768c4410c6e699406fb4bf9f22ef252523a0f9196fa3340d52f31b897719ab3882fb41" },
                { "nn-NO", "7631e8a0730e30989e47b2f029cd804e6008376ac1f0631da49077a32b9425582ff2f8323940e756530147a2d9f12e021d2577a5c5266635585b72ad95eb351f" },
                { "oc", "474a98ee7b5f56590ceacf780c67b1ea0f8120fc95d9b2ec78509c5e27087e67b28b06a43bcc679aaeedc92cfaadcb81c4c27aeec218350779030e366f48068a" },
                { "pa-IN", "ffa0ec4afae03219eb7279f8a1be1d992a42d372da6a14c879dfdb85c9566c16d47c71eb2b7b3eff2a71c40355b7502074daec0253f8d53718b1be6c5230b4bd" },
                { "pl", "fe76ac2205bb7a0eaf978f4670142b91f91f78d12a730334e8aab791b9273716f0611bb72499fc31033c78fc57708f80d4edc892edb2f73bf18651587221b30d" },
                { "pt-BR", "2853eb100aab7eb2ab32e8b2182cb7c4b0170808a16c4a17ed16912ba26470c9d830e1acdffdd20d7b8e6f24e637ea518c01ceac33c8666a955b1f6a53aee581" },
                { "pt-PT", "92307b2fe7e78cfbebd99b63555bb588fe6ef82a0713e3f212544187ed12da31e4d74ab52427bb5b0956f3f94ddba400d9a435185eb625b284f3a95a53754bd8" },
                { "rm", "f6505a83d735afcf93952c2d5ba4352baa12c02489f6409e516b4e4017f9795c18adf47c120aba6e34959f110dbde082bcc919701c468d46e7e08f0304fa1edc" },
                { "ro", "18ae2f62e2943e0dfa59f1c731c5bc526350da9085aa9a1e934fa872df7dccbc5e761ae85317609857a2600fa02f42bc853437370ebc4522a79c69806c15da17" },
                { "ru", "35c20409962e29f33cfc1d74efd6a8a78de54bbddab49192cba84a9fac9b44f1104f9e96fd4d635d0785c579d71582e2cbdaed50b7bb977455a7a944045d5e06" },
                { "sat", "44eba660af8ac4613eaec402876aaa8fc032fd155b15d335cb9e7f158d8bf3428bd9f095fcc84a1180047b20b4acf54d4f89e6016665b8b3bab2135d54e48e8a" },
                { "sc", "45b73893bd23e0f25594ec9078aec16dc43efba5705a3943d4b918348f7bd1bb2c531c4aebb1346e693d249555915f653f23576c9cc7c4cf921562909b07bdac" },
                { "sco", "e1f98f0a5df0d3e89c6a773addb66547a6b4941890e72d7d017bbb568ac8c55325546f5b05ab3096c2341e9e03c82f739dc6dc940244f02277eec5dbaf4f2f2b" },
                { "si", "47db55e36e53f5b5c19e8f30991ddcc8391fde92bddb2617546c5040c6d31be0d5e1199510a1af7cc07c7d5187ce128a498c826b87a2ea61265d1d958b0db2f0" },
                { "sk", "461ebabaceff2a901ffec585714821b451d6a1b654e0c5d1a7ff9e49e58ffb893fe34035be1484540c6430dc50b6860eb9b54d041c7af1f7ad0e3c3dccfcef09" },
                { "sl", "f44ec191386db5b9d16e167bf53781fab4b89b87b9eb2893a3e1364208ff3f372085576f854b2c1df34dab8562df076c5ee4539febd5a7b6236e6bfbd6552593" },
                { "son", "ee17f8e298d1417ff01f9fc09105e2c126ecc91bed3419bf3bae29409ce7e0ab685e9e728e6c6c30214a6d911a708cb4aff330b513591b418c79a4159048b0b6" },
                { "sq", "d6f75cd395195869168c5d0a29390f0317272af73fbea82eff3d7aa802b1f0e905c0d07c9a5d35764e5c73669fa483a5d8f2a44366143d9f1ca1b01da59335ea" },
                { "sr", "1a0a824d1281582ff39f81707ef1efe61f92f3a7fd3528e5204246b384d71f99b02464ea1378f4debb9c06df98befcfc013a8ea4b7710cf8c7bf0bbc7671e14a" },
                { "sv-SE", "e70449c0c73f7bc4524d1a0d82a5f22d2ebc50d579fc61ead696e3b82e5175180d3fa592de663935c1648d4983853db76aafc01876c06a94d625f3ce7b397e5c" },
                { "szl", "21397f04a2b71b9db328d9c13c50fd7286f65cb818ab3e98c14c9a930f07e9c16d41b22d3e6b08da40c5bd0fc43643c1c65cd4dcfdcda0dbba1521acfdc42b61" },
                { "ta", "a13768849770155afe160f2f1db922f738a087fdbcc11f5db51552ef545a21cf632c3f759e397931e92e6d43bc58c2f551ef32be35a4ef4e5a85f31f96f14663" },
                { "te", "7851b1882c147eacff205a94d94576dcfcb1229a5fab0fa671f50f06de7ae3c45d5c02db39856b363157822489c3ea03ba9472843d06aeb6fd81f548d9edd508" },
                { "tg", "ce69628a317886c44c4c05362f0f447eab550d0b72d5ad20df1dc7ba32528fb119cdf13257ebe780bc60c8aec91ca4605c92aaf98409c8d77f90ad483ccc939a" },
                { "th", "74fece9ac660ddef67dbbf844f63a34e8d421bb9ef6f374123011fd64149a68b35a4135e360d651e57cc320958e97038b86ddf0e9782285a43a2f069d86bbcac" },
                { "tl", "05cf9f6a6d8211d57346f0acc3d5a1198581ac7a4e9478297d7cbecf5ac842f4931697be6650848d70e9f285d21cc3c80245afa0f2fb84c4c606e310ceab5003" },
                { "tr", "0eee468d2beafedb131459c070595bac89fce58aedda9631fc201fa2f0bddcb4e11f2d27bfa2e881909644f4e78bb4f338e2c3c619c55b42210fb78f38f49922" },
                { "trs", "82bb55b55997e60bd31e4f0f533ddb1b8eb55fa9abd3b0df246166d838cfd8357723b68dfb461f6ef4107800119cb9f8719f48c0db1abceec6707b0c7d6b3b6c" },
                { "uk", "02af0b2b98d1f8b3b696aa46b947bf747b5d647a16fc2ce1b4ca9fa34e1319e430296077c8eaf68b6d23466d3b8ec0b0b5ce36af177e95507d9246d850650680" },
                { "ur", "5678ad10fee49387e23e716bd5da9142c99e996aa7fdfe07ffe7b17c9772a9ecf230575628e9a966c5e29df22927e8dd8b5845cfa6a83e46c6349c0a5b3f96c6" },
                { "uz", "8837a15b147ed7a85c8a1bf74241a9e77bfa0fcd5bae03de775077f069b4c52893a9cc63700567e75e10db348dd21264e6d86d5654f554d9d8eb239ebac83aec" },
                { "vi", "0fa5e418c5c15ed7646054ebae92b7ac11a081d7fd05313767f443508df5cb28d5a88550f75a504f9966a9799588ca69809c594a32b62f8792af51abeab4efe8" },
                { "xh", "c45adf271041d9a9080fe8e3cca6e15b3e488d3bb62e21139617f615f38b968981d1f0aa0a69a261b7b20884503ba14eb83fa5b4ab4ba5dee799099e0918e8ac" },
                { "zh-CN", "9bfc8a6be9c17916c70dedf9b8072b011599e90388c9763abe71c028010a59238420f58011463efeb5ad444f9a65179392f3661f755d8acd243b4f580f9b97ea" },
                { "zh-TW", "fcfada8cc6f217828fbe51b4aee65f507368ef58fd1588a9bb5121c296afe58ca223dae074a75421cd15d0629a52110ca77fa1c3fdfb385ee6791af8bd94da95" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/126.0/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "dc6523b46579d2cc22a26daa48f052304aedafff508fe632f83156c1549a8d3b19234cc18437ee5e7775c0933a3959fe81552d8595f27fa21f6970461cc4f209" },
                { "af", "c99eeec3531c0e8347d49e1d39701d11786e81629bbd6b5b1dbe1f5d82884ea9c66318d082f251c3ef41ad188fd8193c94133d66cdd0b572dbd24e90e1014e66" },
                { "an", "8077aaa9411d7a5212c383f9fe45995f0007871fa6b130f900ae1270b21bb84214586d7b9987e906401963d06ee44fefcaf3680a5740cfbf7ad3846e3c88cc59" },
                { "ar", "e0274b6d5212a86fbc1cdc16b81aa32dab68aa99ad557098f7bed6461801c37234ba3161dded9936cdfedee4dea9298f3b3df38a27882727b05b0f2a863a1883" },
                { "ast", "2c56d2afba93c8cc85588ba2a08528fde6c345955b11d950f622ec486afbecb45281d3848eb25730abf089c026794fc8e808ca74a7caf08010a5a08137033e39" },
                { "az", "76441bfee84e3b01e056d59ec1b5309f2c6595d5b7dcee1f5d4c14207d1439795b3f6ecf4e27a8bf642b5a7b5b895c3bc5c9dbc12da929fe2be2cafb6139977a" },
                { "be", "951cf625128d66a5d1e17bf1f62d5fc8dbb850f292fbd27e4f40680ac65a2daa7f7d4606eebd1cd206cb48ce0e935ac0b786efda663a20e67d321d52893152b9" },
                { "bg", "55e000392985898e2054a2d2f77f720d916aebf71e01c93b9e045a06327520e664c0322d69bf9eaef253e962b6a61cd2f346a4d7ade35cb2b2d6388226092f0b" },
                { "bn", "adf5485e25364b1af140c7f77de84f432437c28d5b40f9e471f8bdb87787b75b99d4f31688b913c375b0e042233487416f182e6a7d9505a9f43955c32b1c102e" },
                { "br", "b1f3d51b2ed0a5fee86515a4f3a928d8e0fa622d8aabf94aa6b3ba0a907af2bbb95c8691d94e1326f3cf358184b69798c6eb28dcc7841d2ffa9d11109c383c67" },
                { "bs", "047dbc98a834fb874da66b05e5f43eb97236f9575fbee4a27b1ed9bbac0c55591a079741dfbb3b00e61b48927ef2224261a210868fe841745c0827a528bbf038" },
                { "ca", "4be80b430236585fe6cf634868c191becb6816652aef7aea9fcbb07b2f5bfa3a412bec2cc7f9a5309e271d499801df4e0fbec5871ba7b1880f9bf65c1a41eee2" },
                { "cak", "174e1671056c6039cbcb43b261161e553ba1a343f85b9c178d8ccbdb592742b1d30dec12be4cef1032002c4dbd164fbf68642ba14b1339daea171a03f7efe03d" },
                { "cs", "83e082888ac2558c22070ef643db6a0f387144b68d1b96916e2a49e2b156785e470654cb89292b1983f964e8f93526269525da930ef74f5ab4c127363082e80e" },
                { "cy", "64f113667755df3257b31e0674774295031d5c9043c735a8609f8a12730c8d04221fd6f28b414ee77830f4fa33878068edb41c2b2780771ba382c594edefdc1c" },
                { "da", "69d8de51fe9e346792b0c3eb86259c38b762562a79c7c8eae74608c776ad3a1c28aaf7475d3756bf6e10fa1604374e422774834c4af3314797e814dc0a3bd8af" },
                { "de", "4ffcdd7d652dcc8a6eef9d2714232a9e91c4bd1f19d4880c13d3c7ecf7e065a35d413347d62836095994b69d511e14dbffbd43cc15090b015599588fc5a00c9c" },
                { "dsb", "5e231051f4e03503fa42b0165cfaf09e0e5b2b54973356c0a81ca2050e0963a8cc65156fa3f06c15a793ff8d6b7f527ad1c6b872ae6eaf2383bed4d1dd708744" },
                { "el", "724767846dd1bf2157c0f75ba515099f0c1e6e46daf3c18c6072913dd824a792f93f3e986884af4c87cf759b8edfce8eaa282ebc81a5f4f03870bcef99368762" },
                { "en-CA", "51093c45173985b21fda251dff6135f2944e7b888227bc72544212a4bbcc390a5d1ebe7382dc66752031448a072ba1d1b7b9c968c9e37aa39e09c2d947da4982" },
                { "en-GB", "20fbe4a4f8ed388a8b97ce8be8b813ebe85b686d9270c47d87902d437979f9c6dfdcd4e27e4ce1966cdb3217885dac94d90f8c7252ca0e09386740ce0063ce15" },
                { "en-US", "c7b1740bf9fa70638c66b262e34aa8d84a672deb3f96693b11c6a8c01b2f15939ed8e411f088a056286c0ba16772ed069a09a55fc85bedd5ad0557fb939cbab7" },
                { "eo", "7fb7acce6f2732c1bd03ca061b25f8ce94b1ff266985b6bc1b20088e71e354884b1736f70cc92f6953f580775543d7b78857e08b04ce6aca2c49bbfcdbbd23b5" },
                { "es-AR", "9456fcd4ff0fb4a9f9284aec963df4ccf67d8e121a4d9c8d8117b4c164a2252561cae1c5e0f1430737e16c66c23e7094bf968647136c5c36f58f97196d228cc4" },
                { "es-CL", "df0cda9ba65625be873b8e406dc8e2583250658ce9194b52a494d64991e5ec7a846889ce40a5cb4794d0cd0930341c279d24ad9bbe66c3b04c46c16a516a0005" },
                { "es-ES", "7968fa26378286116d78a8c4c5a5f180a6d360963917ee680f450471747a1304342061731ddd0df01197c2d201a716b38eb0f47e2dbea788e8d87d8d97dd964f" },
                { "es-MX", "b7de4623032b0da9459a6dccf9b6f50346b7078ca6f255028775437c5dd43c10923dbf2c9a750ad201ad553ce4ce51a70815d5ff97ed2417caa9dde0104e7bed" },
                { "et", "3b6c0229513fab22ec9cffe63d1548132f8d2bd3ce155bb9195846218c300b7fab6dc830752092fae3dbe3f52d021eca735cc20696fd1af24dd3e432657e8123" },
                { "eu", "e37f8bb16989e24d1aa2c74b16b99942448c2ffa74f4bc744f5086c507c6cf4caddca905cee4806025c9b839881bcf1b3a769cfd37d80b48142d81b9b8578bd2" },
                { "fa", "61581143b01261dc324e5bcd6baa2be9488fddadc1bc5677d89cd0a2052cedee332cfab68b35a89c47935c3d7da3c2338efd7e992d8bcd5a69a0a457d6346107" },
                { "ff", "efb68fa33b68d1e3d8ee16b21e8f658db9c9214ed218b4cffc3fbf90d7709f245841ad4f79c9d89adbc32ff9504e7f60ec6398b6266cfeb9cd73721ddbf17ee7" },
                { "fi", "04869bb19f45aea5058a5221f495968e92d9ac0a1e70d235923732bbb49622f403b73d44e6e72e0b3cba358dcc510a8e6cba4ec6eb333cb58c195b8e40acb52c" },
                { "fr", "503caf3cc50a7b87e045e79b840d2839e8673533fa4ed9e80e89289e529cc5e0537ca749bd385f39570ce1cc7f3226c5a269048d6b49ee1d4c89f35c3ca1c4c5" },
                { "fur", "a49dfcc651c2a307b931b1ee0eab1b882816c7bc2421d4cc80d0330a18eafd8d259821c54ecda3a5250b9871f8bb0b13de393ab9b4dbc40b088976008c218fef" },
                { "fy-NL", "d5d7971509b00185a711925fdfbc900aa88a0798aea59f7d94dda9513a5734471e814bc5d1c2e847f1525df709cffafd8630417d94376addafdbfd5e4d5a7590" },
                { "ga-IE", "c248921f039fb8bbd9db50dea157bac9dc312ab4637216a028a01bac84d2d1df7391b41a4765115ba672da33a9dda4d16fb7ab94073d00d7c332d25e0efb1d8d" },
                { "gd", "4a6e3e60a1928c59aa0053b34d3fa6f950116dfbdb09262c3c88b7cfc3298790151f498cfa6ecd8f798a46aac0ae243516501cde9096716f479c500950d37b37" },
                { "gl", "f921dffcf61360e38d97cfdf0ee135c5dd4cd054afb1f441c8a39adead9bebc7bf41bdf12f8c6479e5f7859a972f56d6df99fbac178c4ab903337a1d4627b389" },
                { "gn", "579e7f5c4e8ab0b3630042dace72af4251b9b80be98ec72de9af2b55c3c82aa8314dc8947c027898eaf01eb1a56cf987e70f8c0ac7bab92d56f7c99bc4b5a396" },
                { "gu-IN", "43da22f0284f71f1cbaa3d59c4ae2a2662e680465da0be6d84a460010fb5d5d5f55bda16aef4fb17fd138b7ce46a673efdf63f355bd518083c6a275b4a35648f" },
                { "he", "cba53b5ca04e9d83e6edbc7b39d66b47963a418a71ae910b3030cc252b2f678daa102d906701c7e5d5631879e186be1bd96369e6a69bdf49fa9bbaecb0be8f72" },
                { "hi-IN", "8fa86c3b882b00a4d975a68f7e85e2c91ad6d5a98896cd0035b2b005f9eb3ffcd70f0448303e3e8e3265287e6f5f08c3651b2bb2c75a79268c4bad661803b972" },
                { "hr", "186caa56cfdf534c822cb0415c9e3c5abdd6ed9aa3d6f869e11b689e2034ec4413d25acca2f35b269f71c002f806725411a7c14b0804f58c0daf6c9f56d41553" },
                { "hsb", "d01d1029521bed302e8ba52debab4430416e574e1e7230e02bba897486531b09716814756ad207eef52940e25c2e01fc842c96ba2e2f1039d79cae53775b73e1" },
                { "hu", "85b2e4506f801af39efbccf0a0aae7e6a285216cb54ea91376d92df729a5d493f6627a70f6524d4ffff5d9bab754edd93f52a1e7dbd45ec40fd5295c14818ae6" },
                { "hy-AM", "154b8f1bdb3b043a215b3872426f927f7b09d36b90fc32691e65158c46677b5ac66d118f3677ff6fcef4543270c4bfccf92201aeab30232be37526d60e62f380" },
                { "ia", "38c396640c4dd4bbe2aaf0c48ac806aa3c8095edae4aaf3590f3bc3fc89545550342e054e2d2cff94fffefb8629b9930c20b9c2613e5f656e44b94e68766a79a" },
                { "id", "b86896327ae2f0af8538f7c208451b6d22dfcb1c89161cef99881b33492f0b4a75f007f7fef2b4f746935fdd999c6cdd380cff20334051c640b7d5352b501808" },
                { "is", "8b60f275baa60a3191218bc90d93851337fd8f67c39879a696366a2e050be01ec4460a87a2e57d5a80e15e8a356552ed16c577ba62bbf5ccdb837c197c21ff97" },
                { "it", "377ac848045178a8217241412a09f7a034c5117eb0d650667aabe89a0c9852a7cb32e1717ccc6492a41d841ed6547a2d65d3649dd691929dff8b1409a4205cfc" },
                { "ja", "cc613e0c8e9476b640aa7191f6f1bcc0dfd8b08fdf0c2ea6050c82c1ea85952b12b08a52f284e9f0c8881dd5c87708ddefed16f97b02af76d294ab19a7003b5e" },
                { "ka", "1eff2eaaaa2a45743584a700497c077a4c0613add47f64aebca7bb03b2d652829a63cb29eb137cb39e6aebf4bcd72b337403defa4110b7e62b35024778ec348a" },
                { "kab", "8b4e3e5fa87af5a75335b111625f3dedf589554fedcd979248d15532fd435c0c2fe5e1e33cc59924a074fb32e9bbeb2365fc8f0e366a69a44cdb275810db192f" },
                { "kk", "c100a0bba2e11723fd874724696449e9e4663625a7057825ca8c72f9a0d357e3a25d58ca0f8481d7149add5bb9442187e24e81ac1e364a230c63fa4462097a45" },
                { "km", "1c49a2597ca75e7bfff2b9770b128c274c5b0bc681fa3fa79a3942ae9287ad3e4b428da89167bc9183b3f8dbaacff535fa55e370674f114765d7f34098e9851a" },
                { "kn", "5e3cdc1e4cc257a2d4e2fd4c38525a01b737641d24c0a75201ec838cb06e8abcb8a0e8f5025913ba1f131ebc02bfcacbca166403d7051354f85a32b49525df52" },
                { "ko", "82a689f2d032137ab6de480e1ea0ecd6c6d140472ddd12d223416c5287fd8e95548bb6cb5ea83a0a2c9a5d94acab0d040f70167cc96cd5435f4b05b33a0aaf2b" },
                { "lij", "66db048885a69672b623ecdf4c37298d9bc117ac11ff399a0d355eb4429f2a8307bcb7f45cc4e277752e78adc8b5a9380b0962d8c5bada198b1b5672951e5c3b" },
                { "lt", "3542ff19aaf3020a3f953b04e3f81536cdb74567526694a9a1a19c64336837adc3a498d0bd7e88d8aa0118fccd46025e91e1ff73643fdc73763c5bc521dffefd" },
                { "lv", "dfe84c2c3c2f3742df3968f74f38b7dfca2c222e6097b3ec0e28757c89153d34a9e36e1d32855e7e9f0bb133e35c220f9444e7e5ba2b0fa402b7de78b2022717" },
                { "mk", "61ab87ee9a38863d471c88a80ce0b1d5194c63c575b2461cb7c5e0b717a110362f850fc17cf46d3f6b07e343e2d630e9b0042161b51490073a533819246d757f" },
                { "mr", "8f1b77d1b585b00222708051bf51e9073c9af6ba5d0d9a6de2e56521e7c0add3a1e0de4116e4e7d7dc3bdec9d2f73ab10cde00ed10f07b0bd6d5953f964779cf" },
                { "ms", "86bd1fe7bd42b3ce94b8b2551dab03eb0d572f03a76b661149dcc4aaaef537c8cc3f13f0cdd87041819c609832d2205f39e7e69f0aaf4746d2491a658466062c" },
                { "my", "0bb1be1303d9ebf531fad5b179de5b4712a2ee3201e9a8114ce3076eb319ddc2b767210153db5b9fac5e35ba2f8180075b885bc05fc52863187c7bc9c0b519cb" },
                { "nb-NO", "490320c418239108dd0b097f8760db24a45fc277c8dadbd759dd70c44593b878d8df6ba4aba8e070d88f2ceb3e4c28f8e5884ea446e65925fa3d75b0b96a0c01" },
                { "ne-NP", "3b23c0a916ebf969189080ae094652f13b2de4add5e1a8adf74a4a9f40b04cb4d325ea54fd426cf52ce4557d0ba3eb6e86b91cb58e8c49ecaa4e6f72a7c9ca0d" },
                { "nl", "b2bf3355fea47c60418b54e31d76ca180558f8df61d1956d5a33d44136b67575a2b061250d5455a582d39723988a9fc842ab78762928ca61b442f5ffd0a77efb" },
                { "nn-NO", "8544ea17fd79c28cc683e9f6c0ea454a7f884d1d34a221dfb4600aa64f011aafd40f3896230bb38d238b045768888f998d9905645a8bb489fa3d41df83a785a4" },
                { "oc", "9ba57b0a8f007aa75bc26f5872f10f61ea9bec8955bb873c4d596116281d8fd1e69189cd3e3aff97aa9f90a129bed1f36a9624d78e077a4b1fd729d05fd49b6e" },
                { "pa-IN", "a54fc371e53b46180b26caa9f4d2f867bbb463516ffbd2782cf6e995253f397c5eec0f20d5bb71c99112a63e5b6aaeb11e4c141c7928fab431a1d554f4b7762d" },
                { "pl", "a23c3a0e571639544570be2eaad01dfbc4d6582d3ce1d246e7819f9fb8bb4113d2b590036230e528b7311b1a6d2f879f523215f842dde01f0f38109edce83131" },
                { "pt-BR", "6d884d6882d19976ec7b77e016fae52e916bbfdab8a83b9a15c7df95912d9c6123717790d1daaa3624a0ed7594c2a566fd32c8173aa8bb7afb95a4f77ca5e9d6" },
                { "pt-PT", "157a57c4ad4e552b5f6fc976beda16dec673e4ff3abb37e76f3f990b33b7bc73c603190801ffb4939e2c0c4118046574d34959b7a4af851fdbbb66fa32ce0ea9" },
                { "rm", "875d38fc9ff0bd089e7f27e897210d20fc42da8626997e20e8fdb528961e7ea523d0740af196490ef12405f36febd092fba06b355f285e6746e58fc97f5a0853" },
                { "ro", "21c1a16e7162ee1c872509262f01d27001df0646d230837a2fbabbbb59fa6669b254e4dcbdbdab05d8d1bbb4edcffa383ca7bd0f60eb770fcb0341d131c5ef5c" },
                { "ru", "63560c0b7fb177f84b0d3307e3dfe0c83f9ace663a1e9156a5173b39c520c4497cf0aa8449e2f80219da6891aee52e9e64e5f539f275092b317ab487c5df613a" },
                { "sat", "b5cbb14e629338627efa4c7a9a786286d7ca11d7f85b948dccab59b443f0a62ad3748a332b85c3cc41c585e82ae63a92050b8dac34a29fedae64ef43863677ef" },
                { "sc", "99afd152f2bf4eb1e818ff84e84108c4ae6a15890c2c974c0958e3bfcfaf81d81f106551c3850c7fc6f05e466b81eaf10cfffe4e097fcb894bb9bb32d2119fdc" },
                { "sco", "60dd622fde1b3b4e162c7d8af0153f21e89a83df32511a76da0e95205ba041ad445abe70cef02be5365cd0d65a0713bd1d01cda9e870124d55298f4a854cf29d" },
                { "si", "7d9e8b5dcf4dc83549c16f466bf5f54ed15d9210c6a993063bea17b1bd90d13fcb9a2f81fd3e05a0487eee1cd2f83383ccd67b471f002d594a9462fd1c62d950" },
                { "sk", "d3bffe29fd85b50f92bdac153fe95e8973edc0c661abcf777716282bbdd7276c381ca4f1a115e10b4db04592543b230b78d74184628de077375fb34d8ce55cd9" },
                { "sl", "b01c3ac510f5def25d5a7859053f4d0451930effd1ec085daf5e05a776065d097cc19fae8aff12fe7457c616f147e8b58ff88e10b69dce780bfd8ecd30935eed" },
                { "son", "b40c013dae95c5d6dff3fb08033639c8f9f3a36da80e994e451a84e388c938e5d2e07544c7657c24216d30de6d4689da2faeb393f31da8cae7ef3843e26d22cf" },
                { "sq", "c68a8056bdf405fc415d8dde286f112cf0530fcddd252ffef6c55b163dcb1793b8e775adebc003f3ccfecba69c029c9b7839ad85d287d0226cd29320f9270a96" },
                { "sr", "9fd88afe3ec263c78d0d5c25bd6c35c5c0943bc1beb4952899814bdee8c4eede5517755a8154bc280ac1ebf9dcc74983e580846ed47881fca9aab972bdf6b50e" },
                { "sv-SE", "e984836def13ece93eabce99d2001484ed766208dcbea52b688d0048e8dcd723cba8db26e11386cd465733695f6095c84ce91a3243cef9772eab406382e156c2" },
                { "szl", "9c638322670c536c30c879b9b5369891d38502cf9b537fa49b3ae74b5ae4425966fd736bf4848fef90300291cae14845102ea1b4f137c09e694b3c8ac5b360da" },
                { "ta", "89a820ab2e62b1f252f59bff2595f8c4559bd043532ecb2c7a9b1531ba9aa08260440b2564be355f7476d31d4554645aac1db3f305e2db113a62d4bc18fa00e3" },
                { "te", "d2f3e1f4420182d16d6bf6feb2ee3b13a5f251ca07a67d1a37bb8d7908101d736fa71b28485b4f14779863409285628df8e06a5e8771ef61e3abc4b30d824007" },
                { "tg", "1b4fb8b098a1cf774b9f1b7eb5daea2d1c6aff1efd49d848ea105b392ce3f57f963bce75cd94f53c5cf30634e6c66495304c79fceb508a7e02b33e7a3d9afd6f" },
                { "th", "88627d945f0c16818973df091d87924aa1e2c266679cfde79a80d07412239c1e972d551233794b8886fdf12fc8714a26447a87ff304b09fa2741760ef9079441" },
                { "tl", "e6be1a8f8fcbc7227e19fd102f6bd1b7771005e432ea90d67c0d18b36efb94dc456ad6707ceb05dfce902140220c0fdffe782376f2105de67979f56199e12e97" },
                { "tr", "68aafc98ca1431bad1828c98d54d2451a5ba5d5516f3186128653266de01e245bda412f76c090fb453bcd3e2972e83cc0a2f71d1fe1b2cbe92c58b48d862f40b" },
                { "trs", "9ddf7b6f3dbf75be0879a43df9320e2ca8d2875edc28142171dfe542ed6e0d9e634ffbaaf3576bd68ae5e45903ed9f4e52deb871e8f81db64ba2ccfdee851742" },
                { "uk", "e7d81b170421ce750871ed4d56d06fc6c446fdbd8f4982b6e63513005c05fc208196825eebdb6f8b8e952d4afa9ed41576b293ee1cdd648587cff4711a6bebee" },
                { "ur", "da3ac3684dace5e56109af087940cd54d86ebaaa814932969b2f2d4d3dcce54ff48259539bbeb660f9bc405c4faee6aa3cbde27efc9bd5b57e94f67a07973aaa" },
                { "uz", "03600ccb965dfbe3292fc204fb0aa4eedddc23cf01fb330da2c35adc7a1fc059bfcb10267299d435fca1e96b43c367125343416b73cdf58d50942e2ce401f008" },
                { "vi", "126c79c95aa8236a61a61ce2a2c9fccfb35d2ed33e16e6c8ab7930af07f8d90815fd3455b810c7bab14576550867cda60d7ed5927d756f09bc95156782e1b407" },
                { "xh", "0bfcfcfcdb6bb5e7c3afd3556ac645ac9a82567ed37467610ef84f72592f05a54be196581f7c1bc2393bd7c5c53682ca8a9e75e1d3885541e0abaa36146f1efc" },
                { "zh-CN", "2f5212a829dbf0482e8e53ba17d79ad3f4fd0631ec31b5e58a90abd5a30d2d3469e9fecc6e8c65ea877b71c3df55d383ea48bd61d379b31716564bfe51cb6fe1" },
                { "zh-TW", "97991d88cc8326e7f59a437908418ef779e7d3bd3f1cb731b986ba5a1cfa1c7346451cb103ab964c5b398f9ea50e2d75745b68c4862bdfa973d78e039b6daede" }
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
            const string knownVersion = "126.0";
            var signature = new Signature(publisherX509, certificateExpiration);
            return new AvailableSoftware("Mozilla Firefox (" + languageCode + ")",
                knownVersion,
                "^Mozilla Firefox ([0-9]+\\.[0-9](\\.[0-9])? )?\\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Mozilla Firefox ([0-9]+\\.[0-9](\\.[0-9])? )?\\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/firefox/releases/" + knownVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + knownVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/firefox/releases/" + knownVersion + "/win64/" + languageCode + "/Firefox%20Setup%20" + knownVersion + ".exe",
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
            return new string[] { "firefox", "firefox-" + languageCode.ToLower() };
        }


        /// <summary>
        /// Tries to find the newest version number of Firefox.
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        public string determineNewestVersion()
        {
            string url = "https://download.mozilla.org/?product=firefox-latest&os=win&lang=" + languageCode;
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
                response = null;
                client = null;
                var reVersion = new Regex("[0-9]{2,3}\\.[0-9](\\.[0-9])?");
                Match matchVersion = reVersion.Match(newLocation);
                if (!matchVersion.Success)
                    return null;
                string currentVersion = matchVersion.Value;

                return currentVersion;
            }
            catch (Exception ex)
            {
                logger.Warn("Error while looking for newer Firefox version: " + ex.Message);
                return null;
            }
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
             * https://ftp.mozilla.org/pub/firefox/releases/51.0.1/SHA512SUMS
             * Common lines look like
             * "02324d3a...9e53  win64/en-GB/Firefox Setup 51.0.1.exe"
             */

            string url = "https://ftp.mozilla.org/pub/firefox/releases/" + newerVersion + "/SHA512SUMS";
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
                logger.Warn("Exception occurred while checking for newer version of Firefox: " + ex.Message);
                return null;
            }

            // look for line with the correct language code and version for 32 bit
            var reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum32Bit = reChecksum32Bit.Match(sha512SumsContent);
            if (!matchChecksum32Bit.Success)
                return null;
            // look for line with the correct language code and version for 64 bit
            var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum64Bit = reChecksum64Bit.Match(sha512SumsContent);
            if (!matchChecksum64Bit.Success)
                return null;
            // checksum is the first 128 characters of the match
            return new string[] { matchChecksum32Bit.Value[..128], matchChecksum64Bit.Value[..128] };
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
            logger.Info("Searching for newer version of Firefox...");
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
                // failure occurred
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
        /// language code for the Firefox ESR version
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
    } // class
} // namespace
